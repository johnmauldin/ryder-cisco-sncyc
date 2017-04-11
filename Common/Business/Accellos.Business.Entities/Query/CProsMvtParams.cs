using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Entities.Query
{
    public class CProsMvtParams
    {
        public string LocCode { get; set; }

        public string InvtLev1 { get; set; }

        public DateTime ProsTransDate { get; set; }

        public string ProsTransTp { get; set; }
        
        public string ProsCode { get; set; } 
    }
}
