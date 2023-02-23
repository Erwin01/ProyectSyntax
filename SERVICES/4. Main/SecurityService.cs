namespace SERVICES
{
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Tooling.Connector;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ViewModels = Models;
    using SERVICES.Models;

    public class SecurityService : BaseService
    {
        private SPGroupService _clientPnpGroupService;
        public SecurityService() : base()
        {
            _clientPnpGroupService = new SPGroupService(Config[Constants.APPSETTINGS.SITE_URL]);
        }

        public void SyncUsersTemplates()
        {
            try
            {

                var existingTemplates = ClientListService.GetItems<dynamic>("M - Grupo Plantilla", "?$select=Title,Rol, IdGrupo, Plantilla").Data.Results;
                QueryExpression queryTemplates = new QueryExpression
                {
                    EntityName = "iber_plantillas",
                    ColumnSet = new ColumnSet("iber_carpeta", "iber_entidad", "iber_rolseguridad")
                };
                var templates = ClientCrmEntityService.Get("iber_plantillas", columns: new List<string> { "iber_carpeta", "iber_entidad", "iber_rolseguridad" });
                foreach (var template in templates)
                {
                    var rol = template.GetAttributeValue<string>("iber_rolseguridad");
                    var templateName = template.GetAttributeValue<string>("iber_carpeta");
                    var group = existingTemplates.Where(t => t.Plantilla.ToString() == templateName).FirstOrDefault();
                    if (group != null)
                    {
                        var grupoId = group.IdGrupo;
                        if (rol != null)
                        {
                            var query = new QueryExpression
                            {
                                EntityName = "systemuser",
                                ColumnSet = new ColumnSet("systemuserid", "firstname", "internalemailaddress", "windowsliveid")
                            };
                            query.PageInfo.ReturnTotalRecordCount = true;
                            query.PageInfo.PageNumber = 1;

                            LinkEntity userRoles = query.AddLink("systemuserroles", "systemuserid", "systemuserid", JoinOperator.Inner);
                            userRoles.Columns.AddColumns("systemuserid", "roleid");
                            userRoles.EntityAlias = "sr";

                            LinkEntity roleLink = userRoles.AddLink("role", "roleid", "roleid", JoinOperator.Inner);
                            roleLink.Columns.AddColumns("name");
                            roleLink.EntityAlias = "r";

                            var roleFilter = new FilterExpression(LogicalOperator.Or);
                            roleFilter.AddCondition("name", ConditionOperator.Equal, rol);
                            roleLink.LinkCriteria.AddFilter(roleFilter);
                            var usersEntity = ConvertToIntegratedUsers(ClientCrmEntityService.Get(query), true);

                            string groupId = grupoId.ToString();
                            IList<ViewModels.SPUser> usersInGroup = ClientGroupService.GetUsers(groupId).Data.Results.ToList();
                            DeleteTemplatePermissions(groupId, usersEntity, usersInGroup);
                            var usersToAdd = new List<ViewModels.IntegratedUser>();
                            foreach (var user in usersEntity)
                            {
                                if (!usersInGroup.Any(_u => _u.LoginName.ToUpper() == user.LoginName.ToUpper()))
                                {
                                    usersToAdd.Add(user);
                                }
                            }
                            var spUsers = new List<ViewModels.SPUserInfoBatch>();
                            foreach (var user in usersToAdd)
                            {
                                var email = user.Email;
                                _clientPnpGroupService.AddUserAsBatch(new ViewModels.SPUser
                                {
                                    Email = email
                                }, int.Parse(grupoId.ToString()));
                            }
                        }
                    }
                }


            }
            catch (Exception)
            {

                throw;
            }
        }
        public string SyncUsers(string entityName)
        {
            try
            {
                var users = GetDynamics365UsersByEntity(entityName);
                var groupEditId = Config[string.Format("{0}GroupEditId", entityName)];
                var groupReadId = Config[string.Format("{0}GroupReadId", entityName)];
                var usersEntity = GetDynamics365UsersByEntity(entityName);
                var readUsers = ClientGroupService.GetUsers(groupReadId).Data.Results.ToList();
                var editUsers = ClientGroupService.GetUsers(groupEditId).Data.Results.ToList();
                var usersToAdd = new List<dynamic>();
                foreach (var user in usersEntity)
                {
                    if (!readUsers.Any(_u => _u.LoginName.ToUpper() == user.LoginName.ToUpper())
                        || !editUsers.Any(_u => _u.LoginName.ToUpper() == user.LoginName.ToUpper()))
                    {
                        usersToAdd.Add(user);
                    }
                }
                Task.Run(() =>
                {
                    foreach (var user in usersToAdd)
                    {
                        var email = user.Email;
                        var levelSP = user.LevelSP;
                        var groupId = Config[string.Format("{0}Group{1}Id", entityName, levelSP)];
                        try
                        {

                            _clientPnpGroupService.AddUserAsBatch(new ViewModels.SPUser
                            {
                                Email = email
                            }, int.Parse(groupId));
                        }
                        catch
                        {

                        }
                    }
                });

                return string.Empty;
            }
            catch (Exception)
            {

                throw;
            }



        }

        private ICollection<ViewModels.IntegratedUser> GetDynamics365UsersByEntity(string entityName)
        {
            try
            {
                var query = new QueryExpression
                {
                    EntityName = "systemuser",
                    ColumnSet = new ColumnSet("systemuserid", "firstname", "internalemailaddress", "windowsliveid")
                };
                query.PageInfo.ReturnTotalRecordCount = true;
                query.PageInfo.PageNumber = 1;

                LinkEntity userRoles = query.AddLink("systemuserroles", "systemuserid", "systemuserid", JoinOperator.Inner);
                userRoles.Columns.AddColumns("systemuserid", "roleid");
                userRoles.EntityAlias = "sr";

                LinkEntity roleLink = userRoles.AddLink("role", "roleid", "roleid", JoinOperator.Inner);
                roleLink.Columns.AddColumns("name");
                roleLink.EntityAlias = "r";

                LinkEntity roleprivilegesLink = roleLink.AddLink("roleprivileges", "roleid", "roleid", JoinOperator.Inner);
                roleprivilegesLink.Columns.AddColumns("privilegedepthmask");
                roleprivilegesLink.EntityAlias = "rp";

                var permMaskFilterExpression = new FilterExpression(LogicalOperator.Or);

                LinkEntity privilegesLink = roleprivilegesLink.AddLink("privilege", "privilegeid", "privilegeid", JoinOperator.Inner);
                privilegesLink.Columns.AddColumns("name", "accessright");
                privilegesLink.EntityAlias = "p";

                LinkEntity objectTypeLink = roleprivilegesLink.AddLink("privilegeobjecttypecodes", "privilegeid", "privilegeid", JoinOperator.Inner);
                objectTypeLink.Columns.AddColumns("objecttypecode");
                objectTypeLink.EntityAlias = "potc";

                var nameFilterExpression = new FilterExpression(LogicalOperator.Or);
                nameFilterExpression.AddCondition("objecttypecode", ConditionOperator.Equal, entityName);
                objectTypeLink.LinkCriteria.AddFilter(nameFilterExpression);
                return ConvertToIntegratedUsers(ClientCrmEntityService.Get(query));
            }
            catch (Exception)
            {

                throw;
            }
        }

        private ICollection<ViewModels.IntegratedUser> ConvertToIntegratedUsers(ICollection<Entity> users, bool isTemplate = false)
        {
            return users.Select(u =>
               new ViewModels.IntegratedUser
               {
                   LoginName = $"i:0#.f|membership|{u.GetAttributeValue<string>("windowsliveid")}",
                   Email = u.GetAttributeValue<string>("windowsliveid"),
                   LevelSP = isTemplate ? string.Empty : u.GetAttributeValue<AliasedValue>("p.accessright").Value.ToString() != "0" ? (u.GetAttributeValue<AliasedValue>("p.accessright").Value.ToString() != "1" ? "Read" : "Edit") : ""
               })
            .Distinct(new ViewModels.IntregratedUserComparer())
            .ToList();
        }

        public void DeletePermissions(string entityName)
        {
            var groupEditId = Config[string.Format("{0}GroupEditId", entityName)];
            var groupReadId = Config[string.Format("{0}GroupReadId", entityName)];
            var usersEntity = GetDynamics365UsersByEntity(entityName);
            var readUsers = ClientGroupService.GetUsers(groupReadId).Data.Results.ToList();
            var usersToDelete = new List<ViewModels.SPUser>();
            foreach (var user in readUsers)
            {
                if (!usersEntity.Any(_u => _u.LoginName.ToUpper() == user.LoginName.ToUpper() && _u.LevelSP == "Read"))
                {
                    usersToDelete.Add(user);
                }
            }
            if (usersToDelete.Count() > 0)
            {
                _clientPnpGroupService.DeleteUsersAsBatch(usersToDelete, groupReadId);
            }

            var editUsers = ClientGroupService.GetUsers(groupEditId).Data.Results.ToList();
            usersToDelete = new List<ViewModels.SPUser>();
            foreach (var user in editUsers)
            {
                if (!usersEntity.Any(_u => _u.LoginName.ToUpper() == user.LoginName.ToUpper() && _u.LevelSP == "Edit"))
                {
                    usersToDelete.Add(user);
                }
            }
            if (usersToDelete.Count() > 0)
            {
                _clientPnpGroupService.DeleteUsersAsBatch(usersToDelete, groupEditId);
            }
        }
        public void DeleteTemplatePermissions(string groupId, ICollection<ViewModels.IntegratedUser> usersEntity, IList<ViewModels.SPUser> usersInGroup)
        {
            var usersToDelete = new List<ViewModels.SPUser>();
            foreach (var user in usersInGroup)
            {
                if (!usersEntity.Any(_u => _u.LoginName.ToUpper() == user.LoginName.ToUpper()))
                {
                    usersToDelete.Add(user);
                }
            }
            if (usersToDelete.Count() > 0)
            {
                _clientPnpGroupService.DeleteUsersAsBatch(usersToDelete, groupId);
            }
        }

        public void Create()
        {
            var businessUnitParam = this.Parameters.Where(p => p.Title == Constants.CONFIGPROPERTIES.BUSINESS_UNIT).FirstOrDefault();
            if (businessUnitParam != null)
            {
                var defaultBusinessUnit = this.ClientCrmEntityService.Get(Guid.Parse(businessUnitParam.Valor), Constants.CRMENTITIES.BUSINESS_UNIT);
                if (defaultBusinessUnit != null)
                {
                    var defaultBusinessUnitName = defaultBusinessUnit.GetAttributeValue<string>(Constants.CRMCOLUMNS.NAME);
                    var dataGroupEntities = this.ClientListService.GetItems<ViewModels.MasterEntityGroup>("M - Grupo", Constants.QUERY.ENTITY_GROUPS);
                    if (dataGroupEntities != null)
                    {
                        if (dataGroupEntities.Data != null)
                        {
                            this.CreateGroupsEntities(dataGroupEntities.Data.Results, defaultBusinessUnitName);
                        }
                    }
                    this.CreateGroupsTemplates(defaultBusinessUnitName);
                }
            }

        }
        public void Delete()
        {
            var dataGroupEntities = this.ClientListService.GetItems<ViewModels.MasterEntityGroup>("M - Grupo", Constants.QUERY.ENTITY_GROUPS);
            if (dataGroupEntities != null)
            {
                if (dataGroupEntities.Data != null)
                {
                    this.DeleteGroups(dataGroupEntities.Data.Results);
                }
            }
            this.DeleteGroupsTemplates();

        }
        private void CreateGroupsEntities(ICollection<ViewModels.MasterEntityGroup> groupEntities, string defaultBusinessUnitName)
        {
            foreach (var groupEntity in groupEntities)
            {
                var newGroup = this.ClientGroupService.Create(new ViewModels.SPGroupInfo
                {
                    Title = groupEntity.Title,
                    Description = groupEntity.Title,
                    MetadataType = new ViewModels.SPDefaultMetadata
                    {
                        ObjectType = Constants.SPMETADATATYPES.GROUP
                    }
                });
                this.ClientListService.Update<ViewModels.MasterEntityGroupInfo>("M - Grupo", groupEntity.Id, new ViewModels.MasterEntityGroupInfo
                {
                    IdGroup = newGroup.Data.Id,
                    Title = groupEntity.Title,
                    MetadataType = new ViewModels.SPDefaultMetadata
                    {
                        ObjectType = "SP.Data.GrupoListItem"
                    }
                });
                this.ClientWebService.AddRoleAssigment(newGroup.Data.Id, Constants.SPROLEASSIGMENTS.READ);
                var urlSubSite = String.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR, defaultBusinessUnitName, groupEntity.Entidad.LogicalName);
                this.ClientWebService.AddRoleAssigment(newGroup.Data.Id, groupEntity.Accion.IdRolSP, urlSubSite);
            }
        }
        private void CreateGroupsTemplates(string defaultBusinessUnitName)
        {
            var dataGroupTemplates = this.ClientListService.GetItems<ViewModels.MasterTemplateInfo>("M - Grupo Plantilla");
            if (dataGroupTemplates != null)
            {
                if (dataGroupTemplates.Data != null)
                {
                    foreach (var dataGroupTemplate in dataGroupTemplates.Data.Results)
                    {
                        var newGroup = this.ClientGroupService.Create(new ViewModels.SPGroupInfo
                        {
                            Title = dataGroupTemplate.Title,
                            Description = dataGroupTemplate.Title,
                            MetadataType = new ViewModels.SPDefaultMetadata
                            {
                                ObjectType = Constants.SPMETADATATYPES.GROUP
                            }
                        });
                        this.ClientListService.Update<ViewModels.MasterTemplate>("M - Grupo Plantilla", dataGroupTemplate.Id, new ViewModels.MasterTemplate
                        {
                            Entidad = dataGroupTemplate.Entidad,
                            Title = dataGroupTemplate.Title,
                            Rol = dataGroupTemplate.Rol = dataGroupTemplate.Rol,
                            IdGrupo = newGroup.Data.Id,
                            Plantilla = dataGroupTemplate.Plantilla,
                            MetadataType = new ViewModels.SPDefaultMetadata
                            {
                                ObjectType = "SP.Data.GrupoPlantillaListItem"
                            }
                        });
                        this.ClientWebService.AddRoleAssigment(newGroup.Data.Id, Constants.SPROLEASSIGMENTS.READ);
                        var urlSubSite = String.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR, defaultBusinessUnitName, dataGroupTemplate.Plantilla);
                        this.ClientWebService.AddRoleAssigment(newGroup.Data.Id, Constants.SPROLEASSIGMENTS.EDIT, urlSubSite);
                    }
                }
            }
        }
        private void DeleteGroups(ICollection<ViewModels.MasterEntityGroup> groupEntities)
        {

            foreach (var groupEntity in groupEntities)
            {
                this.ClientGroupService.Delete(groupEntity.IdGroup);
            }
        }
        private void DeleteGroupsTemplates()
        {
            var dataGroupTemplates = this.ClientListService.GetItems<ViewModels.MasterTemplateInfo>("M - Grupo Plantilla");
            if (dataGroupTemplates != null)
            {
                if (dataGroupTemplates.Data != null)
                {
                    foreach (var dataGroupTemplate in dataGroupTemplates.Data.Results)
                    {
                        this.ClientGroupService.Delete(dataGroupTemplate.IdGrupo);
                    }
                }
            }
        }

        /// <summary>
        /// Method Get All Dynamics 365 Users
        /// </summary>
        public GenericResponse GetAllDynamics365Users()
        {

            CrmServiceClient cRMServiceClient = new CrmServiceClient(Config[Constants.APPSETTINGS.CONNECTION_STRING_CRM]);

            QueryExpression queryExpression = new QueryExpression();

            var query = new QueryExpression
            {
                EntityName = "systemuser",
                ColumnSet = new ColumnSet("systemuserid", "firstname", "internalemailaddress", "windowsliveid")
            };

            //query.PageInfo.ReturnTotalRecordCount = true;
            //query.PageInfo.PageNumber = 1;


            EntityCollection entityCollection = cRMServiceClient.RetrieveMultiple(query);

            var names = entityCollection.Entities.Select(e => e.Attributes["firstname"].ToString()).ToList();
            var quantity = names.Count;

            for (int i = 0; i < entityCollection.Entities.Count; i++)
            {
                if (entityCollection.Entities[i].Attributes.ContainsKey(Constants.CRMCOLUMNS.NAME))
                {
                    _ = entityCollection.Entities[i].Attributes[Constants.CRMCOLUMNS.NAME];

                }

            }

            return new GenericResponse
            {
                Count = quantity,
                result = entityCollection
            };

            //var code = entityCollection.Entities.Select(c => c.Attributes["systemuserid"].ToString()).ToList();

            //var names = new List<string>();

            //for (int i = 0; i < entityCollection.Entities.Count; i++)
            //{

            //    if (entityCollection.Entities[i].Attributes.ContainsKey(queryExpression.EntityName = "systemuser"))
            //    {

            //        _ = entityCollection.Entities[i].Attributes[queryExpression.EntityName = "systemuser"];
            //        //Console.WriteLine(entityCollection.Entities[i].Attributes[Constants.CRMCOLUMNS.NAME]);

            //        names.Add(entityCollection.Entities[i].Attributes[Constants.CRMCOLUMNS.NAME].ToString());

            //    }

            //}

        }





        /// <summary>
        /// Method Get All Dynamics 365 Bussiness Units
        /// </summary>
        //    public void GetAllDynamics365BusinessUnits()
        //    {

        //        CrmServiceClient cRMServiceClient = new CrmServiceClient(Config[Constants.APPSETTINGS.CONNECTION_STRING_CRM]);

        //        QueryExpression queryExpression = new QueryExpression();
        //        queryExpression.EntityName = Constants.CRMENTITIES.BUSINESS_UNIT;

        //        EntityCollection entityCollection = cRMServiceClient.RetrieveMultiple(queryExpression);

        //        for (int i = 0; i < entityCollection.Entities.Count; i++)
        //        {
        //            if (entityCollection.Entities[i].Attributes.ContainsKey(Constants.CRMCOLUMNS.NAME))
        //            {
        //                _ = entityCollection.Entities[i].Attributes[Constants.CRMCOLUMNS.NAME];
        //                Console.WriteLine(entityCollection.Entities[i].Attributes[Constants.CRMCOLUMNS.NAME]);

        //            }

        //        }

        //        Console.WriteLine("Retrieved all Bussiness Unit!");
        //        Console.ReadLine();
        //    }

        //}



        /// <summary>
        /// Method Get All Dynamics 365 Bussiness Units
        /// </summary>
        public GenericResponse GetAllDynamics365BusinessUnits()
        {

            CrmServiceClient cRMServiceClient = new CrmServiceClient(Config[Constants.APPSETTINGS.CONNECTION_STRING_CRM]);

            //QueryExpression queryExpression = new QueryExpression();

            var query = new QueryExpression
            {
                EntityName = Constants.CRMENTITIES.BUSINESS_UNIT,
                ColumnSet = new ColumnSet(Constants.CRMCOLUMNS.NAME)
            };
            
            //query.EntityName = Constants.CRMENTITIES.BUSINESS_UNIT;

            EntityCollection entityCollection = cRMServiceClient.RetrieveMultiple(query);

            //var quantity = namesBusinessUnit.Count;
            var namesBusinessUnit = entityCollection.Entities.Select(e => e.Attributes["name"].ToString()).ToList();
            var quantity = namesBusinessUnit.Count;

            //var userBussinessUnit = entityCollection.Entities.Select(e => e.Attributes["businessunit"].ToString()).ToList();

            //var quantity = Constants.CRMENTITIES.BUSINESS_UNIT.Count();

            for (int i = 0; i < entityCollection.Entities.Count; i++)
            {
                if (entityCollection.Entities[i].Attributes.ContainsKey(Constants.CRMCOLUMNS.NAME))
                {
                    _ = entityCollection.Entities[i].Attributes[Constants.CRMCOLUMNS.NAME];

                }

            }

            return new GenericResponse
            {
                Count = quantity,
                result = entityCollection

            };
        }



        //TODO: Method under construction

        /// <summary>
        /// Method Get All Dynamics Users 365 By Business Unit
        /// </summary>
        /// <returns></returns>
        public GenericResponse GetAllDynamicsUser365ByBusinessUnit()
        {

            CrmServiceClient cRMServiceClient = new CrmServiceClient(Config[Constants.APPSETTINGS.CONNECTION_STRING_CRM]);


            LinkEntity linkEntity = new LinkEntity();

            var query = new QueryExpression
            {
                EntityName = "systemuser",
                ColumnSet = new ColumnSet("systemuserid", "firstname", "internalemailaddress", "windowsliveid")
            };

            LinkEntity roleprivilegesLink = linkEntity.AddLink("roleprivileges", "roleid", "roleid", JoinOperator.Inner);
            roleprivilegesLink.Columns.AddColumns("privilegedepthmask");
            roleprivilegesLink.EntityAlias = "rp";


            //EntityCollection entityCollection = cRMServiceClient.RetrieveMultiple(query);
            EntityCollection entityCollection = new EntityCollection();


            //var names = entityCollection.Entities.Select(e => e.Attributes["systemuserid"].ToString()).ToList();


            for (int i = 0; i < entityCollection.Entities.Count; i++)
            {
                if (entityCollection.Entities[i].Attributes.ContainsKey(Constants.CRMCOLUMNS.NAME))
                {
                    _ = entityCollection.Entities[i].Attributes[Constants.CRMCOLUMNS.NAME];

                }
            }

            return new GenericResponse 
            {
                Count = entityCollection.Entities.Count,
                result = entityCollection
            };
        }

    }


    //public class Response 
    //{
    //    public List<string> Names { get; set; }
    //    public int Quantity { get; set; }


    //}
}

