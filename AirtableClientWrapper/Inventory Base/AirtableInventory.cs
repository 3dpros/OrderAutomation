
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;
using Newtonsoft.Json.Linq;

namespace AirtableClientWrapper
{
    public class AirtableInventory : AirtableBaseTable
    {
        private readonly string TableName = "Multisite Inventory";
        private readonly string componentKey = "Component";

        private AirtablePrinters _namesTable = new AirtablePrinters();
        private AirtableItemLookup _itemsTable = new AirtableItemLookup();


        public AirtableInventory() : base()
        {
        }
        /*
        public bool CreateOrderRecord(TransactionData transaction, bool updateIfPresent = false)
        {
            Fields fields = new Fields();
            fields.FieldsCollection = transaction.ToDictionary();

            {

                var task = _invAirtableBase.CreateRecord(TableName, fields);
                var response = task.Result;
                if(!task.Result.Success)
                {
                    throw new Exception(task.Result.AirtableApiError.ErrorMessage);
                }
                return task.Result.Success;
            }
            
        }
        */
        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _invAirtableBase.RetrieveRecord(TableName, recordID);
            var response = task.Result;

            return response.Record.Fields[componentKey].ToString();

        }

        public InventoryLocationEntry FindRecord(string componentID, string locationName)
        {
            string offset = "";
            string query = $"AND({{Component ID}} = '{componentID}', SEARCH('{locationName}', ID))";
            //mark component entries for the completed order as shipped
            Task<AirtableListRecordsResponse> task = _invAirtableBase.ListRecords(TableName, offset, null, query);
            return new InventoryLocationEntry(task.Result.Records.FirstOrDefault());
        }

        public InventoryLocationEntry FindRecordByName(string componentName, string locationName)
        {
            string offset = "";
            string query = $"AND({{Item Name}} = '{componentName}', SEARCH('{locationName}', ID))";
            //mark component entries for the completed order as shipped
            Task<AirtableListRecordsResponse> task = _invAirtableBase.ListRecords(TableName, offset, null, query);
            return new InventoryLocationEntry(task.Result.Records.FirstOrDefault());
        }

        public void IncrementQuantityOfItem(string componentID, string locationName, int numberToAdd = 1)
        {
            var invItem = FindRecord(componentID, locationName);
            if (invItem.Record != null)
            {
                invItem.Quantity += numberToAdd;
                UpdateOrderRecord(invItem);
            }
        }

        public void IncrementQuantityOfItemByName(string componentName, string locationName, int numberToAdd = 1)
        {
            var invItem = FindRecordByName(componentName, locationName);
            if (invItem.Record != null)
            {
                invItem.Quantity += numberToAdd;
                UpdateOrderRecord(invItem);
            }
        }

        private bool UpdateOrderRecord(InventoryLocationEntry inventoryLocationEntry)
        {
            Fields fields = new Fields();
            fields.FieldsCollection = inventoryLocationEntry.ToDictionary();
            var task = _invAirtableBase.UpdateRecord(TableName, fields, inventoryLocationEntry.id);
            var response = task.Result;
            return task.Result.Success;
        }
        /*
        public TransactionData NewTransactionData(InventoryProduct product)
        {
            TransactionData a = new TransactionData(product);
            if (a is null)
            { throw new ArgumentNullException(); }
            return a;
        }
        */


    }

}
