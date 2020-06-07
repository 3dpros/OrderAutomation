
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;
using Newtonsoft.Json.Linq;

namespace AirtableClientWrapper
{
    public class AirtableOrderTracking : AirtableBaseTable
    {
        private readonly string TableName = "Order Tracking";
        private readonly string nameKey = "Name";
        private readonly string orderIDKey = "order ID";

        private AirtablePrinters _namesTable = new AirtablePrinters();
        private AirtableItemLookup _itemsTable = new AirtableItemLookup();


        public AirtableOrderTracking() : base()
        {
        }
        public OrderTrackingData GetRecordByOrderID(string OrderID, out string recordID)
        {
            return GetRecordByField(orderIDKey, OrderID, out recordID);
        }

        public OrderTrackingData GetRecordByField(string fieldName, string fieldValue, out string recordID)
        {
            recordID = "";
            string offset = "";
            string query = "{" + fieldName + "} = '" + fieldValue + "'";

            Task<AirtableListRecordsResponse> task = _invAirtableBase.ListRecords(TableName, offset, null, query);

            var response = task.Result;
            if (response.Records.Any())
            {
                var record = response.Records.FirstOrDefault();
                recordID = record.Id;
                return new OrderTrackingData(record.Fields, _namesTable.GetNamesLookup(), _itemsTable.GetItemsLookup());
            }
            return null;
        }
        public bool CreateOrderRecord(OrderData order, bool updateIfPresent = false)
        {
            return CreateOrderRecord(OrderDataToOrderTrackingData(order), updateIfPresent);
        }


        public bool CreateOrderRecord(OrderTrackingData order, bool updateIfPresent = false)
        {
            //AirtableRecord record = new AirtableRecord();
            Fields fields = new Fields();
            fields.FieldsCollection = order.ToDictionary();
            string orderID = "";
            var existingRecord = GetRecordByOrderID(order.OrderID.ToString(), out orderID);

            //order is present
            if (existingRecord != null)
            {
                //do not replace if present
                if (!updateIfPresent)
                {
                    return false;
                }
                else
                {
                    var task = _invAirtableBase.UpdateRecord(TableName, fields, orderID);
                    var response = task.Result;
                    return task.Result.Success;

                }
            }
            else
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
        public bool UpdateOrderRecord(OrderTrackingData order)
        {
            Fields fields = new Fields();
            fields.FieldsCollection = order.ToDictionary();
            var existingRecord = GetRecordByOrderID(order.OrderID.ToString(), out string recordID);
            var task = _invAirtableBase.UpdateRecord(TableName, fields, recordID);
            var response = task.Result;
            return task.Result.Success;
        }

        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _invAirtableBase.RetrieveRecord(TableName, recordID);
            var response = task.Result;

            return response.Record.Fields[nameKey].ToString();

        }


        public bool DeleteOrderRecord(OrderTrackingData order)
        {
            var existingRecord = GetRecordByOrderID(order.OrderID.ToString(), out string recordID);
            var task = _invAirtableBase.DeleteRecord(TableName, recordID);
            var response = task.Result;
            return task.Result.Success;
        }

        public OrderTrackingData NewOrderTrackingData(string orderID)
        {
            OrderTrackingData a = new OrderTrackingData(orderID, _namesTable.GetNamesLookup(), _itemsTable.GetItemsLookup());
            if (a is null)
            { throw new ArgumentNullException(); }
            return a;
        }

        public OrderTrackingData OrderDataToOrderTrackingData(OrderData orderData)
        {
            OrderTrackingData orderTrackingData = new OrderTrackingData(orderData.OrderID, _namesTable.GetNamesLookup(), _itemsTable.GetItemsLookup());
            orderTrackingData.Description = orderData.Description;
            orderTrackingData.Notes = orderData.Notes;
            orderTrackingData.PrintOperator = orderData.PrintOperator;
            orderTrackingData.DueDate = orderData.DueDate;
            orderTrackingData.Shipper = orderData.Shipper;
            orderTrackingData.ShipDate = orderData.ShipDate;
            orderTrackingData.Priority = orderData.Rush;
            orderTrackingData.OrderURL = orderData.OrderURL;
            orderTrackingData.OrderValue = orderData.TotalPrice - orderData.ShippingCharge - orderData.MaterialCost;


            if (orderTrackingData is null)
            { throw new ArgumentNullException(); }
            return orderTrackingData;
        }

    }

}
