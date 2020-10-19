using AirtableApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirtableClientWrapper
{
    public class InventoryComponent
    {
        private readonly string nameFieldName = "Name";
        private readonly string minimumStockFieldName = "Minimum Stock";
        private readonly string batchSizeFieldName = "Batch Size";
        private readonly string internalPriceFieldName = "Internal Price";
        private readonly string pendingFieldName = "Pending";
        private readonly string quantityFieldName = "Quantity";
        private readonly string warehouseQuantityFieldName = "Warehouse Quantity";
        private readonly string detailsFieldName = "Details";
        private readonly string externalFieldName = "External";
        private readonly string numberOfBatchesFieldName = "Number Of Batches";

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
                return -1;
            }
            set
            {
                updateRecord(quantityFieldName, value);
            }
        }
        public int WarehouseQuantity
        {
            get
            {
                if (Record.Fields.ContainsKey(warehouseQuantityFieldName))
                {
                    return int.Parse(Record.Fields[warehouseQuantityFieldName]?.ToString());
                }
                return -1;
            }
            set
            {
                updateRecord(warehouseQuantityFieldName, value);
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

        public double InternalPrice
        {
            get
            {
                if (Record.Fields.ContainsKey(internalPriceFieldName))
                {
                    return double.Parse(Record.Fields[internalPriceFieldName]?.ToString());
                }
                return 0;
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

        public int NumberOfBatches
        {
            get
            {
                if (Record.Fields.ContainsKey(numberOfBatchesFieldName))
                {
                    return Math.Max(1, int.Parse(Record.Fields[numberOfBatchesFieldName]?.ToString()));
                }
                return 1;
            }
        }
        
        public int Pending
        {
            get
            {
                if (Record.Fields.ContainsKey(pendingFieldName))
                {
                    return int.Parse(Record.Fields[pendingFieldName]?.ToString());
                }
                return 0;
            }
            set
            {
                updateRecord(pendingFieldName, value);
            }
        }
        public string id { get
            {
                return Record.Id;
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

        public string Details
        {
            get
            {
                if (Record.Fields.ContainsKey(detailsFieldName))
                {
                    return Record.Fields[detailsFieldName].ToString();
                }
                return string.Empty;
            }
        }

        public bool IsExternal
        {
            get
            {
                if (Record.Fields.ContainsKey(externalFieldName))
                {
                    return bool.Parse(Record.Fields[externalFieldName].ToString());
                }
                return false;
            }
        }

        public AirtableRecord Record { get; }

        public InventoryComponent(AirtableRecord record)
        {
            Record = record;
        }

        public bool IsBelowThreshhold()
        {
            return (MinimumStock > (Quantity + Pending));
        }



        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> orderDictionary = new Dictionary<string, object>();
            return orderDictionary;

        }
    }
}