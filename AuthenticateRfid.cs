using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

public static class AuthenticateRfid
{
	[FunctionName("AuthenticateRfid")]
	[OpenApiOperation(operationId: "authenticateRFID", tags: new[] { "RFID" })]
	[OpenApiParameter(name: "RFID", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The RFID tag to authenticate")]
	[OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(bool), Description = "The authentication result")]
	public static async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
		ILogger log)
	{
		log.LogInformation("Processing a request to authenticate an RFID.");

		string rfidTag = req.Query["RFID"];

		if (string.IsNullOrEmpty(rfidTag))
		{
			return new BadRequestObjectResult("Please pass a valid RFID tag in the query string.");
		}

		CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
		CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
		CloudTable table = tableClient.GetTableReference("RFIDTable");

		TableOperation retrieveOperation = TableOperation.Retrieve<TableEntity>("Rfid", rfidTag);
		TableResult result = await table.ExecuteAsync(retrieveOperation);
		TableEntity rfid = result.Result as TableEntity;

		bool isAuthenticated = rfid != null;

		return new OkObjectResult(isAuthenticated);
	}
}