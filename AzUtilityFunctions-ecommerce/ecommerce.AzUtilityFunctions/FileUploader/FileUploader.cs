using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.FileUploader
{
    public static class FileUploader
    {
        [FunctionName("FileUploader")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Queue("files-submit", Connection = "AzureWebJobsStorage")] CloudQueue fileSubmitQueue,
            [Blob("files-submit", FileAccess.Write, Connection = "AzureWebJobsStorage")] CloudBlobContainer filesSubmitContainer,
            ILogger log)
        {
            log.LogInformation("HTTP trigger function processed a request File upload.");
            FileUploaderResponse fileUploaderResponse = new FileUploaderResponse();
            if (req == null)
            {
                log.LogError("no File to upload.");
                fileUploaderResponse.flag = false;
                fileUploaderResponse.message = "You have sent empty content.";
                return (ActionResult)new OkObjectResult(fileUploaderResponse);
            }

            try
            {
                var fileName = $"{Guid.NewGuid().ToString()}.json";
                await filesSubmitContainer.CreateIfNotExistsAsync();
                CloudBlockBlob blob = filesSubmitContainer.GetBlockBlobReference(fileName);

                FileUploaderRequest fileUploaderRequest = await JsonSerializer.DeserializeAsync<FileUploaderRequest>(req.Body);

                //Send to te container 
                await blob.UploadTextAsync(JsonSerializer.Serialize(fileUploaderRequest));

                //Sent ot Queue
                var queueMessage = new CloudQueueMessage(fileName);
                await fileSubmitQueue.AddMessageAsync(queueMessage);
                fileUploaderResponse.flag = false;
                fileUploaderResponse.message = "Has been added to Queue";
                log.LogInformation($"File name : {fileName} , {fileUploaderResponse.message}");
            }
            catch (Exception ex)
            {
                log.LogError($"error msg : {ex.ToString()}");
                fileUploaderResponse.flag = false;
                fileUploaderResponse.message = ex.ToString();
                return new BadRequestObjectResult(fileUploaderResponse);
            }

            return (ActionResult)new OkObjectResult(fileUploaderResponse);
        }
    }
}
