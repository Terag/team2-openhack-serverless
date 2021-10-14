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
    public static class NewFile
    {
        [FunctionName("NewFile")]
        public static void Run([BlobTrigger("orders/{orderId}-{file}",
            Connection = "DefaultEndpointsProtocol=https;AccountName=bfyocstorageaccount;AccountKey=3TaClxdiwSGsxQs46GFSNMS92jEtvBAxzfnWuhWWfxjFva5TYdLlhGsXLCNkHOnyreyoEOMFWHfdz7FqW52cDA==;EndpointSuffix=core.windows.net")]Stream myBlob,
            string orderId,
            string file,
            IDurableOrchestrationClient starter,
            ILogger log)
        {
            starter.StartNewAsync("Orchestrator", orderId, file);
        }

        public static async Task Store(string orderId, dynamic orderContent)
        {
            CosmosClient client = new CosmosClient("AccountEndpoint=https://icecreamrating-cosmos.documents.azure.com:443/;AccountKey=1TRq4McjHzssbHkYJ4lwkwwNoEjpQY6zkcEX3xGC2NN7wHCqvomiHyhgMGX27s4Npzz7a48qUUoqQuUFXMsbfw==;");

            Container container = client.GetContainer("bfyoc - team2", "orders");

            ItemResponse<dynamic> orderResponse = await container.CreateItemAsync(orderContent, new PartitionKey(orderId));
        }
    }
}
