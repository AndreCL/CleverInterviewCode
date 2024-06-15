using Microsoft.Azure.Cosmos.Table;

namespace CleverInterviewCode.Entity
{
	public class RfidEntity : TableEntity
	{
		public RfidEntity() { }

		public RfidEntity(string partitionKey, string rowKey)
		{
			PartitionKey = partitionKey;
			RowKey = rowKey;
		}

		public string Rfid { get; set; }
	}
}
