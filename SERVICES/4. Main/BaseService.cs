namespace SERVICES
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ViewModels = Models;
    public class BaseService: MainService
    {
        private SPListService _clientListService;
        private CRMEntityService _clientCrmEntityService;
        private SPWebService _clientWebService;
        private SPGroupService _clientGroupService;
        private SPFileService _clientFileService;
        private ICollection<ViewModels.Config> _parameters;
        private SPFolderService _clientFolderService;

        public SPListService ClientListService 
        {
            get { return _clientListService; } 
        }
        public CRMEntityService ClientCrmEntityService
        {
            get { return _clientCrmEntityService; }
        }
        public SPWebService ClientWebService
        {
            get { return _clientWebService; }
        }
        public SPGroupService ClientGroupService
        {
            get { return _clientGroupService; }
        }
        public SPFileService ClientFileService
        {
            get { return _clientFileService; }
        }
        public SPFolderService ClientFolderService
        {
            get { return _clientFolderService; }
        }

        public ICollection<ViewModels.Config> Parameters
        {
            get { return _parameters; }
        }
        public BaseService() : base()
        {

            this._clientCrmEntityService = new CRMEntityService();
            this._clientWebService = new SPWebService();
            this._clientListService = new SPListService();
            this._clientGroupService = new SPGroupService();
            this._clientFileService = new SPFileService();
            _clientFolderService = new SPFolderService();
        }
        public string CleanLibraryName(string name)
        {
            if (name == null) 
            {
                name = "NN";
            }
            if (name.Length > 50)
            {
                name = name.Substring(0, 50);
            }
            return name
                    .Trim(' ')
                    .Replace("\t", string.Empty)
                    .Replace("\n", string.Empty)
                    .Replace("\r", string.Empty)
                    .Replace(".", string.Empty)
                    .Replace("/", "-")
                    .Replace("&", "and")
                    .Replace("\"", string.Empty)
                    .Replace("'", string.Empty)
                    .Replace("%", string.Empty)
                    .Replace(":", string.Empty)
                    .Replace("*", "x")
                    .Replace("?", string.Empty)
                    .Replace("!", string.Empty)
                    .Replace("#", "N")
                    .Replace("@", string.Empty)
                    .Replace(";", string.Empty)
                    .Replace("|", string.Empty)
                    .Replace("¿", string.Empty)
                    .Replace("º", string.Empty)
                    .Replace("=>", string.Empty)
                    .Replace(">", string.Empty)
                    .Replace("<", string.Empty)
                    .Replace("=", string.Empty)
                    .Replace("'", string.Empty)
                    .Replace(".", string.Empty)
                   .Trim();
        }

        public List<Guid> CreateDocumentLocationFirstLevel(string libraryName, string entityName, string entityRecordName, Guid entityRecordId, Guid idParentLocation)
        {
            var pathName = entityRecordName;
            if (entityRecordName.Length > 140)
            {
                pathName = entityRecordName.Substring(0, 140);
            }

            Dictionary<string, object> rootAttrs = new Dictionary<string, object>()
                {
                    { "name", String.Format("Root_{0}", pathName) },
                    { "relativeurl" , libraryName},
                    { "parentsiteorlocation", new EntityReference("sharepointsite", idParentLocation)}
                };
            var pathIdRoot = ClientCrmEntityService.Create("sharepointdocumentlocation", rootAttrs);
            Dictionary<string, object> folderAttrs = new Dictionary<string, object>()
                {
                    { "name", pathName},
                    { "relativeurl" , entityRecordName},
                    { "regardingobjectid", new EntityReference(entityName, entityRecordId)},
                    { "parentsiteorlocation", new EntityReference("sharepointdocumentlocation", pathIdRoot)}
                };
            var pathIdRoot2 = ClientCrmEntityService.Create("sharepointdocumentlocation", folderAttrs);
            return new List<Guid> { pathIdRoot, pathIdRoot2 };
        }
        public Guid CreateEntityChildContainer(
            string subSiteUrl,
            string relativeFolder, 
            string folderName, 
            string entityChildName,
            Guid idParentLocation)
        {
            var folderInfo = ClientFolderService.Create(subSiteUrl, relativeFolder, folderName);
            ClientFolderService.BreakRoleInheritance(subSiteUrl, folderInfo.Data.UniqueId);
            var idEditGroup = int.Parse(Config[string.Format("{0}GroupEditId", entityChildName)]);
            var idReadGroup = int.Parse(Config[string.Format("{0}GroupReadId", entityChildName)]);
            ClientFolderService.AddRoleAssigment(subSiteUrl, folderInfo.Data.UniqueId, idEditGroup, Constants.SPROLEASSIGMENTS.EDIT);
            ClientFolderService.AddRoleAssigment(subSiteUrl, folderInfo.Data.UniqueId, idReadGroup, Constants.SPROLEASSIGMENTS.READ);
            var pathName = folderName;
            if (folderName.Length > 140)
            {
                pathName = folderName.Substring(0, 140);
            }
            Dictionary<string, object> rootAttrs = new Dictionary<string, object>()
                {
                    { "name", String.Format("Root_{0}", pathName) },
                    { "relativeurl" , folderName},
                    { "parentsiteorlocation", new EntityReference("sharepointdocumentlocation", idParentLocation)}
                };
            return ClientCrmEntityService.Create("sharepointdocumentlocation", rootAttrs);
        }

        public void CreateDefaultTemplates(string entityName, string subSiteUrl, string relativeFolder)
        {
            var filter = new FilterExpression();
            filter.AddCondition("iber_entidad", ConditionOperator.Equal, entityName);
            var entities = ClientCrmEntityService.Get(
                    "iber_plantillas", 
                    new List<string> 
                    {
                        "iber_carpeta",
                        "iber_entidad"
                    }, 
                    filter: filter);
            var templates = ClientListService.GetItems<dynamic>("M - Grupo Plantilla", string.Format("?$filter=Entidad eq '{0}'&$select=Plantilla,IdGrupo, Rol", entityName)).Data.Results;
            foreach(var template in templates)
            {
                var folderName = template.Plantilla.ToString();
                var groupId = template.IdGrupo.ToString();
                var rol = template.Rol.ToString();
                var folderInfo = ClientFolderService.Create(subSiteUrl, relativeFolder, folderName);
                if (!string.IsNullOrEmpty(rol))
                {
                    ClientFolderService.BreakRoleInheritance(subSiteUrl, folderInfo.Data.UniqueId, true);
                    ClientFolderService.AddRoleAssigment(subSiteUrl, folderInfo.Data.UniqueId, int.Parse(groupId), Constants.SPROLEASSIGMENTS.EDIT);
                }
                
            }
            
        }
    }
}
