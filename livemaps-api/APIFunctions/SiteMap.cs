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

            string rebuild = req.Query["rebuild"];
            bool full = rebuild == "full";

            var siteMapFile = Environment.GetEnvironmentVariable("SiteMapFile");
            if (string.IsNullOrEmpty(siteMapFile))
            {
                prerequisites = false;
                errors.Append("DataBase Connection String property {siteMapFile} is not defined!");
            }

            var atlasConfigFile = Environment.GetEnvironmentVariable("AtlasConfigFile") ?? "atlasConfig.json";
            var bds = new BlobDataService(); 
            
            if (prerequisites)
            {
                await container.CreateIfNotExistsAsync();
                var siteMapRef = container.GetBlockBlobReference(siteMapFile);                

                try
                {
                    if (!string.IsNullOrEmpty(rebuild))  
                        if (prerequisites)
                        {
                           // Custom Sitemap builder code                       
                    }
                    else
                    {
                        if (prerequisites)
                        {                            
                            using (var ms = new MemoryStream())
                            {
                                await siteMapRef.DownloadToStreamAsync(ms);
                                ms.Position = 0;
                                using (StreamReader reader = new StreamReader(ms, Encoding.UTF8))
                                {
                                    var bacmapstr = reader.ReadToEnd();
                                    return new OkObjectResult(bacmapstr);                                    
                                }
                            }
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
