
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;


namespace AirtableClientWrapper
{
    public class AirtableOrders : AirtableBaseTable
    {
        private readonly string TableName = "Orders";
        private readonly string orderIDKey = "order ID";
        private AirtablePayment _paymentTable;
        private AirtableChannels _channelTable;


        public AirtableOrders() : base()
        {
            _paymentTable = new AirtablePayment();
            _channelTable = new AirtableChannels();
        }


        public OrderData GetRecordByOrderID(string OrderID, out string recordID)
        {
            return GetRecordByField(orderIDKey, OrderID, out recordID);
        }

        public OrderData GetRecordByField(string fieldName, string fieldValue, out string recordID)
        {
            recordID = "";
            string offset = "";
            string query = "{" + fieldName + "} = '" + fieldValue + "'";

            Task<AirtableListRecordsResponse> task = _mainAirtableBase.ListRecords(TableName, offset, null, query);
            

            var response = task.Result;
            if (response.Records.Any())
            {
                var record = response.Records.FirstOrDefault();
                recordID = record.Id;
                return new OrderData(record.Fields, _paymentTable.GetNamesLookup(), _channelTable.GetNamesLookup());
            }
            return null;
        }
        
        public List<OrderData> GetAllRecordsInView(string viewName, List<string> nameSearchStrings)
        {
            Task<AirtableListRecordsResponse> task = _mainAirtableBase.ListRecords(TableName, view: viewName);

            var response = task.Result;
            var orders = new List<OrderData>();
            foreach(var record in response.Records)
            {
                var orderData = new OrderData(record.Fields, _paymentTable.GetNamesLookup(), _channelTable.GetNamesLookup());
                bool match = false;
                foreach (string searchString in nameSearchStrings)
                {
                    if (orderData.Description.ToLowerInvariant().Contains(searchString.ToLowerInvariant()))
                    {
                        match = true;
                        break;

                    }
                }
                if(match)
                orders.Add(orderData);
            }
            return orders;
        }


        public bool CreateOrderRecord(OrderData order, bool updateIfPresent = false)
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
                    var task = _mainAirtableBase.UpdateRecord(TableName, fields, orderID);
                }
            }
            else
            {

                var task = _mainAirtableBase.CreateRecord(TableName, fields);
                var response = task.Result;

                return task.Result.Success;
            }
            return false;

        }
        public bool UpdateOrderRecord(OrderData order)
        {
            Fields fields = new Fields();
            fields.FieldsCollection = order.ToDictionary();
            var existingRecord = GetRecordByOrderID(order.OrderID.ToString(), out string orderID);
            var task = _mainAirtableBase.UpdateRecord(TableName, fields, orderID);
            var response = task.Result;
            return task.Result.Success;
        }



        public OrderData newOrderData(string orderID)
        {
            OrderData a = new OrderData(orderID, _paymentTable.GetNamesLookup(), _channelTable.GetNamesLookup());
            if(a is null)
            { throw new ArgumentNullException(); }
            return a;
        }

        /*
        private OrderData GetRecordByOrderIDOld(int OrderID)
        {
            string offset = "";
            string query = "{" + orderIDKey + "} = '" + OrderID.ToString() + "'";
            //   do
            //   {

            Task<AirtableListRecordsResponse> task = _airtableBase.ListRecords(TableName, offset);
            var response = task.Result;
            if (response.Success)
            {
                offset = response.Offset;
            }
                 var recordQuery =
                      from record in response.Records
                      where record.Fields[orderIDKey].ToString() == OrderID.ToString()
                     select record;

                 foreach(var record in recordQuery)
                {
            var record = response.Records.FirstOrDefault();
            return new OrderData(record.Fields);
                }

            
            foreach (var record in response.Records.Where(a => a.Fields.ContainsKey(orderIDKey)))
            {
                string itemOrderID = record.Fields[orderIDKey]?.ToString();
                if (itemOrderID == OrderID)//"1448111924")
                {
                    return record;
                }
            }
            

              } while (offset != null);

            return null;
        }
        */
    }

}
   