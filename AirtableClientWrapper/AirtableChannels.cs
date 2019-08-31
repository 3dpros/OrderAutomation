
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;


namespace AirtableClientWrapper
{
    public class AirtableChannels : AirtableBaseTable
    {
        private readonly string TableName = "Channels";
        private readonly string nameKey = "Name";


        public AirtableChannels() : base()
        {
        }

        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _airtableBase.RetrieveRecord(TableName, recordID);
            var response = task.Result;
            if (response.Record.Fields.ContainsKey(nameKey))
            {
                return response.Record.Fields[nameKey].ToString();
            }
            return string.Empty;
        }

        public Dictionary<string, string> GetNamesLookup()
        {
            Task<AirtableListRecordsResponse> task = _airtableBase.ListRecords(TableName);
            var response = task.Result;
            var dict = new Dictionary<string, string>();

            foreach (var record in response.Records)
            {
                if (record.Fields.ContainsKey(nameKey))
                {
                    dict.Add(record.Id, record.Fields[nameKey].ToString());
                }
            }
            return dict;
        }


    }

}
