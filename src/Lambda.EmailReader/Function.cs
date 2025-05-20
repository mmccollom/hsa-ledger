using System.Diagnostics;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using HsaLedger.Application.Requests;
using HsaLedger.Client.Infrastructure.Managers.Interfaces;
using HsaLedger.Lambda.Infrastructure;
using HsaLedger.Lambda.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HsaLedger.Lambda.EmailReader;

public class Function
{
    private readonly IAmazonS3 _s3;
    private readonly ITransactionManager _transactionManager;
    private readonly IProviderManager _providerManager;
    private readonly IPersonManager _personManager;
    private readonly ITransactionTypeManager _transactionTypeManager;

    public Function()
    {
        var services = new ServiceCollection();
        
        // Set up Configuration
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Needed for reading appsettings.json
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        var configuration = configurationBuilder.Build();
        
        // Get API URL from configuration
        var apiUrl = configuration["ApiUrl"] ?? throw new InvalidOperationException();
        
        // Parse "SecretInfo" from configuration as SecretInfo class
        var secretInfo = configuration.GetSection("SecretInfo").Get<SecretInfo>() ?? throw new InvalidOperationException();
        
        // Register secret info
        services.AddSingleton(secretInfo);
        
        // Register configuration
        services.AddSingleton(configuration);
        
        // Register services
        services.AddInfrastructureServices(apiUrl);
        services.AddLambdaServices();
        
        // Build DI container
        var serviceProvider = services.BuildServiceProvider();

        // Intentionally eagerly build Function and its dependencies.
        // This causes time spent in the Constructor to appear on INIT_REPORTs
        _s3 = serviceProvider.GetRequiredService<IAmazonS3>();
        _transactionManager = serviceProvider.GetRequiredService<ITransactionManager>();
        _providerManager = serviceProvider.GetRequiredService<IProviderManager>();
        _personManager = serviceProvider.GetRequiredService<IPersonManager>();
        _transactionTypeManager = serviceProvider.GetRequiredService<ITransactionTypeManager>();
    }

    public async Task FunctionHandler(S3Event s3Event, ILambdaContext context)
    {
        // start stop watch timer
        var stopWatch = Stopwatch.StartNew();
        
        // get providers
        var providers = await _providerManager.Get();

        if (!providers.Succeeded || providers.Data == null)
        {
            if (providers.Messages != null)
            {
                foreach(var message in providers.Messages)
                {
                    context.Logger.LogError(message);
                }
            }
            else
            {
                context.Logger.LogError("Failed to get providers");
            }
        }
        
        // get persons
        var persons = await _personManager.Get();
        
        if (!persons.Succeeded || persons.Data == null)
        {
            if (persons.Messages != null)
            {
                foreach(var message in persons.Messages)
                {
                    context.Logger.LogError(message);
                }
            }
            else
            {
                context.Logger.LogError("Failed to get persons");
            }
        }
        
        // get transaction types
        var transactionTypes = await _transactionTypeManager.Get();
        
        if (!transactionTypes.Succeeded || transactionTypes.Data == null)
        {
            if (transactionTypes.Messages != null)
            {
                foreach(var message in transactionTypes.Messages)
                {
                    context.Logger.LogError(message);
                }
            }
            else
            {
                context.Logger.LogError("Failed to get transaction types");
            }
        }
        
        // get placeholders
        var provider = providers.Data!.First(x => x.Name == "Placeholder");
        var person = persons.Data!.First(x => x.Name == "Placeholder");
        var transactionType = transactionTypes.Data!.First(x => x.Code == "Placeholder");
        
        context.Logger.LogInformation($"Provider: {provider.Name}");
        context.Logger.LogInformation($"Person: {person.Name}");
        context.Logger.LogInformation($"Transaction Type: {transactionType.Code}");
        
        foreach (var record in s3Event.Records)
        {
            var bucket = record.S3.Bucket.Name;
            var key    = record.S3.Object.Key;

            context.Logger.LogInformation($"Reading object {key} from bucket {bucket}");

            // fetch the raw email
            var getReq = new GetObjectRequest { BucketName = bucket, Key = key };
            using var s3Obj  = await _s3.GetObjectAsync(getReq);
            await using var emlStream = s3Obj.ResponseStream;

            // parse with MimeKit
            var message = await MimeMessage.LoadAsync(emlStream);
            
            // create a new transaction
            var transactionRequest = new AddTransactionRequest
            {
                TransactionTypeId = transactionType.TransactionTypeId,
                ProviderId = provider.ProviderId,
                PersonId = person.PersonId,
                Date = DateTime.Today,
                Amount = 0,
                IsPaid = false,
                IsHsaWithdrawn = false,
                IsAudited = false,
                Documents = []
            };

            // iterate attachments
            foreach (var attachment in message.Attachments)
            {
                if (attachment is MimePart part)
                {
                    var fileName = part.FileName ?? "unnamed";
                    context.Logger.LogInformation($"Found attachment: {fileName}");

                    using var ms = new MemoryStream();
                    await part.Content.DecodeToAsync(ms);
                    ms.Position = 0;

                    var content = ms.ToArray();
                    
                    transactionRequest.Documents.Add(new AddDocumentRequest
                    {
                        Fullname = fileName,
                        Name = Path.GetFileNameWithoutExtension(fileName),
                        Extension = Path.GetExtension(fileName),
                        Content = content,
                    });
                    context.Logger.LogInformation($"New addDocumentRequest created for {fileName}");
                }
            }
            // add transaction
            var transactionResult = await _transactionManager.Put(transactionRequest);
            if (transactionResult.Succeeded)
            {
                context.Logger.LogInformation($"Transaction created successfully with ID: {transactionResult.Data}");
            }
            else
            {
                if (transactionResult.Messages != null)
                {
                    foreach(var error in transactionResult.Messages)
                    {
                        context.Logger.LogError(error);
                    }
                }
                else
                {
                    context.Logger.LogError("Failed to create transaction");    
                }
            }
        }

        // stop timer and log completion
        stopWatch.Stop();
        context.Logger.LogInformation($"Completed in {stopWatch.ElapsedMilliseconds}ms");
    }
}