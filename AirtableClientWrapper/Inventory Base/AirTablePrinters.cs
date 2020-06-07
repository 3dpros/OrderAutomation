
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;


namespace AirtableClientWrapper
{
    public class AirtablePrinters : AirtableBaseTable
    {
        private readonly string TableName = "Printers";
        private readonly string nameKey = "Name";

        public AirtablePrinters() : base()
        {
        }

        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _invAirtableBase.RetrieveRecord(TableName, recordID);
            var response = task.Result;

            return response.Record.Fields[nameKey].ToString();

        }

        public Dictionary<string, string> GetNamesLookup()
        {
            Task<AirtableListRecordsResponse> task = _invAirtableBase.ListRecords(TableName);
            var response = task.Result;
            var dict = new Dictionary<string, string>();

            foreach (var record in response.Records)
            {
                if (record.Fields.ContainsKey(nameKey))
                    dict.Add(record.Id, record.Fields[nameKey].ToString());
            }
            return dict;
        }


    }

}
