using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

using ssir.api.Services;

namespace ssir.api
{
    public static class SiteMap
    {
        [FunctionName("SiteMap")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Blob("appdata", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            bool prerequisites = true;

            var errors = new StringBuilder();
            string siteMap = "sitemap";

            bool rebuild;
            bool.TryParse(req.Query["rebuild"], out rebuild);
            
            var siteMapFile = Environment.GetEnvironmentVariable("SiteMapFile");
            if (string.IsNullOrEmpty(siteMapFile))
            {
                prerequisites = false;
                errors.Append("Property {siteMapFile} is not defined!");
            }           
            
            if (prerequisites)
            {
                var blobDataService = new BlobDataService();
                await container.CreateIfNotExistsAsync();                              

                try
                {
                    if (rebuild)
                    { 
                           // Custom Sitemap builder code                       
                    }
                    else
                    {
                        if (prerequisites)
                        {
                            var siteMapData = await blobDataService.ReadBlobData(container, siteMapFile);
                            return new OkObjectResult(siteMapData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors.Append(ex.Message);
                }
            }

            if (!prerequisites || errors.Length > 0)
            {
                log.LogError(errors.ToString());
                return new NotFoundResult();
            }

            return new OkObjectResult(siteMap);
        }
    }
}
