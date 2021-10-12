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
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "%CosmosDb.Database%",
                collectionName: "%CosmosDb.Collection%",
                ConnectionStringSetting = "CosmosDb.ConnectionString")] IAsyncCollector<dynamic> documentsOut,
            ILogger log)
        {
            dynamic data = await JsonSerializer.DeserializeAsync<ExpandoObject>(req.Body);
            string productId = data?.productId.GetString();
            string userId = data?.userId.GetString();
            int rating = data?.rating.GetInt32();

            // Call GetProduct API
            string getProductUrl = "https://serverlessohapi.azurewebsites.net";

            var client = new RestClient(getProductUrl);
            var request = new RestRequest("/api/GetProduct", Method.GET);
            request.AddQueryParameter("productId", productId);
            IRestResponse response = await client.ExecuteAsync(request);
            if (rating <= 5 && rating >=0 && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                request = new RestRequest("/api/GetUser", Method.GET);
                request.AddQueryParameter("userId", userId);
                response = await client.ExecuteAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    await documentsOut.AddAsync(new {
                        id = Guid.NewGuid(),
                        timestamp = DateTime.UtcNow.ToString("o"),
                        userId = userId,
                        productId = productId,
                        rating = rating,
                        locationName = data?.locationName.GetString(),
                        userNotes = data?.userNotes.GetString()
                    });
                    return new OkObjectResult("success");
                }
            }

            return new BadRequestResult();
        }
    }
}
