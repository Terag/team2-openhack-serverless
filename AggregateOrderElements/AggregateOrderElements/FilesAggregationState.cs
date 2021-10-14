using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AggregateOrderElements
{
    public class FilesAggregationState
    {
        [JsonProperty("orderId")]
        public string OrderId { get; set; } = "NEW";

        [JsonProperty("orderHeaderDetailsAvailable")]
        public bool OrderHeaderDetailsAvailable { get; set; } = false;

        [JsonProperty("orderLineItemsAvailable")]
        public bool OrderLineItemsAvailable { get; set; } = false;

        [JsonProperty("productInformationAvailable")]
        public bool ProductInformationAvailable { get; set; } = false;

        public void SetOrderHeaderDetailsAvailable() => OrderHeaderDetailsAvailable = true;
        public void SetOrderLineItemsAvailable() => OrderLineItemsAvailable = true;
        public void SetProductInformationAvailable() => ProductInformationAvailable = true;
        public bool GetState() => OrderHeaderDetailsAvailable && OrderLineItemsAvailable && ProductInformationAvailable;

        [FunctionName(nameof(FilesAggregationState))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx) => ctx.DispatchAsync<FilesAggregationState>();
    }
}
