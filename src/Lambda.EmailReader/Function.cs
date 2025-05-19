using System.Diagnostics;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace HsaLedger.Lambda.EmailReader;

public class Function
{
    
    public async Task FunctionHandler(object? input, ILambdaContext context)
    {
        context.Logger.LogInformation($"{input}");
        // start stop watch timer
        var stopWatch = Stopwatch.StartNew();

        var serviceProvider = await context.InitializeContainer();
        
        // API is initialized at this point
        

        // stop timer and log completion
        stopWatch.Stop();
        context.Logger.LogInformation($"Completed in {stopWatch.ElapsedMilliseconds}ms");
    }
}