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
    [Export(typeof(IBatchQueryFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class BatchQueryFactory : IBatchQueryFactory
    {
        public T GetBatchQuery<T>() where T : IBatchQuery
        {
            // manually resolve type from the MEF Container, by passing in the Interface for T
            return ObjectBase.Container.GetExportedValue<T>();
        }

        
    }
}
