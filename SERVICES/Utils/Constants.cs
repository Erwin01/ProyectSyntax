using System;
using System.Collections.Generic;
using System.Text;

namespace SERVICES
{
    public class Constants
    {
        public struct PROPERTIES
        {
            public const string ID = "id";
            public const string TOKEN_TYPE = "token_type";
            public const string EXPIRES_IN = "expires_in";
            public const string NOT_BEFORE = "not_before";
            public const string EXPIRES_ON = "expires_on";
            public const string RESOURCE = "resource";
            public const string ACCESS_TOKEN = "access_token";
            public const string SP_D = "d";
            public const string SP_RESULTS = "results";
            public const string SP_TYPE = "type";
            public const string SP_METADATA = "__metadata";
        }
        public struct APPSETTINGS
        {
            public const string SITE_URL = "siteUrl";
            public const string CLIENT_ID = "ClientId";
            public const string CLIENT_SECRET = "ClientSecret";
            public const string RESOURCE_ID = "ResourceId";
            public const string TENANT_ID = "TenantId";
            public const string DOMAIN = "Domain";
            public const string GRANT_TYPE = "GrantType";
            public const string CONNECTION_STRING_CRM= "ConnectionStringCRM";
            public const string CONFIG_LIST = "ConfigList";
            public const string BASE_TEMPLATE = "BaseTemplate";
            public const string DEFAULT_LANGUAGE = "DefaultLanguage";
        }  
        public struct FORMATS
        {
            public struct CRMCOLUMNS
            {
                public const string COLUMN_ID = "{0}id";
                public const string PARENT_COLUMN_ID = "parent{0}id";
            }
            public struct MESSAGES
            {
                public const string ERROR_IN_CALL = "Error in call with status code {0}, reason {1}";
            }
            public struct GENERAL
            {
                public const string URL_SEPARATOR = "{0}/{1}";
                public const string UNDERSCORE_SEPARATOR = "{0}_{1}";
            }
            public struct OAUTH2
            {
                public const string CLIENT_ID_SP = "{0}@{1}";
                public const string OAUTH_URL_SP = "https://accounts.accesscontrol.windows.net/{0}/tokens/OAuth/2";
                public const string RESOURCE_SP = "{0}/{1}@{2}";
            }
            public struct SPROUTES
            {
                public struct SPCONTEXT
                {
                    public const string INFO = "{0}/_api/contextinfo";
                }
                public struct SPWEB
                {
                    public const string ADD = "{0}/_api/web/webinfos/add";
                    public const string GENERAL = "{0}/_api/web";
                    public const string ADD_ROLE_ASSIGMENT = "{0}/_api/web/roleassignments/addroleassignment(principalid={1}, roledefid={2})";
                    public const string ENSURE_USER = "{0}/_api/web/ensureUser('{1}')";
                    public const string BATCH = "{0}/_api/$batch";
                }
                public struct SPGROUP
                {
                    public const string GENERAL = "{0}/_api/web/sitegroups";
                    public const string REMOVE_BY_ID = "{0}/_api/web/sitegroups/removebyid({1})";
                    public const string GET = "{0}/_api/web/sitegroups({1})";
                    public const string GET_USERS = "{0}/_api/web/sitegroups({1})/users";
                    public const string DELETE_USER_BY_EMAIL = "{0}/_api/web/sitegroups({1})/users/getbyemail('{2}')";
                }
                public struct SPLIST
                {
                    public const string GENERAL = "{0}/_api/web/lists{1}";
                    public const string GETITEMS = "{0}/_api/web/lists/getByTitle('{1}')/items{2}";
                    public const string BREAK_ROLE_INHERITANCE = "{0}/_api/web/lists/getByTitle('{1}')/breakroleinheritance({2})";
                    public const string ADD_ROLE_ASSIGMENT = "{0}/_api/web/lists/getByTitle('{1}')/roleassignments/addroleassignment(principalid={2},roleDefId={3})";
                    public const string DELETE_ITEM_BY_ID = "{0}/_api/web/lists/getByTitle('{1}')/items({2})";
                }
                public struct SPFILE
                {
                    public const string ADD = "{0}/_api/web/getfolderbyserverrelativeurl('{1}')/Files/add(overwrite={2},url='{3}')";
                }
                public struct SPFOLDER
                {
                    public const string GENERAL = "{0}/_api/web/folders{1}";
                    public const string BREAK_ROLE_INHERITANCE = "{0}/_api/web/GetFolderByServerRelativeUrl('{1}')/ListItemAllFields/breakroleinheritance({2})";
                    public const string BREAK_ROLE_INHERITANCE_BY_ID = "{0}/_api/web/GetFolderById('{1}')/ListItemAllFields/breakroleinheritance({2})";

                    public const string ROLE_ASSIGMENTS = "{0}/_api/web/GetFolderByServerRelativeUrl('{1}')/ListItemAllFields/roleassignments";
                    public const string GET_ROLE_ASSIGMENT = "{0}/_api/web/GetFolderByServerRelativeUrl('{1}')/ListItemAllFields/roleassignments/getbyprincipalid({2})";
                    public const string ADD_ROLE_ASSIGMENT = "{0}/_api/web/GetFolderByServerRelativeUrl('{1}')/ListItemAllFields/roleassignments/addroleassignment(principalid={2},roleDefId={3})";
                    public const string ADD_ROLE_ASSIGMENT_BY_ID = "{0}/_api/web/GetFolderById('{1}')/ListItemAllFields/roleassignments/addroleassignment(principalid={2},roleDefId={3})";
                }
            }
        }
        public struct CONFIGFILES
        {
            public const string APPSETTINGS = "appsettings.json";

        }
        public struct HEADERS
        {
            public const string ACCEPT = "Accept";
            public const string CONTENT_TYPE = "Content-Type";
            public const string X_REQUEST_DIGEST = "X-RequestDigest";
            public const string X_HTTP_METHOD = "X-HTTP-Method";
            public const string IF_MATCH = "IF-MATCH";
        }
        public struct MEDIATYPES
        {
            public const string JSON_ODATA_VERBOSE = "application/json;odata=verbose";
            public const string JSON = "application/json";
            public const string MULTIPART_MIXED_CHANGESET = "multipart/mixed; boundary=\"changeset_{0}\"";
        }
        public struct AUTHORIZATION
        {
            public const string BEARER = "Bearer";
        }
        public struct SPMETADATATYPES
        {
            public const string WEB_INFORMATION_CREATION = "SP.WebInfoCreationInformation";
            public const string WEB = "SP.Web";
            public const string GROUP = "SP.Group";
        }
        public struct CRMCOLUMNS
        {
            public const string ID = "id";
            public const string NAME = "name";
            public const string IBER_CARPETA = "iber_carpeta";
            public const string IBER_ENTIDAD = "iber_entidad";
            public const string IBER_ROLSEGURIDAD = "iber_plantillasid";
            public const string OWNING_BUSINESS_UNIT_ID = "owningbusinessunit";
            public const string BUSINESS_UNIT_ID = "businessunitid";
            public const string CREATED_ON = "createdon";
        }
        public struct CRMENTITIES
        {
            public const string BUSINESS_UNIT = "businessunit";
            public const string ACCOUNT = "account";
            public const string OPPORTUNITY = "opportunity";
            public const string QUOTE = "quote";
            public const string APPOINTMENT = "appointment";
            public const string QUOTEDETAIL = "quotedetail";
            public const string INCIDENT = "incident";
            public const string IBER_PLANTILLAS = "iber_plantillas";
        }
        public struct CONFIGPROPERTIES
        {
            public const string LIST_ENTITITES = "Lista Entidades";
            public const string BUSINESS_UNIT = "Unidad de Negocio";
            public const string LIST_TEMPLATES = "Lista Plantillas";
        }
        public struct QUERY
        {
            public const string ENTITY_GROUPS = "?$select=NivelPermiso/Title,NivelPermiso/Id,Accion/Title,Accion/Id, Accion/IdRolSP,Entidad/Title, Entidad/LogicalName,Entidad/Id,Id,Title, IdGroup&$expand=NivelPermiso,Accion,Entidad";
        }
        public struct SPROLEASSIGMENTS
        {
            public const string READ = "1073741826";
            public const string EDIT = "1073741830";
        }
        
    }
    public enum HttpRestMethod
    {
        POST,
        GET,
        DELETE,
        PATCH,
        MERGE,
        UPDATE
    }
}
