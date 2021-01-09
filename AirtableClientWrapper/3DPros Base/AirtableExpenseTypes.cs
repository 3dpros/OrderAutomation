
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;


namespace AirtableClientWrapper
{
    public class AirtableExpenseTypes : AirtableBaseTable
    {
        private readonly string TableName = "Expense Types";
        private readonly string nameKey = "Name";
        private readonly string keyWordsKey = "Keywords";


        public AirtableExpenseTypes() : base()
        {
        }

        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _mainAirtableBase.RetrieveRecord(TableName, recordID);
            var response = task.Result;

            return response.Record.Fields[nameKey].ToString();

        }

        public string MatchExpenseTypeRecordID(string expenseName)
        {
            Task<AirtableListRecordsResponse> task = _mainAirtableBase.ListRecords(TableName);
            var response = task.Result;
            var dict = new Dictionary<string, string>();

            foreach (var record in response.Records)
            {
                if(record.Fields.ContainsKey(keyWordsKey))
                {
                    var keywords = (record.Fields[keyWordsKey].ToString().Split(',').ToList());
                    foreach (var word in keywords)
                    {
                        if (expenseName.ToLower().Trim().Contains(word.ToLower().Trim()))
                        {
                            return record.Id;
                        }
                    }
                }
            }
            return "";
        }

        private Dictionary<string, string> GetExpenseTypeEntries()
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
