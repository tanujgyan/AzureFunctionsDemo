using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFunctionsDemo;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public static class CosmosTrigger1
    {
        [FunctionName("CosmosTrigger1")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "Tasks",
            collectionName: "Order",
            ConnectionStringSetting = "cosmosteranet_DOCUMENTDB",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists =true)]IReadOnlyList<Document> input, ILogger log,
            [SignalR(HubName = "transferchartdata")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
                await signalRMessages.AddAsync(
        new SignalRMessage
        {
            Target = "transferchartdata",
            Arguments = new[] {""}
        });
            }
        }
        [FunctionName("negotiate")]
        public static SignalRConnectionInfo Negotiate(
     [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
     [SignalRConnectionInfo(HubName = "transferchartdata")] SignalRConnectionInfo connectionInfo)
        {
            return connectionInfo;
        }
    }
}
