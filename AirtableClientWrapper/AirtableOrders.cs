
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


        public OrderData GetRecordByOrderID(int OrderID)
        {
            string offset = "";
            string query = "{" + orderIDKey + "} = '" + OrderID.ToString() + "'";

            Task<AirtableListRecordsResponse> task = _airtableBase.ListRecords(TableName, offset, null, query);

            var response = task.Result;
            if (response.Records.Any())
            {
                var record = response.Records.FirstOrDefault();
                return new OrderData(record.Fields, _paymentTable.GetNamesLookup(), _channelTable.GetNamesLookup());
            }
            return null;
        }
        

        public void CreateOrderRecord(OrderData order, bool replaceIfIdPresent = false)
        {
            //AirtableRecord record = new AirtableRecord();
            Fields fields = new Fields();
            fields.FieldsCollection = order.ToDictionary();
            bool doUpdate = true;

            if (!replaceIfIdPresent)
            {
                if (GetRecordByOrderID(order.OrderID) != null)
                {
                    doUpdate = false;
                }
            }
            if (doUpdate)
            {
                Task<AirtableCreateUpdateReplaceRecordResponse> task = _airtableBase.CreateRecord(TableName, fields);
                var response = task.Result;
            }


        }

        public OrderData newOrderData(int orderID)
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
   