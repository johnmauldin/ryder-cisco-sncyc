using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Contracts
{
    public interface IReadOnlyRepository
    {

    }

    public interface IReadOnlyRepository<T, TId> : IReadOnlyRepository
        where T : Entity<TId>, new()
    {
        IEnumerable<T> Get();

        T Get(TId id);
    }

    public interface IReadOnlyRepository<T> : IReadOnlyRepository<T, int>
        where T : Entity, new()
    {
        
        
    }

    
}
