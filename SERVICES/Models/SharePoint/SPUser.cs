using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES.Models
{
    public class SPUser: SPUserInfo
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }
    public class SPUserInfo: SPDefaultObject
    {
        private string _loginName;

        public string LoginName
        {
            get { return _loginName; }
            set { _loginName = value; }
        }
        private SPDefaultMetadata _metadataType;

        override public SPDefaultMetadata MetadataType
        {
            get {
                _metadataType.ObjectType = "SP.User";
                return _metadataType;
            }
            set { _metadataType = value; }
        }
        public SPUserInfo()
        {
            this.MetadataType = new SPDefaultMetadata();
        }


    }
    public class SPUserInfoBatch: SPUserInfo
    {
        public string IdGroup { get; set; }
    }
}
