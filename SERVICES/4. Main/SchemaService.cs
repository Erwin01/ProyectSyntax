namespace SERVICES
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using ViewModels = Models;
    public class SchemaService : BaseService
    {
        
        public SchemaService() : base()
        {
        }
        public void Delete()
        {
            if (this.Parameters != null)
            {

                var businessUnitParam = this.Parameters.Where(p => p.Title == Constants.CONFIGPROPERTIES.BUSINESS_UNIT).FirstOrDefault();
                if (businessUnitParam != null)
                {
                    var defaultBusinessUnit = this.ClientCrmEntityService.Get(Guid.Parse(businessUnitParam.Valor), Constants.CRMENTITIES.BUSINESS_UNIT);
                    if (defaultBusinessUnit != null)
                    {
                        var defaultBusinessUnitName = defaultBusinessUnit.GetAttributeValue<string>(Constants.CRMCOLUMNS.NAME);
                        var defaultBusinessUnitSPUrl = defaultBusinessUnitName;
                        this.DeleteEntitySites(defaultBusinessUnitSPUrl);
                        this.DeleteTemplateSites(defaultBusinessUnitSPUrl);
                    }
                }
            }
        }
        public void Create()
        {
            if (this.Parameters != null)
            {
                var businessUnitParam = this.Parameters.Where(p => p.Title == Constants.CONFIGPROPERTIES.BUSINESS_UNIT).FirstOrDefault();
                if (businessUnitParam != null)
                {
                    var defaultBusinessUnit = this.ClientCrmEntityService.Get(Guid.Parse(businessUnitParam.Valor), Constants.CRMENTITIES.BUSINESS_UNIT);
                    if (defaultBusinessUnit != null)
                    {
                        var defaultBusinessUnitName = defaultBusinessUnit.GetAttributeValue<string>(Constants.CRMCOLUMNS.NAME);
                        var newBusinessUnitWeb = this.CreateWeb(defaultBusinessUnitName, defaultBusinessUnitName, defaultBusinessUnitName + businessUnitParam.Valor);
                        this.CreateEntitySites(newBusinessUnitWeb);
                        this.CreateTemplateSites(newBusinessUnitWeb);
                    }

                }
            }
        }
        private void DeleteTemplateSites(string defaultBusinessUnitSPUrl)
        {
            var crmDataEntitites = this.ClientCrmEntityService.Get(Constants.CRMENTITIES.IBER_PLANTILLAS);
            foreach (var dataEntity in crmDataEntitites)
            {
                var title = dataEntity.GetAttributeValue<string>(Constants.CRMCOLUMNS.IBER_CARPETA);
                var url = String.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR,
                                defaultBusinessUnitSPUrl,
                                title);
                this.ClientWebService.DeleteWeb(url);
            }
            this.ClientWebService.DeleteWeb(defaultBusinessUnitSPUrl);
        }
        private void DeleteEntitySites(string defaultBusinessUnitSPUrl)
        {
            var listTitleEntities = this.Parameters.Where(p => p.Title == Constants.CONFIGPROPERTIES.LIST_ENTITITES).FirstOrDefault();
            if (listTitleEntities != null)
            {
                var dataEntitities = this.ClientListService.GetItems<ViewModels.MasterEntity>(listTitleEntities.Valor);
                if (dataEntitities != null)
                {
                    if (dataEntitities.Data != null)
                    {
                        foreach (var masterEntity in dataEntitities.Data.Results)
                        {
                            var url = String.Format(Constants.FORMATS.GENERAL.URL_SEPARATOR,
                                defaultBusinessUnitSPUrl,
                                masterEntity.LogicalName);
                            this.ClientWebService.DeleteWeb(url);
                        }
                    }
                } 
            }
        }
        private void CreateTemplateSites(ViewModels.SPDefaultData<ViewModels.SPWeb> newBusinessUnitWeb)
        {
            var dataEntitites = this.ClientCrmEntityService.Get(Constants.CRMENTITIES.IBER_PLANTILLAS);
            var subSite = newBusinessUnitWeb.Data.ServerRelativeUrl.Split('/').LastOrDefault();
            foreach (var dataEntity in dataEntitites)
            {
                var title = dataEntity.GetAttributeValue<string>(Constants.CRMCOLUMNS.IBER_CARPETA);
                var newTemplateWeb = this.CreateWeb(title, title, title, subSite);
            }
        }
        private void CreateEntitySites(ViewModels.SPDefaultData<ViewModels.SPWeb> newBusinessUnitWeb)
        {
            var listTitleEntities = this.Parameters.Where(p => p.Title == Constants.CONFIGPROPERTIES.LIST_ENTITITES).FirstOrDefault();
            if (listTitleEntities != null)
            {
                var dataEntitities = this.ClientListService.GetItems<ViewModels.MasterEntity>(listTitleEntities.Valor);
                if (dataEntitities != null)
                {
                    if (dataEntitities.Data != null)
                    {
                        var subSite = newBusinessUnitWeb.Data.ServerRelativeUrl.Split('/').LastOrDefault();
                        foreach (var masterEntity in dataEntitities.Data.Results)
                        {
                            var newEntityWeb = this.CreateWeb(masterEntity.Title, masterEntity.LogicalName, masterEntity.Title, subSite);
                        }
                    }
                }
            }
        }
        private ViewModels.SPDefaultData<ViewModels.SPWeb> CreateWeb(string title, string url, string description, string subSite = "")
        {
            return this.ClientWebService.CreateWeb(new ViewModels.SPWebInformation
            {
                Parameters = new ViewModels.SPWebBaseInfo
                {
                    Description = description,
                    Title = title,
                    Language = int.Parse(this.Config[Constants.APPSETTINGS.DEFAULT_LANGUAGE]),
                    Url = url,
                    UseUniquePermissions = true,
                    WebTemplate = this.Config[Constants.APPSETTINGS.BASE_TEMPLATE],
                    MetadataType = new ViewModels.SPDefaultMetadata
                    {
                        ObjectType = Constants.SPMETADATATYPES.WEB_INFORMATION_CREATION
                    }
                }
            }, subSite);
        }
    }
}
