using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SERVICES.Models
{
    public class MasterEntityGroup
    {
        public string Title { get; set; }
        public MasterPermission NivelPermiso { get; set; }
        public MasterEntity Entidad { get; set; }
        public MasterAction Accion { get; set; }
        public string IdGroup { get; set; }
        public string Id { get; set; }
    }
    public class MasterEntityGroupInfo: SPDefaultObject
    {
        public string Title { get; set; }
        public string IdGroup { get; set; }
    }
}
