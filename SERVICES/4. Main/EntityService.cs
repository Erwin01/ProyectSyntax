namespace SERVICES
{
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Sdk;
    using System;
    using System.Collections.Generic;
    using System.Text;    
    using System.Linq;
    using System.Web;
    using Newtonsoft.Json.Linq;
    using Models;
    using System.Threading.Tasks;

    public class EntityService: BaseService
    {
        //private properties
        private Entity defaultBusinessUnit;
        private Guid parentRecordId;
        private string spPath;
        private string recordName;
        private SPUser owner;

        //constructores
        public EntityService(): base()
        {
            defaultBusinessUnit = this.ClientCrmEntityService.Get(Guid.Parse(Config["IdUnidadNegocio"]), Constants.CRMENTITIES.BUSINESS_UNIT, new List<string> { Constants.CRMCOLUMNS.BUSINESS_UNIT_ID });
        }

        public void CreateEntity(string entityName, Guid entityId)
        {
            if (defaultBusinessUnit != null)
            {
                Task.Run(() => 
                {
                    switch (entityName)
                    {
                        case Constants.CRMENTITIES.ACCOUNT: CreateAccount(entityId); break;
                        case Constants.CRMENTITIES.OPPORTUNITY: CreateOpportunity(entityId); break;
                        case Constants.CRMENTITIES.APPOINTMENT: CreateAppointment(entityId); break;
                        case Constants.CRMENTITIES.INCIDENT: CreateIncident(entityId); break;
                        case Constants.CRMENTITIES.QUOTE: CreateQuote(entityId); break;
                        case Constants.CRMENTITIES.QUOTEDETAIL: CreateQuoteDetail(entityId); break;
                        default: throw new Exception(String.Format("CRM entity not allowed: '{0}'", entityName));
                    }
                });
                
            }
        }
        
        //creación de entidades principal
        private void CreateAccount(Guid accountId)
        {
            var idsPathCRM = CreateGenericFirstLevelEntity("account", accountId, "name");
            dynamic ownerId = null;
            if(owner != null)
            {
                ownerId = owner.Id;
            }
            var crmAccountItem = new 
            {
                __metadata = new {type= Config["SPEntityAccount"] } ,
                Title =  recordName,
                IdCRM = accountId.ToString(),
                IdRutaCRM1 = idsPathCRM[0].ToString(),
                IdRutaCRM2 = idsPathCRM[1].ToString(),
                RutaSP = spPath,
                PropietarioId = ownerId
            };
            ClientListService.Create<dynamic>("M - account", crmAccountItem);
        }
        private void CreateOpportunity(Guid opportunityId)
        {
            var idsPathCRM  = CreateGenericFirstLevelEntity("opportunity", opportunityId, "name", subFolder:"Ofertas", entityChildName: "quote", parentColumnName: "parentaccountid");
            parentRecordId = parentRecordId != null ? parentRecordId : Guid.Empty;
            var accountItems = ClientListService.GetItems<JToken>("M - account", string.Format("?$filter=IdCRM eq '{0}'", parentRecordId));
            var accountItem = accountItems.Data.Results.FirstOrDefault();
            dynamic ownerId = null;
            if (owner != null)
            {
                ownerId = owner.Id;
            }
            var crmOpportunityItem = new
            {
                __metadata = new { type = Config["SPEntityOpportunity"] },
                Title = recordName,
                IdCRM = opportunityId.ToString(),
                IdRutaCRM1 = idsPathCRM[0].ToString(),
                IdRutaCRM2 = idsPathCRM[1].ToString(),
                IdRutaCRM3 = idsPathCRM[2].ToString(),
                accountId = accountItem != null ? accountItem["Id"].ToObject<string>() : null,
                RutaSP = spPath,
                PropietarioId = ownerId
            };
            ClientListService.Create<dynamic>("M - opportunity", crmOpportunityItem);
        }
        private void CreateIncident(Guid incidentId)
        {
            var idsPathCRM = CreateGenericFirstLevelEntity("incident", incidentId, "title", subFolder:"Ofertas", entityChildName: "quote", parentColumnName: "customerid");
            parentRecordId = parentRecordId != null ? parentRecordId : Guid.Empty;
            var accountItems = ClientListService.GetItems<JToken>("M - account", string.Format("?$filter=IdCRM eq '{0}'", parentRecordId));
            var accountItem = accountItems.Data.Results.FirstOrDefault();
            dynamic ownerId = null;
            if (owner != null)
            {
                ownerId = owner.Id;
            }
            var crmIncidenttem = new
            {
                __metadata = new { type = Config["SPEntityIncident"] },
                Title = recordName,
                IdCRM = incidentId.ToString(),
                IdRutaCRM1 = idsPathCRM[0].ToString(),
                IdRutaCRM2 = idsPathCRM[1].ToString(),
                IdRutaCRM3 = idsPathCRM[2].ToString(),
                accountId = accountItem != null ? accountItem["Id"].ToObject<string>() : null,
                RutaSP = spPath,
                PropietarioId = ownerId
            };
            ClientListService.Create<dynamic>("M - incident", crmIncidenttem);
        }
        private void CreateAppointment(Guid appointmentId)
        {
            var idsPathCRM = CreateGenericFirstLevelEntity("appointment", appointmentId, "subject", "activity", parentColumnName: "iber_cliente");
            parentRecordId = parentRecordId != null ? parentRecordId : Guid.Empty;
            var accountItems = ClientListService.GetItems<JToken>("M - account", string.Format("?$filter=IdCRM eq '{0}'", parentRecordId));
            var accountItem = accountItems.Data.Results.FirstOrDefault();
            dynamic ownerId = null;
            if (owner != null)
            {
                ownerId = owner.Id;
            }
            var crmappointmentItem = new
            {
                __metadata = new { type = Config["SPEntityAppointment"] },
                Title = recordName,
                IdCRM = appointmentId.ToString(),
                IdRutaCRM1 = idsPathCRM[0].ToString(),
                IdRutaCRM2 = idsPathCRM[1].ToString(),
                accountId = accountItem != null ? accountItem["Id"].ToObject<string>() : null,
                RutaSP = spPath,
                PropietarioId = ownerId
            };
            ClientListService.Create<dynamic>("M - appointment", crmappointmentItem);
        }
        private void CreateQuote(
            Guid quoteId, 
            string entityName = "quote", 
            List<string> entityColumns = null, 
            string referenceIdName = "",
            string principalColumnTitleName = "name",
            string parentColumnName = "opportunityid",
            string parentEntityLogicalName = "opportunity")
        {
            var entityRecordId = quoteId;
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

            //get parameters
            var templateName = "quote";
            if (entity.GetAttributeValue<EntityReference>("iber_casoid") != null)
            {
                parentColumnName = "iber_casoid";
                parentEntityLogicalName = "incident";
                templateName = "quote_incident";
            }
            var url = Config[string.Format("{0}SiteUrl", parentEntityLogicalName)];
            var name = CleanLibraryName(entity.GetAttributeValue<string>(principalColumnTitleName));
            name += String.Format("_{0}",quoteId.ToString().Substring(0, 4));
            

            if (!string.IsNullOrEmpty(parentColumnName))
            {
               parentRecordId = entity.GetAttributeValue<EntityReference>(parentColumnName) != null ? entity.GetAttributeValue<EntityReference>(parentColumnName).Id : Guid.Empty;
               if(parentRecordId != Guid.Empty)
                {
                    var parentItems = ClientListService.GetItems<JToken>(String.Format("M - {0}", parentEntityLogicalName), string.Format("?$filter=IdCRM eq '{0}'", parentRecordId));
                    var parentItem = parentItems.Data.Results.FirstOrDefault();
                    var spUser = new SPDefaultData<SPUser>();
                    try
                    {
                        spUser = ClientWebService.EnsureUser((entity.GetAttributeValue<object>("u.windowsliveid") as AliasedValue).Value.ToString());

                    }
                    catch { }
                    owner = spUser != null ? spUser.Data : null;
                    //create principal folder
                    var folderInfo = ClientFolderService.Create(
                            url,
                            string.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR, parentItem["RutaSP"].ToString(), "Ofertas"),
                            name, "?$expand=RootFolder");
                    Dictionary<string, object> rootAttrs = new Dictionary<string, object>()
                    {
                        { "name", String.Format("{0}", name) },
                        { "relativeurl" , name},
                        { "regardingobjectid", new EntityReference(entityName, entityRecordId)},
                        { "parentsiteorlocation", new EntityReference("sharepointdocumentlocation", Guid.Parse(parentItem["IdRutaCRM3"].ToString()))}
                    };
                    var idPathRoot = ClientCrmEntityService.Create("sharepointdocumentlocation", rootAttrs);
                    ClientFolderService.BreakRoleInheritance(url, folderInfo.Data.UniqueId, false);
                    ClientFolderService.AddRoleAssigment(url, folderInfo.Data.UniqueId, spUser.Data.Id, Constants.SPROLEASSIGMENTS.EDIT);

                    //create subfolders
                    CreateDefaultTemplates(templateName, url, folderInfo.Data.ServerRelativeUrl);
                    var subFolderInfo = ClientFolderService.Create(url, folderInfo.Data.ServerRelativeUrl, "Líneas de oferta", "?$expand=RootFolder");
                   /* ClientFolderService.BreakRoleInheritance(url, subFolderInfo.Data.ServerRelativeUrl);
                    var idEditGroup = int.Parse(Config[string.Format("{0}GroupEditId", "quotedetail")]);
                    var idReadGroup = int.Parse(Config[string.Format("{0}GroupReadId", "quotedetail")]);
                    ClientFolderService.AddRoleAssigment(url, subFolderInfo.Data.ServerRelativeUrl, idEditGroup, Constants.SPROLEASSIGMENTS.EDIT);
                    ClientFolderService.AddRoleAssigment(url, subFolderInfo.Data.ServerRelativeUrl, idReadGroup, Constants.SPROLEASSIGMENTS.READ); */

                    Dictionary<string, object> subAttrs = new Dictionary<string, object>()
                    {
                        { "name", String.Format("Root_{0}", "Líneas de oferta") },
                        { "relativeurl" , "Líneas de oferta"},
                        { "parentsiteorlocation", new EntityReference("sharepointdocumentlocation", idPathRoot)}
                    };
                    var idSubFolderPath = ClientCrmEntityService.Create("sharepointdocumentlocation", subAttrs);

                    dynamic ownerId = null;
                    if (owner != null)
                    {
                        ownerId = owner.Id;
                    }
                    //save item info
                    var crmQuoteItem = new
                    {
                        __metadata = new { type = Config["SPEntityQuote"] },
                        Title = name,
                        IdCRM = entityRecordId.ToString(),
                        IdRutaCRM1 = "N/A",
                        IdRutaCRM2 = idPathRoot.ToString(),
                        IdRutaCRM3 = idSubFolderPath.ToString(),
                        Parent = parentItem != null ? parentItem["Id"].ToObject<string>() : null,
                        ParentEntity = parentEntityLogicalName,
                        RutaSP = folderInfo.Data.ServerRelativeUrl,
                        PropietarioId = ownerId
                    };
                    ClientListService.Create<dynamic>("M - quote", crmQuoteItem);
                }
            }
        }
        private void CreateQuoteDetail(
            Guid quoteDetailId,
            string entityName = "quotedetail",
            List<string> entityColumns = null,
            string referenceIdName = "",
            string principalColumnTitleName = "quotedetailname",
            string parentColumnName = "quoteid",
            string parentEntityLogicalName = "quote")
        {
            var entityRecordId = quoteDetailId;
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

            //get parameters
            var name = CleanLibraryName(entity.GetAttributeValue<string>(principalColumnTitleName));
            name += string.Format("_{0}",entityRecordId.ToString().Substring(0, 4));

            if (!string.IsNullOrEmpty(parentColumnName))
            {
                parentRecordId = entity.GetAttributeValue<EntityReference>(parentColumnName) != null ? entity.GetAttributeValue<EntityReference>(parentColumnName).Id : Guid.Empty;
                if (parentRecordId != Guid.Empty)
                {
                    var parentItems = ClientListService.GetItems<JToken>(String.Format("M - {0}", parentEntityLogicalName), string.Format("?$filter=IdCRM eq '{0}'", parentRecordId));
                    var parentItem = parentItems.Data.Results.FirstOrDefault();
                    var spUser = ClientWebService.EnsureUser((entity.GetAttributeValue<object>("u.windowsliveid") as AliasedValue).Value.ToString());
                    var url = Config[string.Format("{0}SiteUrl", parentItem["ParentEntity"].ToString())];
                    //create principal folder
                    var folderInfo = ClientFolderService.Create(
                            url,
                            string.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR, parentItem["RutaSP"].ToString(), "Líneas de oferta"),
                            name, "?$expand=RootFolder");
                    Dictionary<string, object> rootAttrs = new Dictionary<string, object>()
                    {
                        { "name", String.Format("{0}", name) },
                        { "relativeurl" , name},
                        { "regardingobjectid", new EntityReference(entityName, entityRecordId)},
                        { "parentsiteorlocation", new EntityReference("sharepointdocumentlocation", Guid.Parse(parentItem["IdRutaCRM3"].ToString()))}
                    };
                    var idPathRoot = ClientCrmEntityService.Create("sharepointdocumentlocation", rootAttrs);
                    ClientFolderService.BreakRoleInheritance(url, folderInfo.Data.UniqueId, false);
                    ClientFolderService.AddRoleAssigment(url, folderInfo.Data.UniqueId, spUser.Data.Id, Constants.SPROLEASSIGMENTS.EDIT);

                    //create subfolders
                    CreateDefaultTemplates(entityName, url, folderInfo.Data.ServerRelativeUrl);

                    //save item info
                    var crmQuoteDetailItem = new
                    {
                        __metadata = new { type = Config["SPEntityQuotedetail"] },
                        Title = name,
                        IdCRM = entityRecordId.ToString(),
                        IdRutaCRM1 = "N/A",
                        IdRutaCRM2 = idPathRoot.ToString(),
                        quoteId = parentItem != null ? parentItem["Id"].ToObject<string>() : null,
                        RutaSP = folderInfo.Data.ServerRelativeUrl
                    };
                    ClientListService.Create<dynamic>("M - quotedetail", crmQuoteDetailItem);
                }
            }

        }

        //metodos genéricos
        private List<Guid>  CreateGenericFirstLevelEntity(
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
            var spUser = new SPDefaultData<SPUser>();
            try
            {
                spUser = ClientWebService.EnsureUser((entity.GetAttributeValue<object>("u.windowsliveid") as AliasedValue).Value.ToString());
     
            }
            catch { }
            owner = spUser != null ? spUser.Data : null;
            var libraryInfo = ClientListService.CreateLibrary(listTitle, url, "?$expand=RootFolder");
            ClientListService.BreakRoleInheritance(listTitle, url);
            if(spUser != null)
            {
                ClientListService.AddRoleAssigment(spUser.Data.Id, listTitle, Constants.SPROLEASSIGMENTS.EDIT, url);
            }
            ClientListService.AddRoleAssigment(int.Parse(Config[string.Format("{0}GroupReadId", entityName)]), listTitle, Constants.SPROLEASSIGMENTS.READ, url);
            ClientListService.AddRoleAssigment(int.Parse(Config[string.Format("{0}GroupEditId", entityName)]), listTitle, Constants.SPROLEASSIGMENTS.EDIT, url);
            var folderInfo = ClientFolderService.Create(url, libraryInfo.Data.RootFolder.ServerRelativeUrl, name, "?$expand=RootFolder");
            parentRecordId = entity.GetAttributeValue<EntityReference>(parentColumnName) != null ? entity.GetAttributeValue<EntityReference>(parentColumnName).Id : Guid.Empty;
            spPath = folderInfo.Data.ServerRelativeUrl;
            recordName = name;
            var idsPathCRM = CreateDocumentLocationFirstLevel(libraryInfo.Data.RootFolder.ServerRelativeUrl.Split('/').LastOrDefault(), entityName, name, entityRecordId, Guid.Parse(Config[string.Format("{0}PathId", entityName)]));
            if(!string.IsNullOrEmpty(subFolder))
            {
                idsPathCRM.Add(CreateEntityChildContainer(url, folderInfo.Data.ServerRelativeUrl, subFolder, entityChildName, idsPathCRM.LastOrDefault()));
            }
            CreateDefaultTemplates(entityName, url, folderInfo.Data.ServerRelativeUrl);
            return idsPathCRM;
        }
    }
}

