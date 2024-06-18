using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using CleverInterviewCode.StorageService;
using Microsoft.AspNetCore.Components;

public static class CreateRfid
{
    [FunctionName("CreateRfid")]
	[OpenApiOperation(operationId: "createRFID", tags: new[] { "RFID" })]
	[OpenApiRequestBody("application/json", typeof(string), Description = "JSON request body containing the RFID tag")]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
	public static async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
		ILogger log
        )
	{
		log.LogInformation("Processing a request to create a new RFID.");

		string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

		if (string.IsNullOrEmpty(requestBody))
		{
			return new BadRequestObjectResult("Please pass a valid RFID tag in the request body.");
		}

		return await SaveToTableStorage(requestBody);
	}

	private static async Task<IActionResult> SaveToTableStorage(string rfidTag)
	{
		CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
		CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
		CloudTable table = tableClient.GetTableReference("RFIDTable");

		await table.CreateIfNotExistsAsync();

		var newRfid = new TableEntity("Rfid", rfidTag);

		TableOperation insertOperation = TableOperation.Insert(newRfid);
		await table.ExecuteAsync(insertOperation);

		return new OkObjectResult($"RFID tag {rfidTag} has been created.");
	}

}