using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AggregateOrderElements
{
    class Store
    {
        [FunctionName("Store")]
        public static async Task Run(
            [ActivityTrigger] string orderCombined,
            [CosmosDB(
                databaseName: "%CosmosDb.Database%",
                collectionName: "%CosmosDb.Collection%",
                ConnectionStringSetting = "CosmosDb.ConnectionString")] IAsyncCollector<dynamic> documentsOut)
        {
            JArray orders = JArray.Parse(orderCombined);

            foreach(JObject order in orders)
            {
                await documentsOut.AddAsync(order);
            }

        }
    }
}
