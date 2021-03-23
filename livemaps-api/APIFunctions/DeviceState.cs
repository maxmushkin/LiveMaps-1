using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

using ssir.api.Services;

namespace ssir.api
{
    public static class DeviceState
    {
        [FunctionName("DeviceState")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "state/{region}/{campus}/{building}/{level?}")] HttpRequest req,
            [Blob("appdata", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            string region,
            string campus,
            string building,
            string level,
            ILogger log)
        {
            string fileName;
            try
            {
                if (string.IsNullOrEmpty(level))
                {
                    fileName = $"{region}_{campus}_{building}_currentState.json";
                }
                else
                {
                    fileName = $"{region}_{campus}_{building}_{level}_currentState.json";
                }

                fileName = fileName.ToLower();

                var blobDataService = new BlobDataService();
                var deviceStateData = await blobDataService.ReadBlobData(container, fileName);
                return new OkObjectResult(deviceStateData);
            }
            catch(Exception ex)
            {
                log.LogError(ex.Message);
                return new NotFoundObjectResult("Data not found!");
            }
        }
    }
}
