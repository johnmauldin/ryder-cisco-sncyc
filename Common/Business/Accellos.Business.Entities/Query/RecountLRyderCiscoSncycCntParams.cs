using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Entities.Query
{
    /// <summary>
    /// parameters to move rows in l_ryder_cisco_sncyc_cnt
    /// </summary>
    public class RecountLRyderCiscoSncycCntParams : IBatchQueryParams
    {
        public string CustCode { get; set; }

        public string LocCode { get; set; }

        public string ItemCode { get; set; }

      
    }
}
