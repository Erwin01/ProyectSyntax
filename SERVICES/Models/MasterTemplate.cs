using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES.Models
{
    public class MasterTemplate: SPDefaultObject
    {
        public string Title{ get; set; }
        public string Plantilla { get; set; }
        public string Entidad { get; set; }
        public string Rol { get; set; }
        public string IdGrupo { get; set; }
    }
    public class MasterTemplateInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Plantilla { get; set; }
        public string Entidad { get; set; }
        public string Rol { get; set; }
        public string IdGrupo { get; set; }
    }
}
