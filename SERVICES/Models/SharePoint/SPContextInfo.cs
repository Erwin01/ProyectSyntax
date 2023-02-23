using System;
using System.Collections.Generic;
using System.Text;

namespace SERVICES.Models
{
    public class SPContextInfo : SPDefaultObject
    {
        public int FormDigestTimeoutSeconds { get; set; }
        public string FormDigestValue { get; set; }
        public string LibraryVersion { get; set; }
        public string SiteFullUrl { get; set; }
        public string WebFullUrl { get; set; }
        public SPSchemaVersion SupportedSchemaVersions { get; set; }
    }
    public class SPSchemaVersion : SPDefaultResults<string>
    {

    }

}
