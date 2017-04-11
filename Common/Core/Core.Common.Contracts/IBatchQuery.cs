using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Contracts
{
    public interface IBatchQuery
    {
        void Execute();
    }

    /// <summary>
    /// expresses behavior for performing operations that span multiple repositories, 
    /// like updating a batch of records
    /// </summary>
    public interface IBatchQuery<P> : IBatchQuery
        where P : IBatchQueryParams
    {
        void Execute(P parameters);
    }
}
