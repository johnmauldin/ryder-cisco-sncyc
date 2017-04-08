using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Entities
{
    public class CLoc : Entity<string>
    {
        public string LocCode { get; set; }

        public string CompCode { get; set; }

        public string CustCode { get; set; }

        public int OnHandQty { get; set; }

        public string InvtAccess { get; set; }

        public string InvtLev1 { get; set; }

        public string HoldCode { get; set; } 
        
    }
}
