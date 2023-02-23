using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES.Models
{
    public class DocumentTree
    {
        [JsonProperty(PropertyName ="data")]
        public ICollection<TreeNode> Data { get; set; }
    }
    public class TreeNode
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "nodes")]
        public ICollection<TreeNode> Nodes { get; set; }

        public TreeNode()
        {
            this.Nodes = new List<TreeNode>();
        }
    }
}
