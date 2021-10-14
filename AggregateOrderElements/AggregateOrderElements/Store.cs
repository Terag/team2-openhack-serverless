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
        public static async Task<int> Run(
            [CosmosDB(
                databaseName: "%CosmosDb.Database%",
                collectionName: "%CosmosDb.Collection%",
                ConnectionStringSetting = "CosmosDb.ConnectionString")] IAsyncCollector<dynamic> documentsOut,
            string orderCombined)
        {
            dynamic order = JObject.Parse(orderCombined);

            await documentsOut.AddAsync(order);

            return 0;
        }
    }
}
