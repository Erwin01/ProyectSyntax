namespace SERVICES
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Tooling.Connector;
    public class CRMBaseService: MainService
    {
        private CrmServiceClient client;
        private OrganizationServiceProxy proxyClient;

        public OrganizationServiceProxy ProxyClient
        {
            get { return proxyClient; }
        }

        public CrmServiceClient Client 
        {
            get { return client; }
        }

        public CRMBaseService(): base()
        {
            var connectionString = this.Config[Constants.APPSETTINGS.CONNECTION_STRING_CRM];
            client = new CrmServiceClient(connectionString);
            proxyClient = client.OrganizationServiceProxy;
        }
    }
}
