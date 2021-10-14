using System;
using System.IO;
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
    }
}
