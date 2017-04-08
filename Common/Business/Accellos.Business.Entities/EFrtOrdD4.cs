using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Entities
{
    /// <summary>
    /// this table has no single-column primary key
    /// </summary>
    public class EFrtOrdD4 : Entity
    {
        public string CompCode { get; set; }

        public int LoadNum { get; set; }

        public int OrdNum { get; set; }

        public string FrtTerCode { get; set; }
    }
}
