using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

public static class OrderItemsReserver
{
    [FunctionName("OrderItemsReserver")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
        ILogger log)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        
        var blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        var containerClient = blobServiceClient.GetBlobContainerClient("order-requests");
        await containerClient.CreateIfNotExistsAsync();

        var blobName = $"order-{Guid.NewGuid()}.json";
        var blobClient = containerClient.GetBlobClient(blobName);
        
        using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(requestBody)))
        {
            await blobClient.UploadAsync(stream);
        }

        return new OkResult();
    }
}
