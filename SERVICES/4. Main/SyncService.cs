namespace SERVICES
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Newtonsoft.Json;
    using SERVICES.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using ViewModels = Models;
    public class SyncService : BaseService
    {
        private Entity defaultBusinessUnit;
        public SyncService() : base()
        {
            defaultBusinessUnit = this.ClientCrmEntityService.Get(Guid.Parse(Config["IdUnidadNegocio"]), Constants.CRMENTITIES.BUSINESS_UNIT, new List<string> { Constants.CRMCOLUMNS.BUSINESS_UNIT_ID });
        }
        public string SyncTemplates()
        {
            try
            {
                var templates = ClientCrmEntityService.Get("iber_plantillas", new List<string> { "iber_carpeta", "iber_entidad", "iber_rolseguridad" });
                var existingTemplates = ClientListService.GetItems<dynamic>("M - Grupo Plantilla", "?$select=Title,Id").Data.Results;
                var messages = String.Empty;
                foreach (var template in templates)
                {
                    var name = template.GetAttributeValue<string>("iber_carpeta");
                    var entityName = template.GetAttributeValue<string>("iber_entidad");
                    var rolId = template.GetAttributeValue<string>("iber_rolseguridad");
                    var titleGroup = string.Format("{0}_{1}", name, entityName);
                    var _existingTemplate = existingTemplates.Where(existingTemplate => existingTemplate.Title.ToString() == titleGroup).FirstOrDefault();
                    if (_existingTemplate == null)
                    {
                        var spGroup = ClientGroupService.Create(new SPGroupInfo
                        {
                            Title = titleGroup,
                            Description = string.Format("grupo por defecto para la plantilla {0}", titleGroup)
                        });
                        var itemTemplate = new
                        {
                            Plantilla = name,
                            Entidad = entityName,
                            Rol = rolId,
                            IdGrupo = spGroup.Data.Id,
                            Title = titleGroup,
                            __metadata = new { type = "SP.Data.GrupoPlantillaListItem" }
                        };
                        ClientListService.Create<dynamic>("M - Grupo Plantilla", itemTemplate);
                    }
                    else
                    {
                        var itemTemplate = _existingTemplate;
                        itemTemplate.Rol = rolId;
                        ClientListService.Update<dynamic>("M - Grupo Plantilla", itemTemplate.Id.ToString(), itemTemplate);
                    }
                }
                return messages;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private string DeleteTemplates()
        {
            try
            {
                var spTemplates = ClientListService.GetItems<dynamic>("M - Grupo Plantilla").Data.Results;
                var groups = new List<ViewModels.SPGroup>();
                foreach (var spTemplate in spTemplates)
                {
                    groups.Add(new ViewModels.SPGroup
                    {
                        Id = spTemplate.IdGrupo
                    });
                }
                if (spTemplates.Count > 0)
                {
                    var response = ClientListService.DeleteItemsAsBatch("M - Grupo Plantilla", spTemplates);
                    return new StreamReader(response).ReadToEnd();
                }
            }
            catch (Exception)
            {

                throw;
            }

            return null;
        }
        public string SyncEntity(string entityName, DateTime initDate, DateTime endDate)
        {
            string message = string.Empty;
            if (defaultBusinessUnit != null)
            {
                switch (entityName)
                {
                    case Constants.CRMENTITIES.ACCOUNT:
                        {
                            message = CreateAccounts(initDate, endDate);
                        }
                        break;
                    case Constants.CRMENTITIES.OPPORTUNITY: { } break;
                    case Constants.CRMENTITIES.APPOINTMENT: { } break;
                    case Constants.CRMENTITIES.INCIDENT: { } break;
                    case Constants.CRMENTITIES.QUOTE: { } break;
                    case Constants.CRMENTITIES.QUOTEDETAIL: { } break;
                    default: throw new Exception(String.Format("CRM entity not allowed: '{0}'", entityName));
                }
            }

            return message;
        }
        private string CreateAccounts(DateTime initDate, DateTime endDate)
        {
            var filter = new FilterExpression(LogicalOperator.And);
            filter.AddCondition(Constants.CRMCOLUMNS.OWNING_BUSINESS_UNIT_ID, ConditionOperator.Equal, defaultBusinessUnit.Id);
            if(initDate != null && initDate != DateTime.MinValue)
            {
                filter.AddCondition(Constants.CRMCOLUMNS.CREATED_ON, ConditionOperator.GreaterEqual, initDate);
            }
            if(endDate != null && endDate != DateTime.MinValue)
            {
                filter.AddCondition(Constants.CRMCOLUMNS.CREATED_ON, ConditionOperator.LessEqual, endDate);
            }
            QueryExpression query = new QueryExpression 
            {
                EntityName = "account",
                ColumnSet = new ColumnSet("accountid","name") ,
                Criteria = filter,
                TopCount = 300
            };
            var accounts = ClientCrmEntityService.Get(query);
            var entityClient = new EntityService();
            var messages = new List<string>();
            foreach(var account in accounts)
            {
                try
                {
                    
                }
                catch(Exception ex) 
                {
                    messages.Add(
                        string.Format("error en cuenta: {0} - {1}, con mensaje {2}", 
                        account.GetAttributeValue<string>("name"), 
                        account.Id, 
                        ex.Message));
                }
            }
            return string.Join("\r\n",messages);

        }
        private List<Guid> CreateGenericFirstLevelEntity(
           string entityName,
           Guid entityRecordId,
           string principalColumnTitleName,
           string referenceIdName = "",
           string subFolder = "",
           string entityChildName = "",
           string parentColumnName = "",
           List<string> entityColumns = null)
        {
            var filter = new FilterExpression();
            filter.AddCondition(Constants.CRMCOLUMNS.OWNING_BUSINESS_UNIT_ID, ConditionOperator.Equal, defaultBusinessUnit.Id);
            LinkEntity users = new LinkEntity(entityName, "systemuser", "ownerid", "systemuserid", JoinOperator.Inner);
            users.Columns.AddColumns("systemuserid", "windowsliveid");
            users.EntityAlias = "u";
            var entity = ClientCrmEntityService
                .Get(
                    entityRecordId,
                    entityName,
                    entityColumns,
                    filter,
                    new List<LinkEntity> { users },
                    referenceIdName);
            var url = Config[string.Format("{0}SiteUrl", entityName)];
            var name = CleanLibraryName(entity.GetAttributeValue<string>(principalColumnTitleName));
            var listTitle = string.Format(Constants.FORMATS.GENERAL.UNDERSCORE_SEPARATOR, name, entityRecordId);
            var spUser = ClientWebService.EnsureUser((entity.GetAttributeValue<object>("u.windowsliveid") as AliasedValue).Value.ToString());
            var libraryInfo = ClientListService.CreateLibrary(listTitle, url, "?$expand=RootFolder");
            ClientListService.BreakRoleInheritance(listTitle, url);
            ClientListService.AddRoleAssigment(spUser.Data.Id, listTitle, Constants.SPROLEASSIGMENTS.EDIT, url);
            var folderInfo = ClientFolderService.Create(url, libraryInfo.Data.RootFolder.ServerRelativeUrl, name, "?$expand=RootFolder");
            var parentRecordId = !string.IsNullOrEmpty(parentColumnName) ? entity.GetAttributeValue<EntityReference>(parentColumnName).Id : Guid.Empty;
            var spPath = folderInfo.Data.ServerRelativeUrl;
            var recordName = name;
            var idsPathCRM = CreateDocumentLocationFirstLevel(libraryInfo.Data.RootFolder.ServerRelativeUrl.Split('/').LastOrDefault(), entityName, name, entityRecordId, Guid.Parse(Config[string.Format("{0}PathId", entityName)]));
            if (!string.IsNullOrEmpty(subFolder))
            {
                idsPathCRM.Add(CreateEntityChildContainer(url, folderInfo.Data.ServerRelativeUrl, subFolder, entityChildName, idsPathCRM.LastOrDefault()));
            }
            CreateDefaultTemplates(entityName, url, folderInfo.Data.ServerRelativeUrl);
            return idsPathCRM;
        }
        private void CreateDefaultTemplates(string entityName, string subSiteUrl, string relativeFolder)
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
            foreach (var entity in entities)
            {
                var folderName = entity.GetAttributeValue<string>("iber_carpeta");
                ClientFolderService.Create(subSiteUrl, relativeFolder, folderName);
            }

        }
        private new Guid CreateEntityChildContainer(
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
            ClientFolderService.AddRoleAssigment(subSiteUrl, folderInfo.Data.ServerRelativeUrl, idEditGroup, Constants.SPROLEASSIGMENTS.EDIT);
            ClientFolderService.AddRoleAssigment(subSiteUrl, folderInfo.Data.ServerRelativeUrl, idReadGroup, Constants.SPROLEASSIGMENTS.READ);
            Dictionary<string, object> rootAttrs = new Dictionary<string, object>()
                {
                    { "name", String.Format("Root_{0}", folderName) },
                    { "relativeurl" , folderName},
                    { "parentsiteorlocation", new EntityReference("sharepointdocumentlocation", idParentLocation)}
                };
            return ClientCrmEntityService.Create("sharepointdocumentlocation", rootAttrs);
        }
        private new string CleanLibraryName(string name)
        {
            return name.Trim(' ').Trim('\t').Trim('\n').Trim('\r').Replace(".", string.Empty).Replace("/", "-");
        }
        private new List<Guid> CreateDocumentLocationFirstLevel(string libraryName, string entityName, string entityRecordName, Guid entityRecordId, Guid idParentLocation)
        {
            Dictionary<string, object> rootAttrs = new Dictionary<string, object>()
                {
                    { "name", String.Format("Root_{0}", entityRecordName) },
                    { "relativeurl" , libraryName},
                    { "parentsiteorlocation", new EntityReference("sharepointsite", idParentLocation)}
                };
            var pathIdRoot = ClientCrmEntityService.Create("sharepointdocumentlocation", rootAttrs);
            Dictionary<string, object> folderAttrs = new Dictionary<string, object>()
                {
                    { "name", entityRecordName},
                    { "relativeurl" , entityRecordName},
                    { "regardingobjectid", new EntityReference(entityName, entityRecordId)},
                    { "parentsiteorlocation", new EntityReference("sharepointdocumentlocation", pathIdRoot)}
                };
            var pathIdRoot2 = ClientCrmEntityService.Create("sharepointdocumentlocation", folderAttrs);
            return new List<Guid> { pathIdRoot, pathIdRoot2 };
        }


    }
}
