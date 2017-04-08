using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Entities
{
    public class MLoc : AssignableIdEntity<string>
    {
        public string LocCode { get; set; }

        public string CompCode { get; set; }

        public string LocDes { get; set; }

        public string LocStat { get; set; }

        public bool IsActive { get { return this.LocStat == "A"; } } // not "D"
    }
}
