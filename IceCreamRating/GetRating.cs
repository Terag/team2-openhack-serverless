

namespace Company.Function
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc; 
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

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
                Id = "{ratingId}",
                PartitionKey = "{ratingId}"
                )] RatingItem documentOut,
            ILogger log) 
        {
            
            return documentOut != null ? (IActionResult)new OkObjectResult(documentOut) : new NotFoundObjectResult($@"No rating found");
        }

    }
}
