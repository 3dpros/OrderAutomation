
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
        public InventoryProduct Record { set; get; }
        public int Quantity { set; get; }
    }

    public class AirtableItemLookup : AirtableBaseTable
    {
        private readonly string ProductsTableName = "Products Database";
        private readonly string componentsTableName = "Components";


        private readonly string nameKey = "Name";
        private readonly string logFieldName = "Log";
        private readonly string skuKey = "SKU";
        private readonly string potentialPrintersFieldName = "Potential Printers";
        private readonly string preferredPrinterFieldName = "Preferred Printer";
        private readonly string preferredShipperFieldName = "Preferred Shipper";
        private readonly string defaultOwnersTableName = "Printers";


        public AirtableItemLookup() : base()
        {
        }

        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _invAirtableBase.RetrieveRecord(ProductsTableName, recordID);
            var response = task.Result;

            return response.Record.Fields[nameKey].ToString();

        }

        public AirtableRecord RetreiveComponentsRecord(string recordID)
        {
            var cTask = _invAirtableBase.RetrieveRecord(componentsTableName, recordID);
            return cTask.Result.Record;
        }

        private List<AirtableRecord> GetMaterialsLookup()
        {
            var recordsList = new List<AirtableRecord>();
            var offset = "";
            while(offset != null)
             { 
                Task < AirtableListRecordsResponse > task = _invAirtableBase.ListRecords(ProductsTableName, offset);
                var response = task.Result;
                offset = task.Result.Offset;

                foreach (var record in response.Records)
                {
                    if (record.Fields.ContainsKey(nameKey))
                    {
                        recordsList.Add(record);
                    }
                }
            }
            return recordsList;
        }


        //Transaction is a purchase of one or more of a single product, orders can contain one or more transactions
        public bool UpdateInventoryCountForTransaction(InventoryProduct product, int quantityOrdered, out List<InventoryComponent> components, string orderID = "")
        {
            components = new List<InventoryComponent>();
            if (product == null)
            {
                return false;
            }
            components = product.getComponents(this);

            foreach (var component in components)
            {
                component.Quantity = component.Quantity - quantityOrdered;
                LogInventoryEntry(component.Name, component.Record.Id, -quantityOrdered, orderID);
                UpdateComponentRecord(component);
            }
            return true;


        }
        public bool GetPotentialPrintersList(InventoryProduct product, out List<string> printerNames, out string preferredPrinter)
        {
            return GetPotentialPrintersListCore(product.Record, out printerNames, out preferredPrinter);
        }
        public bool GetPotentialPrintersList(InventoryComponent component, out List<string> printerNames, out string preferredPrinter)
        {
            return GetPotentialPrintersListCore(component.Record, out printerNames, out preferredPrinter);
        }
        private bool GetPotentialPrintersListCore(AirtableRecord record, out List<string> printerNames, out string preferredPrinter)
        {
            
            printerNames = new List<string>();
            preferredPrinter = "";
            if (record == null)
            {
                return false;
            }
            var productRecord = record;
            if (productRecord != null)
            {

                if (productRecord.Fields.ContainsKey(potentialPrintersFieldName))
                {

                    foreach (var component in (JArray)(productRecord.Fields[potentialPrintersFieldName]))
                    {

                        var cTask = _invAirtableBase.RetrieveRecord(defaultOwnersTableName, component.ToString());
                        printerNames.Add(cTask.Result.Record.Fields["Name"].ToString());

                    }
                }
                if(productRecord.Fields.ContainsKey(preferredPrinterFieldName))
                { 
                    foreach (var component in (JArray)(productRecord.Fields[preferredPrinterFieldName]))
                    {
                        var cTask = _invAirtableBase.RetrieveRecord(defaultOwnersTableName, component.ToString());
                        preferredPrinter =cTask.Result.Record.Fields["Name"].ToString();
                        break;
                    }

                    return true;
                }
            }
            return false;
        }

        public bool GetPreferredShipper(InventoryProduct product, out string preferredShipper)
        {
            return GetPreferredShipperCore(product.Record, out preferredShipper);
        }

        private bool GetPreferredShipperCore(AirtableRecord record, out string preferredShipper)
        {
            preferredShipper = "";
            if (record.Fields.ContainsKey(preferredShipperFieldName))
            {
                foreach (var component in (JArray)(record.Fields[preferredShipperFieldName]))
                {
                    var cTask = _invAirtableBase.RetrieveRecord(defaultOwnersTableName, component.ToString());
                    preferredShipper = cTask.Result.Record.Fields["Name"].ToString();
                    break;
                }

                return true;
            }
            return false;
        }


        public bool LogInventoryRequestCreation(InventoryComponent componentRecord, int numberToRequest)
        {
            componentRecord.Pending += numberToRequest;
            LogInventoryEntry("Inventory Request created for " + componentRecord.Name, componentRecord.Record.Id, numberToRequest);
            UpdateComponentRecord(componentRecord);
            return true;
        }
        public bool UpdateComponentQuantityByName(string componentName, int quantityChange, int originalRequestQuantity)
        {
            return UpdateComponentQuantity(GetComponentByName(componentName), quantityChange, originalRequestQuantity);
        }
        public bool UpdateComponentQuantity(InventoryComponent component, int quantityChange, int originalRequestQuantity)
        {
            if (component != null)
            {
                var fields = new Fields();
                component.Quantity = component.Quantity + quantityChange;
                component.Pending = component.Pending - originalRequestQuantity;
                UpdateComponentRecord(component);
                LogInventoryEntry("Inventory Request completed for " + component.Name, component.Record.Id, quantityChange);
                return true;
            }
            return false;
        }

        public InventoryComponent GetComponentByName(string componentName, bool exactMatch = true)
        {
            string offset = "";
            string query;
            if (exactMatch)
                query = "{" + nameKey + "} = '" + componentName + "'";
            else
                query = "SEARCH(LOWER('" + componentName + "'),LOWER({" + nameKey + "})) > 0";
                
            Task<AirtableListRecordsResponse> task = _invAirtableBase.ListRecords(componentsTableName, offset, null, query);
            //if there are multiple matches in an inexact match, try an exact match instead
            if(task.Result.Records?.Count() > 1 && !exactMatch)
            {
                return this.GetComponentByName(componentName, true);
            }
            var record = task.Result.Records?.SingleOrDefault();
            if (record != null)
            {
                return new InventoryComponent(record);
            }
            return null;
        }
        public InventoryComponent GetComponentByID(string componentID)
        {
            var task = _invAirtableBase.RetrieveRecord(componentsTableName, componentID);
            return new InventoryComponent(task.Result.Record);
        }

        public void UpdateComponentRecord(InventoryComponent component)
        {
            var currentRecord = RetreiveComponentsRecord(component.Record.Id);
            var task = _invAirtableBase.UpdateRecord(componentsTableName, component.UpdatedFields, component.Record.Id);
            var response = task.Result;
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

        public InventoryProduct AddProductRecord(string name, string color = "", int sizeInInches = 0, string imageURL = "")
        {
            if (FindItemRecord(name) == null)
            {
                Fields fields = new Fields();
                fields.FieldsCollection.Add("Name", name);
                fields.FieldsCollection.Add("Color", color);

                if (sizeInInches > 0)
                {
                    fields.FieldsCollection.Add("Size", sizeInInches);
                }
                var task = _invAirtableBase.CreateRecord(ProductsTableName, fields);
                var response = task.Result;
                return new InventoryProduct(task.Result.Record);
            }
            return null;
        }
        public void AddImageToProduct(InventoryProduct product, string imageURL = "")
        {
            //Task<AirtableRetrieveRecordResponse> task = _invAirtableBase.RetrieveRecord(ProductsTableName, product.Record.Id);
            //var response = task.Result;

            Fields mainField = new Fields();
            Fields imgField = new Fields();
            imgField.AddField("url", imageURL);
            mainField.AddField("Image", imgField);


            var updateTask = _invAirtableBase.UpdateRecord(ProductsTableName, mainField, product.Record.Id);
            var updateResponse = updateTask.Result;

        }




        public InventoryProduct FindItemRecord(string searchString, string color = null, int size = 0)
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
                        return new InventoryProduct(record);
                    }
                }
            }
            return null;
        }
        public InventoryProduct FindItemRecordBySKU(string SKU)
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
                            return new InventoryProduct(record);
                        }
                    }
                }
            }
            return null;
        }

        public InventoryProduct GetItemRecordByRecordID(string productID)
        {
            var task = _invAirtableBase.RetrieveRecord(ProductsTableName, productID);
            return new InventoryProduct(task.Result.Record);
        }

        public Dictionary<string, string> GetProductsLookup()
        {
            Task<AirtableListRecordsResponse> task = _invAirtableBase.ListRecords(ProductsTableName);
            var response = task.Result;
            var dict = new Dictionary<string, string>();

            string key = "UniqueName";

            foreach (var record in response.Records)
            {
                if (record.Fields.ContainsKey(key))
                {
                    dict.Add(record.Id, record.Fields[key].ToString());
                }               
            }
            return dict;
        }

    }

}
