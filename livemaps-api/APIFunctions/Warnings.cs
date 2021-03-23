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
    public static class Warnings
    {
        [FunctionName("Warnings")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "faults/{region}/{campus}/{building}")] HttpRequest req,
            [Blob("appdata", Connection = "AzureWebJobsStorage")] CloudBlobContainer container,
            string region,
            string campus,
            string building,            
            ILogger log)
        {
            string fileName;
            try
            {
                if (!string.IsNullOrEmpty(building))
                {
                    fileName = $"{region}_{campus}_{building}_warnings.json".ToLower();
                }
                else
                {
                    return new NotFoundObjectResult("Data not found!");
                }

                var blobDataService = new BlobDataService();
                var warningsData = await blobDataService.ReadBlobData(container, fileName);
                return new OkObjectResult(warningsData);
            }
            catch(Exception ex)
            {
                log.LogError(ex.Message);
                return new NotFoundObjectResult("Data not found!");
            }
        }
    }
}
