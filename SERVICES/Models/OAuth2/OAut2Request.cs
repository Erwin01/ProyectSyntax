namespace SERVICES.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class OAut2Request
    {

        public string grant_type { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string resource { get; set; }
    }
}
