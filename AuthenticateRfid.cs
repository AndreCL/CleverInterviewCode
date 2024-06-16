using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using CleverInterviewCode.Entity;
using System.Threading.Tasks;
using System;

public static class AuthenticateRfid
{
	[FunctionName("AuthenticateRfid")]
	public static async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
		ILogger log)
	{
		log.LogInformation("Processing a request to authenticate an RFID.");

		string rfidTag = req.Query["rfidTag"];

		if (string.IsNullOrEmpty(rfidTag))
		{
			return new BadRequestObjectResult("Please pass a valid RFID tag in the query string.");
		}

		CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
		CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
		CloudTable table = tableClient.GetTableReference("RFIDTable");

		TableOperation retrieveOperation = TableOperation.Retrieve<RfidEntity>("RFID", rfidTag);
		TableResult result = await table.ExecuteAsync(retrieveOperation);
		RfidEntity rfid = result.Result as RfidEntity;

		bool isAuthenticated = rfid != null;

		return new OkObjectResult(isAuthenticated);
	}
}