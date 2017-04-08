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
    /// Expresses behavior for querying the m_cust_h table
    /// </summary>
    public interface IMCustHRepository : IReadOnlyRepository<MCustH, string>
    {
        
    }
}
