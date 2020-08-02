
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
        private AirtableMonthly _monthlyTable;
        private AirtableExpenseTypes _expenseTypes;



        public AirtableExpenses() : base()
        {
            _monthlyTable = new AirtableMonthly();
            _expenseTypes = new AirtableExpenseTypes();
        }

        public string GetRecordName(string recordID)
        {

            Task<AirtableRetrieveRecordResponse> task = _mainAirtableBase.RetrieveRecord(TableName, recordID);
            var response = task.Result;

            return response.Record.Fields["Name"].ToString();

        }

        public void CreateExpensesRecord(ExpensesData expensesData)
        {
            //AirtableRecord record = new AirtableRecord();
            Fields fields = new Fields();
            fields.FieldsCollection = expensesData.ToDictionary();
            fields.FieldsCollection["Month"] = new string[] { _monthlyTable.GetLatestMonthlyID() };
            var expenseType = _expenseTypes.MatchExpenseTypeRecordID(expensesData.Name);
            if (!string.IsNullOrEmpty(expenseType))
            {
                fields.FieldsCollection["Expense Type"] = new string[] { expenseType };
            }
            Task<AirtableCreateUpdateReplaceRecordResponse> task = _mainAirtableBase.CreateRecord(TableName, fields);
            var response = task.Result;
            if(!task.Result.Success)
            {
                throw new Exception(task.Result.AirtableApiError.ErrorMessage);
            }

        }

    }

}
