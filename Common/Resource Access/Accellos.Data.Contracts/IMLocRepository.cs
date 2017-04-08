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
    /// Expresses behavior for querying the m_loc table
    /// </summary>
    public interface IMLocRepository : IReadOnlyRepository<MLoc, string>
    {
        IList<MLoc> GetByExample(MLocParams example);
    }
}
