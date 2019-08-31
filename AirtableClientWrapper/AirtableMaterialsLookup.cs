
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;


namespace AirtableClientWrapper
{
    public class AirtableMaterialsLookup : AirtableBaseTable
    {
        private readonly string TableName = "Materials Lookup";
        private Dictionary<string, double> _dict = new Dictionary<string, double>();
        private readonly string nameKey = "Name";

        public AirtableMaterialsLookup() : base()
        {
        }

        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _airtableBase.RetrieveRecord(TableName, recordID);
            var response = task.Result;

            return response.Record.Fields[nameKey].ToString();

        }

        private Dictionary<string,double> GetMaterialsLookup()
        {
            if (!_dict.Any())
            {
                Task<AirtableListRecordsResponse> task = _airtableBase.ListRecords(TableName);
                var response = task.Result;
                _dict = new Dictionary<string, double>();

                foreach (var record in response.Records)
                {
                    if (record.Fields.ContainsKey(nameKey))
                        _dict.Add(record.Fields[nameKey].ToString(), Double.Parse(record.Fields["Material Cost"].ToString()));
                }
            }
            return _dict;
        }

        public double GetMaterialCostForItem(string itemName)
        {
            var dict = GetMaterialsLookup();
            foreach (var record in dict)
            {
                if(itemName.ToLowerInvariant().Contains(record.Key.ToLowerInvariant()))
                {
                    return record.Value;
                }
            }
            return 0;
        }

    }

}
