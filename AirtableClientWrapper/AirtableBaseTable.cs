using AirtableApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirtableClientWrapper
{
    public class AirtableBaseTable : IDisposable
    {

            protected AirtableApiClient.AirtableBase _mainAirtableBase;
            protected AirtableApiClient.AirtableBase _invAirtableBase;

            private readonly string APIkey = "keyulyUigcnm900QJ";
            private readonly string mainAppID = "appelZMJhSKxEgz9q";
            private readonly string invAppID = "appcbOsHUuRnuaPZ9";


            public AirtableBaseTable()
            {
                //required since airtable API requires TLS 1.2 or later
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                _mainAirtableBase = new AirtableApiClient.AirtableBase(APIkey, mainAppID);
                _invAirtableBase = new AirtableApiClient.AirtableBase(APIkey, invAppID);
            }

        public void Dispose()
            {
                _mainAirtableBase.Dispose();
            }
    }
}

