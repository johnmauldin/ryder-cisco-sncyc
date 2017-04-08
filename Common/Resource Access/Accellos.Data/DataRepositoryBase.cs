using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Common.Contracts;
using Core.Common.Data;
using Core.Common.Core;

namespace Accellos.Data
{
    public abstract class DataRepositoryBase<T, TId> : Core.Common.Data.DataRepositoryBase<T, AccellosContext>
        where T : class, IIdentifiableEntity, new()
    {
    }

}
