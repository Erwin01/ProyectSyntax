namespace SERVICES
{
    using System;
    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Query;
    using Microsoft.Xrm.Tooling.Connector;
    using System.Linq;
    using System.Collections.Generic;

    public class CRMEntityService: CRMBaseService
    {
        public CRMEntityService(): base()
        {

        }
        public ICollection<Entity> Get(QueryExpression query)
        {
            var request = ProxyClient.RetrieveMultiple(query);
            var entities = request.Entities;
            while (request.MoreRecords)
            {
                query.PageInfo.PageNumber += 1;
                request = ProxyClient.RetrieveMultiple(query);
                entities.AddRange(request.Entities);
            }
            return entities.ToList();
        }
        public Entity Get(
            Guid idEntity, 
            string entityLogicalName, 
            ICollection<string> columns = null, 
            FilterExpression customFilter = null, List<LinkEntity> entities = null, 
            string referenceIdName = "")
        {
            var filter = customFilter == null?  new FilterExpression(): customFilter;
            filter.AddCondition(string.Format(Constants.FORMATS.CRMCOLUMNS.COLUMN_ID, string.IsNullOrEmpty(referenceIdName) ? entityLogicalName.ToLowerInvariant(): referenceIdName), ConditionOperator.Equal, idEntity);
            QueryExpression query = new QueryExpression
            {
                EntityName = entityLogicalName.ToLowerInvariant(),
                ColumnSet = columns != null ? new ColumnSet(columns.ToArray()) : new ColumnSet(true) ,
                Criteria = filter,
                TopCount = 1
            };
            if(entities != null)
            {
                entities.ForEach(e => query.LinkEntities.Add(e));
            }
            var entity = this.ProxyClient.RetrieveMultiple(query).Entities.ToList().FirstOrDefault();
            return entity;
        }
        public ICollection<Entity> Get(
            string entityLogicalName, 
            ICollection<string> columns = null,
            //int topCount = 5000,
            int topCount = 50000,
            FilterExpression filter = null)
        {
            QueryExpression query = new QueryExpression
            {
                EntityName = entityLogicalName.ToLowerInvariant(),
                ColumnSet = columns != null ? new ColumnSet(columns.ToArray()): new ColumnSet(true),
                TopCount = topCount
            };
            if(filter != null)
            {
                query.Criteria = filter;
            }
            return this.ProxyClient.RetrieveMultiple(query).Entities.ToList();
        }

        public Guid Create(string entityName, Dictionary<string, object> attributes)
        {
            Entity record = new Entity(entityName);
            record.Attributes.AddRange(attributes.ToList());
            return ProxyClient.Create(record);
        }
    }
}
