using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CleverInterviewCode.StorageService
{
    public class RfidStorageService : IRfidStorageService
    {
        public async Task<bool> AuthenticateRfidTagAsync(string rfidTag)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("RFIDTable");

            TableOperation retrieveOperation = TableOperation.Retrieve<TableEntity>("Rfid", rfidTag);
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            TableEntity rfid = result.Result as TableEntity;

            bool isAuthenticated = rfid != null;
            return isAuthenticated;
        }

        public async Task<IActionResult> SaveRfidTagAsync(string rfidTag)
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
}
