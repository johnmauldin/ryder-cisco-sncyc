using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Core.Common.Contracts;
using Core.Common.Core;

namespace Accellos.Data
{
    [Export(typeof(IReadOnlyRepositoryFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReadOnlyRepositoryFactory : IReadOnlyRepositoryFactory
    {
        public T GetDataRepository<T>() where T : IReadOnlyRepository
        {
            // manually resolve type from the MEF Container, by passing in the Interface for T
            return ObjectBase.Container.GetExportedValue<T>();
        }

        
    }
}
