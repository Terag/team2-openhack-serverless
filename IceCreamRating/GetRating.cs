using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Dynamic;
using RestSharp;

namespace Company.Function
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Function, "get", "post", 
                Route = "rating/{ratingId}")] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDb.Database%",
                collectionName: "%CosmosDb.Collection%",
                ConnectionStringSetting = "CosmosDb.ConnectionString",
                SqlQuery = "select * from ratings r where r.id = {ratingId}")] IAsyncCollector<dynamic> documentOut,
            ILogger log)
        {
            return documentOut != null ? new NotFoundObjectResult($@"No rating found'") : (IActionResult)new OkObjectResult(documentOut);
        }

    }
}
