using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace AggregateOrderElements
{
    public class Orchestrator
    {

        [FunctionName("Orchestrator")]
        public static async Task<int> Run([OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var input = context.GetInput<OrchestratorInput>();

            var entityId = new EntityId(nameof(FilesAggregationState), input.OrderId);

            // One-way signal to the entity - does not await a response
            if (input.File == "OrderHeaderDetails")
            {
                context.SignalEntity(entityId, "SetOrderHeaderDetailsAvailable");
            }

            if (input.File == "OrderLineItems")
            {
                context.SignalEntity(entityId, "SetOrderLineItemsAvailable");
            }

            if (input.File == "ProductInformation")
            {
                context.SignalEntity(entityId, "SetProductInformationAvailable");
            }

            // Two-way call to the entity which returns a value - awaits the response
            bool state = await context.CallEntityAsync<bool>(entityId, "GetState");
            if (state)
            {
                // Call Combine
                string orderCombined = await context.CallActivityAsync<string>("Combine", input.OrderId);
                // Call Store
                if(!string.IsNullOrWhiteSpace(orderCombined))
                {
                    await context.CallActivityAsync<string>("Store", orderCombined);
                }
            }

            return 0;
        }
    }
}
