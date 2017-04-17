using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Entities
{
    public class MCustH : Entity<string>
    {
        public string CustCode { get; set; }

        public string CustName { get; set; }

        public string CustStat { get; set; }

        public string CompCode { get; set; }

        public bool IsActive { get { return this.CustStat == "A"; } } // not "D"

        public bool IsCisco { get { 
            return CustCode == "CISCOSYS" || 
                CustCode == "CISCOZB"; 
        } 
        }
    }
}
