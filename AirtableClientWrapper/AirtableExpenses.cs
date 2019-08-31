
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirtableApiClient;


namespace AirtableClientWrapper
{
    public class AirtableExpenses : AirtableBaseTable
    {
        private readonly string TableName = "Expenses";


        public AirtableExpenses() : base()
        {
        }

        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _airtableBase.RetrieveRecord(TableName, recordID);
            var response = task.Result;

            return response.Record.Fields["Name"].ToString();

        }

        public void CreateExpensesRecord(ExpensesData expensesData)
        {
            //AirtableRecord record = new AirtableRecord();
            Fields fields = new Fields();
            fields.FieldsCollection = expensesData.ToDictionary();
            Task<AirtableCreateUpdateReplaceRecordResponse> task = _airtableBase.CreateRecord(TableName, fields);
            var response = task.Result;

        }

    }

}
