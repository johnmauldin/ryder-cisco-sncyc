using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Entities
{
    public class MOp : Entity<string>
    {
        public string OpCode { get; set; }

        public string OPPword { get; set; }

        public string OpStat { get; set; }

        public string CompCode { get; set; }

        public bool IsActive { get { return this.OpStat == "A"; } }
    }
}
