using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Contracts
{
    public interface IBatchQueryFactory
    {
        T GetBatchQuery<T>() where T : IBatchQuery;

        //T GetBatchQuery<T, P>() 
        //    where T : IBatchQuery<P>
        //    where P : IBatchQueryParams;

    }
}
