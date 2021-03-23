using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

using Newtonsoft.Json;

using ssir.api.Models;
using ssir.api.Services;

namespace ssir.api
{
    public static class Config
    {
        [FunctionName("Config")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "config/{region}/{campus}/{building}")] HttpRequest req,
            [Blob("appdata", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            string region,
            string campus,
            string building,             
            ILogger log)
        {
            bool prerequisites = true;
            var errors = new StringBuilder();

            var blobDataService = new BlobDataService();
            
            var atlasConfigFile = Environment.GetEnvironmentVariable("AtlasConfigFile") ?? "atlasConfig.json";                     

            if (string.IsNullOrEmpty(building))
            {
                prerequisites = false;
                errors.Append("Required query {building} was not defined");
            }
            
            var result = string.Empty;
            if (prerequisites)
            {
                try
                {
                    var config = await blobDataService.ReadBlobData<BuildingConfig[]>(container, atlasConfigFile);
                    var buildingCfg = config.FirstOrDefault(cfg => cfg.BuildingId.ToLower() == $"{region}/{campus}/{building}".ToLower());
                    if (buildingCfg != null)
                        result = JsonConvert.SerializeObject(buildingCfg);                  

                }
                catch (Exception ex)
                {
                    log.LogError(ex.Message);
                }                
            }
            else
            {
                log.LogError(errors.ToString());
                return new NotFoundResult();
            }

            return new OkObjectResult(result);
        }        
    }
}
