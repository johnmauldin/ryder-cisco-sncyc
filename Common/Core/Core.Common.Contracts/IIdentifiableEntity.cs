using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Contracts
{
    /// <summary>
    /// Could use [Key] attribute of EntityFramework, but this mechanism avoids use of Reflection
    /// </summary>
    public interface IIdentifiableEntity
    {
        //int EntityId { get; set; }  // explicity establish which column is the primary key
    }

    
}
