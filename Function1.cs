using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Net.Http;
using System.Runtime;

namespace debugmode56
{
    public static class Function1
    {
        static bool createThread = true;

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            var req2 = new HttpRequestMessage();

            //for (int y = 0; y < 3; y++)
            //{
                //if (y % 10 == 0)
                //{
                //    log.LogInformation("Status: " + y/10 + "%");
                //}

                for (int i = 0; i < 20; i++)
                {
                    req2.Headers.Add(i.ToString(), i.ToString());
                }

                var data = new Dictionary<string, StringValues>();

                foreach (var item in req2.Headers)
                {
                    var tempList = new List<string>();
                    
                    foreach (var dataItem in data)
                    {
                        tempList.Add(dataItem.Key);
                    }

                    foreach (var tempItem in tempList)
                    {
                        var myGuid = System.Guid.NewGuid();
                        try
                        {
                            data.Add(myGuid.ToString(), myGuid.ToString());
                        }
                        catch (Exception exc)
                        {
                            log.LogInformation(exc.Message);
                        }
                    }
                    data[item.Key] = item.Value.ToString();
                }

                foreach (var item2 in req2.Headers)
                {
                    data[item2.Key] = item2.Value.ToString();
                }

                Task.Run(() =>
                {
                    var tempData = data;
                    tempData.Add("tempKey", "tempValue");

                    if (createThread)
                    {
                        Task.Run(() =>
                        {
                            for (int x = 0; x < tempData.Count; x++)
                            {
                                var temp = tempData.ElementAt(x);
                                if (x == tempData.Count - 2)
                                {
                                    x = 0;
                                }
                                System.Threading.Thread.Sleep(500);
                            }
                        });

                        createThread = false;
                    }

                });

            //}

            // end loop
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data2 = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data2?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
