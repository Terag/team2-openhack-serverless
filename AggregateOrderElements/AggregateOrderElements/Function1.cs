using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AggregateOrderElements
{
    public class FilesAggregationState
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; } = "NEW";

        [JsonProperty("orderHeaderDetailsAvailable")]
        public bool OrderHeaderDetailsAvailable { get; set; } = false;

        [JsonProperty("orderLineItems")]
        public bool OrderLineItems { get; set; } = false;

        [JsonProperty("productInformation ")]
        public bool ProductInformation { get; set; } = false;
    }

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

        public static void Store()
        {
            //TO DO
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
