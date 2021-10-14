using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
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


    public static class OrdersAggregatorFunctions
    {

        [FunctionName("NewFileArrived")]
        public static async Task NewFileArrived(
            [BlobTrigger("orders/{orderId}-{part}.csv", Connection = "OrderStorageCnxStr")] Stream myBlob, string orderId, string part,
            [DurableClient] IDurableEntityClient entityClient,
            ILogger log)
        {
            log.LogInformation($"New partial file arrive : Name={orderId}    part={part}");

            var entityId = new EntityId("OrdersAggregator", orderId);
            await entityClient.SignalEntityAsync(entityId, $"{part}");
        }



        [FunctionName("OrdersAggregator")]
        public static async Task OrdersAggregator(
            [EntityTrigger]IDurableEntityContext ctx, 
            ILogger log)
        {
            log.LogInformation($"OrdersAggregator: operationName={ctx.OperationName} for orderId={ctx.EntityKey}");

            var currentState = ctx.GetState<FilesAggregationState>(() => new FilesAggregationState() { OrderId = ctx.EntityKey });

            log.LogInformation($"#{currentState.OrderId}# : READ State [ {currentState.OrderHeaderDetailsAvailable} - {currentState.OrderLineItems} - {currentState.ProductInformation} ]");

            switch (ctx.OperationName)
            {
                case "OrderHeaderDetails":
                    if (currentState.OrderHeaderDetailsAvailable)
                        log.LogWarning($"#{currentState.OrderId}# : duplicate OrderHeaderDetailsAvailable  received !");
                    currentState.OrderHeaderDetailsAvailable = true;
                    break;
                case "OrderLineItems":
                    if (currentState.OrderLineItems)
                        log.LogWarning($"#{currentState.OrderId}# : duplicate OrderLineItems received !");
                    currentState.OrderLineItems = true;
                    break;
                case "ProductInformation":
                    if (currentState.ProductInformation)
                        log.LogWarning($"#{currentState.OrderId}# : duplicate ProductInformation received !");
                    currentState.ProductInformation = true;
                    break;
            }
            ctx.SetState(currentState);

            log.LogInformation($"#{currentState.OrderId}# : SAVE State [ {currentState.OrderHeaderDetailsAvailable} - {currentState.OrderLineItems} - {currentState.ProductInformation} ]");

            if (currentState.OrderHeaderDetailsAvailable && currentState.OrderLineItems && currentState.ProductInformation)
            {
                log.LogInformation($"#{currentState.OrderId}# : 3 PARTS RECEIVED ==> Merging ...");
                var mergedJson = await AggregateCsv(currentState.OrderId, log);
                log.LogWarning($"#{currentState.OrderId}# : Merge result = {mergedJson} ...");
                ctx.DeleteState(); 
            }
        }

        public static async Task<string> AggregateCsv(string orderId,ILogger log)
        {
            string saName = System.Environment.GetEnvironmentVariable("OrderStorageAccountName"); // "h6ncsa";
            string apiUrl = System.Environment.GetEnvironmentVariable("MergeApiUrl"); // "https://serverlessohmanagementapi.trafficmanager.net/api/order/combineOrderContent";// Beurk mais ca marche pour tester ...


            string body = "{" +
                $"    \"orderHeaderDetailsCSVUrl\" : \"https://{saName}.blob.core.windows.net/orders/{orderId}-OrderHeaderDetails.csv\" , " +
                $"    \"orderLineItemsCSVUrl\" : \"https://{saName}.blob.core.windows.net/orders/{orderId}-OrderLineItems.csv\" , " +
                $"    \"productInformationCSVUrl\" : \"https://{saName}.blob.core.windows.net/orders/{orderId}-ProductInformation.csv\" " +
                "}";

            var httpClient = new HttpClient();
            var content = new StringContent(body);
            var response= await httpClient.PostAsync(apiUrl, content);

            string txt = await response.Content.ReadAsStringAsync();
            if (response.StatusCode!=System.Net.HttpStatusCode.OK)
            {
                txt = $"ERREUR : {response.StatusCode} " + txt;
            }
            else
            {
                dynamic d = JsonConvert.DeserializeObject<dynamic>(txt);
            }
            return txt;
        }
    }
}
