using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Company.Function
{
    public static class VerifyHelper
    {

        private static bool verifyUser(string userid, ILogger log) 
        {
            log.LogInformation("<-- verifyUser(): "+userid);

            // Check things before save
            String strRequestUrl = "https://serverlessohapi.azurewebsites.net/api/GetUser?userId=" + userid;

            bool ret = false;

            HttpClient client = new HttpClient();

            HttpRequestMessage myRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(strRequestUrl),
                Headers = {
                },
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };

            HttpResponseMessage res = null;

            var t = Task.Run(async () =>
            {
                res = await client.SendAsync(myRequest);
            });
            t.Wait();

            if (res.IsSuccessStatusCode)
            {
                log.LogInformation("verify user call returned 200 OK!");
                string strResponse = null;
                dynamic model = null;

                var t2 = Task.Run(async () =>
                {
                    strResponse = await res.Content.ReadAsStringAsync();
                });
                t2.Wait();

                model = JsonConvert.DeserializeObject(strResponse);

                // Everything ok, return success.
                ret = true;

                // If the call went through, the user is fine.

                /*
                    {{
                      "userId": "cc20a6fb-a91f-4192-874d-132493685376",
                      "userName": "doreen.riddle",
                      "fullName": "Doreen Riddle"
                    }}                 
                 */
            }
            else
            {
                log.LogInformation("verify user call returned non 200 status = " + res.StatusCode);
                ret = false;
            }

            log.LogInformation("<-- verifyUser() returning: " + ret);

            return ret;
        }

        private static bool verifyProduct(string productid, ILogger log)
        {
            log.LogInformation("<-- verifyProduct(): " + productid);

            String strRequestUrl = "https://serverlessohapi.azurewebsites.net/api/GetProduct?productId="+productid;
            bool ret = false;

            HttpClient client = new HttpClient();

            HttpRequestMessage myRequest = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(strRequestUrl),
                Headers = {
                },
                Content = new StringContent("", Encoding.UTF8, "application/json")
            };

            HttpResponseMessage res = null;

            var t = Task.Run(async () =>
            {
                res = await client.SendAsync(myRequest);
            });
            t.Wait();

            if (res.IsSuccessStatusCode)
            {
                log.LogInformation("verify product call returned 200 OK!");
                string strResponse = null;
                dynamic model = null;

                var t2 = Task.Run(async () =>
                {
                    strResponse = await res.Content.ReadAsStringAsync();
                });
                t2.Wait();

                model = JsonConvert.DeserializeObject(strResponse);

                // Everything ok, return success.
                ret = true;

                // If the call went through, the user is fine.

                /*
                    {{
                      "userId": "cc20a6fb-a91f-4192-874d-132493685376",
                      "userName": "doreen.riddle",
                      "fullName": "Doreen Riddle"
                    }}                 
                 */
            }
            else
            {
                log.LogInformation("verify product call returned non 200 status = " + res.StatusCode);
                ret = false;
            }

            log.LogInformation("<-- verifyProduct() returning: "+ret);

            return ret;
        }

        public static bool validateRating(string s, ILogger log)
        {
            log.LogInformation("--> validateRating() starting: " + s);

            try
            {
                int val = Int32.Parse(s);
                if(val<0 || val>5) {
                    log.LogInformation("validateRating(): value is not between 0-5. returning false.");
                    return false;
                }
            }
            catch (Exception e) {
                log.LogInformation("validateRating(): not an integer. returning false.");
                return false;
            }

            log.LogInformation("<-- validateRating() returning: true");
            return true;
        }

    }
}
