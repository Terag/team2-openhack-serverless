using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AggregateOrderElements
{
    public class OrchestratorInput
    {
        public string OrderId { get; set; }
        public string File { get; set; }
    }

    public static class NewFile
    {
        [FunctionName("NewFile")]
        public static async Task Run([BlobTrigger("orders/{orderId}-{file}.csv",
            Connection = "Blob.ConnectionString")]Stream myBlob,
            string orderId,
            string file,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var input = new OrchestratorInput
            {
                OrderId=orderId,
                File=file
            };
            await starter.StartNewAsync("Orchestrator", orderId + "-" + file, input);
        }

        public static async Task Store(string orderId, dynamic orderContent)
        {
            CosmosClient client = new CosmosClient("AccountEndpoint=https://icecreamrating-cosmos.documents.azure.com:443/;AccountKey=1TRq4McjHzssbHkYJ4lwkwwNoEjpQY6zkcEX3xGC2NN7wHCqvomiHyhgMGX27s4Npzz7a48qUUoqQuUFXMsbfw==;");

            Container container = client.GetContainer("bfyoc - team2", "orders");

            ItemResponse<dynamic> orderResponse = await container.CreateItemAsync(orderContent, new PartitionKey(orderId));
        }
    }
}
