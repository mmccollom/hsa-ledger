using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
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
    private readonly IAmazonSimpleEmailService _ses;
    private readonly ITransactionManager _transactionManager;
    private readonly IProviderManager _providerManager;
    private readonly IPersonManager _personManager;
    private readonly ITransactionTypeManager _transactionTypeManager;
    private readonly string _emailSendFrom;

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
        
        // Parse "EmailInfo" from configuration as EmailInfo class
        var emailInfo = configuration.GetSection("EmailInfo").Get<EmailInfo>() ?? throw new InvalidOperationException();
        _emailSendFrom = emailInfo.SendFrom;
        
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
        _ses = serviceProvider.GetRequiredService<IAmazonSimpleEmailService>();
        _transactionManager = serviceProvider.GetRequiredService<ITransactionManager>();
        _providerManager = serviceProvider.GetRequiredService<IProviderManager>();
        _personManager = serviceProvider.GetRequiredService<IPersonManager>();
        _transactionTypeManager = serviceProvider.GetRequiredService<ITransactionTypeManager>();
    }

    public async Task FunctionHandler(S3Event s3Event, ILambdaContext context)
    {
        #region Get providers, people, and transaction types from the API
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

            return;
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

            return;
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

            return;
        }
        #endregion
        
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

            if (transactionRequest.Documents.Count == 0)
            {
                context.Logger.LogInformation("No documents found in email, skipping");

                continue;
            }
            // add transaction
            var transactionResult = await _transactionManager.Put(transactionRequest);
            if (transactionResult.Succeeded)
            {
                context.Logger.LogInformation($"Transaction created successfully with ID: {transactionResult.Data}");

                var recipient = message.From.Mailboxes.FirstOrDefault()?.Address;
                var subject = message.Subject;
                var messageId = message.MessageId;

                if (string.IsNullOrWhiteSpace(recipient))
                {
                    context.Logger.LogInformation("Recipient address not set, skipping response email");
                    continue;
                }

                context.Logger.LogInformation($"Recipient: {recipient}");

                // Construct reply email using MimeKit
                var reply = new MimeMessage();
                reply.From.Add(MailboxAddress.Parse(_emailSendFrom)); // Must be SES-verified
                reply.To.Add(MailboxAddress.Parse(recipient));
                reply.Subject = $"Re: {subject}";
                reply.InReplyTo = messageId;
                reply.References.Add(messageId);

                reply.Body = new TextPart("plain")
                {
                    Text = "Thank you! Your HSA documents were successfully processed and recorded."
                };

                using var memStream = new MemoryStream();
                await reply.WriteToAsync(memStream);
                var rawMessage = new RawMessage
                {
                    Data = new MemoryStream(memStream.ToArray())
                };

                var sendRequest = new SendRawEmailRequest
                {
                    RawMessage = rawMessage,
                    Source = _emailSendFrom,
                    Destinations = [recipient]
                };

                try
                {
                    var response = await _ses.SendRawEmailAsync(sendRequest);
                    context.Logger.LogInformation($"Reply sent to {recipient}, message ID: {response.MessageId}");
                }
                catch (Exception ex)
                {
                    context.Logger.LogError($"Failed to send confirmation reply: {ex.Message}");
                }
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
    }
}