using System;
using System.IO;
using System.Net.Http;
using System.Text;
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
            string orderHeaderFileUri = "https://bfyocstorageaccount.blob.core.windows.net/orders/" + orderId + "-OrderHeaderDetails.csv";
            string orderLinesFileUri = "https://bfyocstorageaccount.blob.core.windows.net/orders/" + orderId + "-OrderLineItems.csv";
            string orderInformationFileUri = "https://bfyocstorageaccount.blob.core.windows.net/orders/" + orderId + "-ProductInformation.csv";

            String strRequestUrl = "https://serverlessohmanagementapi.trafficmanager.net/api/order/combineOrderContent";

            bool ret = false;

            HttpClient client = new HttpClient();

            HttpRequestMessage myRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(strRequestUrl),
                Headers = {
                },
                Content = new StringContent("{" +
                    $"\"orderHeaderDetailsCSVUrl\": \"{orderHeaderFileUri}\"," +
                    $"\"orderLineItemsCSVUrl\": \"{orderLinesFileUri}\"," +
                    $"\"productInformationCSVUrl\": \"{orderInformationFileUri}\"" +
                "}", Encoding.UTF8, "application/json")
            };

            HttpResponseMessage res = null;

            var t = Task.Run(async () =>
            {
                res = await client.SendAsync(myRequest);
            });
            t.Wait();

            if(res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return await res.Content.ReadAsStringAsync();
            }
            else
            {
                return "";
            }

        }
    }
}
