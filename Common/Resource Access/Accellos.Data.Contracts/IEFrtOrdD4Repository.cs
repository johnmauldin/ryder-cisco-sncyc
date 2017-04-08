using Accellos.Business.Entities;
using Accellos.Business.Entities.Query;
using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Data.Contracts
{
    /// <summary>
    /// Expresses behavior for querying the e_frt_ord_d4 table
    /// </summary>
    public interface IEFrtOrdD4Repository : IReadOnlyRepository<EFrtOrdD4>
    {
        IList<EFrtOrdD4> GetByExample(EFrtOrdD4Params example);
    }
}
