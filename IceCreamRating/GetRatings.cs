using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Dynamic;
using RestSharp;
using System.Collections.Generic;
using System.Linq;

namespace Company.Function
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
                        [CosmosDB(
                // databaseName: "icecreamdb",
                databaseName: "%CosmosDb.Database%",
                collectionName: "%CosmosDb.Collection%",
                ConnectionStringSetting = "CosmosDb.ConnectionString")]
        IEnumerable<dynamic> allRatings)
        {
            string userId = null;

            if (req.GetQueryParameterDictionary()?.TryGetValue(@"userId", out userId) == true
                && !string.IsNullOrWhiteSpace(userId))
            {
                var userRatings = allRatings.Where(r => r.Value<string>(@"userId") == userId);

                return !userRatings.Any() ? new NotFoundObjectResult($@"No ratings found for user '{userId}'") : (IActionResult)new OkObjectResult(userRatings);

            }
            else
            {
                return new BadRequestObjectResult(@"userId is required as a query parameter");
            }
        }
    }
}
