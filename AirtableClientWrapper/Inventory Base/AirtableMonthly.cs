
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;


namespace AirtableClientWrapper
{
    public class AirtableMonthly : AirtableBaseTable
    {
        private readonly string TableName = "Monthly";
        private readonly string nameKey = "Month";

        public AirtableMonthly() : base()
        {
        }

        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _mainAirtableBase.RetrieveRecord(TableName, recordID);
            var response = task.Result;

            return response.Record.Fields[nameKey].ToString();

        }
        public string GetLatestMonthlyID()
        {
            return GetMonthlyID(DateTime.Now);
        }
        public string GetMonthlyID(DateTime date)
        {
            var currentMonth = date.ToString("MM/yyyy");
            Fields fields = new Fields();
            fields.AddField(nameKey, currentMonth);
            var dict = GetMonthlyEntries();
            if (dict.ContainsKey(currentMonth))
            {
                return dict[currentMonth];
            }
            else
            {
                Task<AirtableCreateUpdateReplaceRecordResponse> task = _mainAirtableBase.CreateRecord(TableName, fields);
                var response = task.Result;
                return task.Result.Record.Id;
            }
        }


        public Dictionary<string, string> GetMonthlyEntries()
        {
            Task<AirtableListRecordsResponse> task = _mainAirtableBase.ListRecords(TableName);
            var response = task.Result;
            var dict = new Dictionary<string, string>();

            foreach (var record in response.Records)
            {
                if (record.Fields.ContainsKey(nameKey))
                    dict.Add(record.Fields[nameKey].ToString(), record.Id);
            }
            return dict;
        }


    }

}
