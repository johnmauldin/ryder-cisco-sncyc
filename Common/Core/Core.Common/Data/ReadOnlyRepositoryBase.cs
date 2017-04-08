using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.Entity;
using System.Linq;
using Core.Common.Contracts;
using Core.Common.Utils;

namespace Core.Common.Data
{
    /// <summary>
    /// minimize the amount of code that is going to differ from one repo to another
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public abstract class ReadOnlyRepositoryBase<T, TId, U> : IReadOnlyRepository<T, TId>
        where T : Entity<TId>, new()
        where U : IDbContext, new()
        
    {
        protected abstract IEnumerable<T> GetEntities(U entityContext);

        protected abstract T GetEntity(U entityContext, TId id);
        //protected abstract T GetEntity(TId id);

        public IEnumerable<T> Get()
        {
            using (U entityContext = new U())
                return (GetEntities(entityContext)).ToArray().ToList();
        }

        public T Get(TId id)
        {
            using (U entityContext = new U())
                return GetEntity(entityContext, id);
        }
    }
}