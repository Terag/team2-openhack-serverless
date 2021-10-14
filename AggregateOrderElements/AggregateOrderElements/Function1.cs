using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace AggregateOrderElements
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([BlobTrigger("orders/{name}", Connection = "DefaultEndpointsProtocol=https;AccountName=bfyocstorageaccount;AccountKey=3TaClxdiwSGsxQs46GFSNMS92jEtvBAxzfnWuhWWfxjFva5TYdLlhGsXLCNkHOnyreyoEOMFWHfdz7FqW52cDA==;EndpointSuffix=core.windows.net")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }

        public static string ExtractId(string name)
        {
            return "";
        }

        public static async Task Store(string orderId, dynamic orderContent)
        {
            CosmosClient client = new CosmosClient("AccountEndpoint=https://icecreamrating-cosmos.documents.azure.com:443/;AccountKey=1TRq4McjHzssbHkYJ4lwkwwNoEjpQY6zkcEX3xGC2NN7wHCqvomiHyhgMGX27s4Npzz7a48qUUoqQuUFXMsbfw==;");

            Container container = client.GetContainer("bfyoc - team2", "orders");
            
            ItemResponse<dynamic> orderResponse = await container.CreateItemAsync(orderContent, new PartitionKey(orderId));
        }

        public static void CombineOrder()
        {
            //TO DO
        }

        // Retrieve the state, save the new state, if the state is complete, it triggers CombineOrder and Store the final result.
        public static void Orchestrator()
        {
            //TO DO
        }
    }
}
