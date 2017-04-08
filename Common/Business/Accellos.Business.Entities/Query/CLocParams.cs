using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Entities.Query
{
    public class CLocParams
    {
        public string LocCode { get; set; }

        public string CompCode { get; set; }

        public string CustCode { get; set; }

        public string InvtLev1 { get; set; } //item_code
    }
}
