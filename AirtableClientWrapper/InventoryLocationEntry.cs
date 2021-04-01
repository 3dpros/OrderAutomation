using AirtableApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirtableClientWrapper
{
    public class InventoryLocationEntry
    {
        private readonly string nameFieldName = "ID";
        private readonly string componentIdFieldName = "Component";
        private readonly string locationFieldName = "Location";

        private readonly string minimumStockFieldName = "Minimum Stock";
        private readonly string batchSizeFieldName = "Batch Size";
        private readonly string calculatedPendingFieldName = "Calculated Pending";
        private readonly string quantityFieldName = "Quantity At Location";
        private readonly string quantityInOpenOrdersFieldName = "Open Orders";



        public Fields UpdatedFields = new Fields();
        private void updateRecord(string fieldName, object value)
        {
            Record.Fields[fieldName] = value;
            UpdatedFields.FieldsCollection[fieldName] = value;
        }

        public int Quantity
        {
            get
            {
                if (Record.Fields.ContainsKey(quantityFieldName))
                {
                    return int.Parse(Record.Fields[quantityFieldName]?.ToString());
                }
                return 0;
            }
            set
            {
                updateRecord(quantityFieldName, value);
            }
        }
        public int QuantityInOpenOrders
        {
            get
            {
                if (Record.Fields.ContainsKey(quantityInOpenOrdersFieldName))
                {
                    return int.Parse(Record.Fields[quantityInOpenOrdersFieldName]?.ToString());
                }
                return -1;
            }
        }

        public int MinimumStock
        {
            get
            {
                if (Record.Fields.ContainsKey(minimumStockFieldName))
                {
                    return int.Parse(Record.Fields[minimumStockFieldName]?.ToString());
                }
                return -1;
            }
        }

        public int BatchSize
        {
            get
            {
                if (Record.Fields.ContainsKey(batchSizeFieldName))
                {
                    return int.Parse(Record.Fields[batchSizeFieldName]?.ToString());
                }
                return 1;
            }
        }
        
        public int CalculatedPending
        {
            get
            {
                if (Record.Fields.ContainsKey(calculatedPendingFieldName))
                {
                    return int.Parse(Record.Fields[calculatedPendingFieldName]?.ToString());
                }
                return 0;
            }
        }
        public string id { get
            {
                return Record.Id;
             }
        }

        public string ComponentId
        {
            get
            {
                if (Record.Fields.ContainsKey(componentIdFieldName))
                {
                    return Record.Fields[componentIdFieldName]?.ToString();
                }
                return "";
            }
        }

        public string Name
        {
            get
            {
                if (Record.Fields.ContainsKey(nameFieldName))
                {
                    return Record.Fields[nameFieldName].ToString();
                }
                return string.Empty;
            }
        }

        public AirtableRecord Record { get; }

        public InventoryLocationEntry(AirtableRecord record)
        {
            Record = record;
        }

        public bool IsBelowThreshhold()
        {
            return (MinimumStock > (Quantity + CalculatedPending));
        }

        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> orderDictionary = new Dictionary<string, object>();
            orderDictionary.AddIfNotNull(quantityFieldName, Quantity);
            return orderDictionary;
        }
    }
}