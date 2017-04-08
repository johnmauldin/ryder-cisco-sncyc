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
    /// Expresses behavior for querying the m_item_h table
    /// </summary>
    public interface IMItemHRepository : IReadOnlyRepository<MItemH, string>
    {
        IList<MItemH> GetByExample(MItemHParams example);
    }
}
