using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace AggregateOrderElements
{
    public class Orchestrator
    {

        [FunctionName("Orchestrator")]
        public static async Task<int> Run([OrchestrationTrigger] IDurableOrchestrationContext context, string entityKey, string fileType)
        {
            var entityId = new EntityId("FilesAggregationState", entityKey);

            // One-way signal to the entity - does not await a response
            if (fileType == "OrderHeaderDetails")
            {
                context.SignalEntity(entityId, "SetOrderHeaderDetailsAvailable");
            }

            if (fileType == "OrderLineItems")
            {
                context.SignalEntity(entityId, "SetOrderLineItemsAvailable");
            }

            if (fileType == "ProductInformation")
            {
                context.SignalEntity(entityId, "SetProductInformationAvailable");
            }

            // Two-way call to the entity which returns a value - awaits the response
            bool state = await context.CallEntityAsync<bool>(entityId, "GetState");
            if (state)
            {
                // Call Combine
                await context.CallActivityAsync<string>("Combine", entityKey);
                // Call Store
                await context.CallActivityAsync<string>("Store", entityKey);
            }

            return 0;
        }
    }
}
