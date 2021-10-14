using System;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AggregateOrderElements
{
    public static class CombineOrder
    {
        [FunctionName("Combine")]
        public static async Task<string> CombineFiles([ActivityTrigger] IDurableActivityContext context)
        {
            //public static async Task<int
            string orderId = context.GetInput<string>();

            var storageAccountConnetionString = "DefaultEndpointsProtocol=https;AccountName=bfyocstorageaccount;AccountKey=3TaClxdiwSGsxQs46GFSNMS92jEtvBAxzfnWuhWWfxjFva5TYdLlhGsXLCNkHOnyreyoEOMFWHfdz7FqW52cDA==;EndpointSuffix=core.windows.net";
            var account = CloudStorageAccount.Parse(storageAccountConnetionString);

            var cloudBlobClient = account.CreateCloudBlobClient();
            var container  = cloudBlobClient.GetContainerReference("orders");


            using (var orderHeader = await container.GetBlobReferenceFromServerAsync($"{orderId}-OrderHeaderDetails.csv").GetCsvReader())
            {
                
            }
            return "wip";
        }

        public static async Task<CsvReader> GetCsvReader(this Task<ICloudBlob> cloudBlob)
        {
            Stream stream = await cloudBlob.Result.OpenReadAsync();
            StreamReader streamReader = new StreamReader(stream);

            return new CsvReader(streamReader, System.Globalization.CultureInfo.InvariantCulture);
        }

    }
}
