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
    /// Expresses behavior for querying the c_loc table
    /// </summary>
    public interface ICLocRepository : IReadOnlyRepository<CLoc, string>
    {
        IList<CLoc> GetByExample(CLocParams example);
    }
}
