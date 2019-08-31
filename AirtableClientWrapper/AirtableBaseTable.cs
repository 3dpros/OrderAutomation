using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirtableClientWrapper
{
    public class AirtableBaseTable : IDisposable
    {

            protected AirtableApiClient.AirtableBase _airtableBase;
            private readonly string APIkey = "keyulyUigcnm900QJ";
            private readonly string AppID = "appelZMJhSKxEgz9q";


            public AirtableBaseTable()
            {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            _airtableBase = new AirtableApiClient.AirtableBase(APIkey, AppID);
            }

            public void Dispose()
            {
                _airtableBase.Dispose();
            }

    }
}

