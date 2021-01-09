
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;
using Newtonsoft.Json.Linq;

namespace AirtableClientWrapper
{
    public class AirtableTransactions : AirtableBaseTable
    {
        private readonly string TableName = "Transactions";
        private readonly string nameKey = "Name";
        private readonly string orderIDKey = "order ID";

        private AirtablePrinters _namesTable = new AirtablePrinters();
        private AirtableItemLookup _itemsTable = new AirtableItemLookup();


        public AirtableTransactions() : base()
        {
        }

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

        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _invAirtableBase.RetrieveRecord(TableName, recordID);
            var response = task.Result;

            return response.Record.Fields[nameKey].ToString();

        }


        public TransactionData NewTransactionData(InventoryProduct product)
        {
            TransactionData a = new TransactionData(product, _itemsTable.GetProductsLookup());
            if (a is null)
            { throw new ArgumentNullException(); }
            return a;
        }

        public TransactionData GetTransactionByRecordID(string TransactionRecordID)
        {
            Task<AirtableRetrieveRecordResponse> task = _invAirtableBase.RetrieveRecord(TableName, TransactionRecordID);
            return new TransactionData(task.Result.Record.Fields, _itemsTable.GetProductsLookup());
        }

    }

}
