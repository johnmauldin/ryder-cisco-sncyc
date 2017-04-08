using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Entities
{
    public class LRyderCiscoSncycCnt : IIdentifiableEntity
    {
        public string CustCode { get; set; }

        public string LocCode { get; set; }

        public string ItemCode { get; set; }

        public string Serial { get; set; }

        public DateTime ProsDateTime { get; set; }

        public string UserName { get; set; }

        public string BulkItem { get; set; }

        public string ItemType { get; set; }
    }
}
