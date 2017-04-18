using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Entities
{
    public class MItemH : Entity<string>
    {
        public string CompCode { get; set; }

        public string CustCode { get; set; }

        public string ItemCode { get; set; }

        public string ItemDes1 { get; set; }

        public string ItemStat { get; set; }

        public string ProsProfCode { get; set; }
        

        public bool IsActive { get { return this.ItemStat == "A"; } } // not "D"

        public bool Serialized { get { return this.ProsProfCode.ToUpper() == "SN"; } } 
    }
}
