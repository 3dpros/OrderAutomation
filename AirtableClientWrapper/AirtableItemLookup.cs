
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;
using Newtonsoft.Json.Linq;

namespace AirtableClientWrapper
{
    public class TransactionRecord
    {
        public ItemData Record { set; get; }
        public int Quantity { set; get; }
    }

    public class AirtableItemLookup : AirtableBaseTable
    {
        private readonly string TableName = "Item Lookup";
        private readonly string nameKey = "Name";
        private readonly string logFieldName = "Log";
        private readonly string skuKey = "SKU";
        private readonly string componentsFieldName = "Components";





        public AirtableItemLookup() : base()
        {
        }

        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _invAirtableBase.RetrieveRecord(TableName, recordID);
            var response = task.Result;

            return response.Record.Fields[nameKey].ToString();

        }

        private List<AirtableRecord> GetMaterialsLookup()
        {
            var recordsList = new List<AirtableRecord>();
            
                Task<AirtableListRecordsResponse> task = _invAirtableBase.ListRecords(TableName);
                var response = task.Result;

                foreach (var record in response.Records)
                {
                    if (record.Fields.ContainsKey(nameKey))
                    recordsList.Add(record);
                }
            
            return recordsList;
        }


        //Transaction is a purchase of one or more of a single product, orders can contain one or more transactions
        public bool UpdateInventoryCountForTransaction(ItemData product, int quantityOrdered, out List<ItemComponentData> components, string orderID = "")
        {
            components = new List<ItemComponentData>();
            if (product == null)
            {
                return false;
            }
            var productRecord = product.Record;
            if (productRecord != null)
            {

                if (productRecord.Fields.ContainsKey(componentsFieldName))
                {
                    foreach (var component in (JArray)(productRecord.Fields[componentsFieldName]))
                    {

                        var cTask = _invAirtableBase.RetrieveRecord(componentsFieldName, component.ToString());
                        var componentRecord = new ItemComponentData(cTask.Result.Record);

                        var fields = new Fields();
                        fields.FieldsCollection["Quantity"] = componentRecord.Quantity - quantityOrdered;
                        UpdateInventoryRecord(componentsFieldName, componentRecord.Record.Id, fields);
                        LogInventoryEntry(productRecord.Fields[nameKey].ToString(), component.ToString(), -quantityOrdered, orderID);                        

                        //re-read the record so the component is updated based on the new quantities.  eventually should make this cleaner
                        cTask = _invAirtableBase.RetrieveRecord(componentsFieldName, component.ToString());
                        componentRecord = new ItemComponentData(cTask.Result.Record);
                        components.Add(componentRecord);
                    }
                    return true;
                }
            }
            return false;

        }

        public bool UpdateComponentForInventoryRequest(ItemComponentData componentRecord)
        {
            var fields = new Fields();
            var numberToRequest = componentRecord.BatchSize * componentRecord.NumberOfBatches;
            fields.FieldsCollection["Pending"] = componentRecord.Pending + numberToRequest;
            LogInventoryEntry("Inventory Request created for " + componentRecord.Name, componentRecord.Record.Id, numberToRequest);
            return UpdateInventoryRecord(componentsFieldName, componentRecord.Record.Id, fields);
        }

        public bool UpdateComponentQuantityByName(string componentName, int quantityChange, int originalRequestQuantity)
        {
            string offset = "";
            string query = "{" + nameKey + "} = '" + componentName + "'";
            
            Task<AirtableListRecordsResponse> task = _invAirtableBase.ListRecords(componentsFieldName, offset, null, query);
            var record = task.Result.Records.FirstOrDefault();
            if (record != null)
            {
                var fields = new Fields();
                fields.FieldsCollection["Quantity"] = int.Parse(record.Fields["Quantity"].ToString()) + quantityChange;
                fields.FieldsCollection["Pending"] = int.Parse(record.Fields["Pending"].ToString()) - originalRequestQuantity;
                UpdateInventoryRecord(componentsFieldName, record.Id, fields);
                LogInventoryEntry("Inventory Request completed for " + componentName, record.Id, quantityChange);
                return true;
            }
            return false;
        }

        private void UpdateInventoryFromComponentData(ItemComponentData component)
        {
            var fields = new Fields();
           // fields = component.Quantity
        }
        private bool UpdateInventoryRecord(string tableName, string id, Fields fieldsToUpdate)
        {

            var task = _invAirtableBase.UpdateRecord(tableName, fieldsToUpdate, id);
            var response = task.Result;

            return response.Success;
        }

        public bool UpdateCompletedOrderComponentEntries(string orderID)
        {
            string offset = "";
            string query = "{OrderID} = '" + orderID + "'";
            //mark component entries for the completed order as shipped
            Task<AirtableListRecordsResponse> task = _invAirtableBase.ListRecords(logFieldName, offset, null, query);
            foreach(var logRecord in task.Result.Records)
            {
                var logFields = new Fields();
                logFields.AddField("Shipped", true);
                var updateTask = _invAirtableBase.UpdateRecord(logFieldName, logFields, logRecord.Id);
                var response = updateTask.Result;
            }
            return true;
        }

        private void LogInventoryEntry(string entryName, string componentID, int quantityChange, string orderID = "")
        {
            var logFields = new Fields();
            logFields.AddField("Name", entryName);
            logFields.AddField("Items", new JArray { componentID });
            logFields.AddField("Quantity Change", quantityChange);
            logFields.AddField("OrderID", orderID);
            var task = _invAirtableBase.CreateRecord(logFieldName, logFields);
            var response = task.Result;
        }

        public ItemData FindItemRecord(string searchString, string color = null, int size = 0)
        {
            var dict = GetMaterialsLookup();

            foreach (var record in dict)
            {
                if (searchString.ToLowerInvariant().Contains(record.Fields[nameKey].ToString().ToLowerInvariant()))
                {
                    while (true)
                    {
                        //if the matching field requires a color and the item has a color, see if they match
                        if (!string.IsNullOrEmpty(color) && record.Fields.ContainsKey("Color"))
                        {
                            var recordColor = record.Fields["Color"];
                            if (recordColor != null)
                            {
                                if (color.ToLowerInvariant() != recordColor.ToString().ToLowerInvariant())
                                {
                                    break;
                                }
                            }
                        }
                        //if the matching field specifies a color but the item does not have one, break
                        else if (string.IsNullOrEmpty(color) && record.Fields.ContainsKey("Color"))
                        {
                            break;
                        }
                        //if the matching field does not specify a color, continue


                        if (size != 0 && record.Fields.ContainsKey("Size"))
                        {
                            var recordSize = record.Fields["Size"];
                            if (recordSize != null)
                            {
                                if (size != int.Parse(recordSize.ToString()))
                                {
                                    break;
                                }
                            }
                        }
                        else if (size == 0 && record.Fields.ContainsKey("Size"))
                        {
                            break;
                        }
                        return new ItemData(record);
                    }
                }
            }
            return null;
        }
        public ItemData FindItemRecordBySKU(string SKU)
        {
            var dict = GetMaterialsLookup();
            if (!string.IsNullOrEmpty(SKU))
            {

                foreach (var record in dict)
                {
                    if (record.Fields.ContainsKey(skuKey))
                    {
                        if (SKU.ToLowerInvariant().Equals(record.Fields[skuKey].ToString().ToLowerInvariant()))
                        {
                            return new ItemData(record);
                        }
                    }
                }
            }
            return null;
        }

    }

}
