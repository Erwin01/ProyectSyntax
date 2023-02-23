namespace SERVICES.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    public class SPGroupInfo: SPDefaultObject
    {
        public string Title { get; set; }
        public string Description { get; set; }

        private SPDefaultMetadata _metadataType;
        override public SPDefaultMetadata MetadataType
        {
            get
            {
                _metadataType.ObjectType = "SP.Group";
                return _metadataType;
            }
            set { _metadataType = value; }
        }
        public SPGroupInfo()
        {
            _metadataType = new SPDefaultMetadata();
        }
    }
    public class SPGroup
    {
        public string Id { get; set; }
        public bool IsHiddenInUI { get; set; }
        public string LoginName { get; set; }
        public string Title { get; set; }
        public int PrincipalType { get; set; }
        public bool AllowMembersEditMembership { get; set; }
        public bool AllowRequestToJoinLeave { get; set; }
        public bool AutoAcceptRequestToJoinLeave { get; set; }
        public string Description { get; set; }
        public bool OnlyAllowMembersViewMembership { get; set; }
        public string OwnerTitle { get; set; }
        public string RequestToJoinLeaveEmailSetting { get; set; }


    }
}
