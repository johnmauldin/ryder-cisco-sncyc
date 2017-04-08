using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Entities
{
    public class MCompH : Entity<string>
    {
        public string CompCode { get; set; }

        public string CompName { get; set; }

        public string CompStat { get; set; }

        public string GlobalCode { get; set; }

        public bool IsActive { get { return this.CompStat == "A"; } } // not "D"

    }
}
