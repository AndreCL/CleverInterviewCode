using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using CleverInterviewCode.Entity;
using System.Threading.Tasks;
using System.IO;
using System;

public static class CreateRfid
{
	[FunctionName("CreateRfid")]
	public static async Task<IActionResult> Run(
		[HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
		ILogger log)
	{
		log.LogInformation("Processing a request to create a new RFID.");

		string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
		dynamic data = JsonConvert.DeserializeObject(requestBody);
		string rfidTag = data?.rfidTag;

		if (string.IsNullOrEmpty(rfidTag))
		{
			return new BadRequestObjectResult("Please pass a valid RFID tag in the request body.");
		}

		return await SaveToTableStorage(rfidTag);
	}

	private static async Task<IActionResult> SaveToTableStorage(string rfidTag)
	{
		CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
		CloudTableClient tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());
		CloudTable table = tableClient.GetTableReference("RFIDTable");

		await table.CreateIfNotExistsAsync();

		RfidEntity newRfid = new RfidEntity("RFID", rfidTag)
		{
			Rfid = rfidTag
		};

		TableOperation insertOperation = TableOperation.Insert(newRfid);
		await table.ExecuteAsync(insertOperation);

		return new OkObjectResult($"RFID tag {rfidTag} has been created.");
	}

}