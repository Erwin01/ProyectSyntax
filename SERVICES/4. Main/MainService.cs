namespace SERVICES
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Text;
    using ViewModels = Models;
    public class MainService
    {
        private NameValueCollection _config;
        private ICollection<ViewModels.Config> _parameters;

        public NameValueCollection Config
        {
            get { return _config; }
        }

        public MainService()
        {
            _config = ConfigurationManager.AppSettings;
        }
    }
}
