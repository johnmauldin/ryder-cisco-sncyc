using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Contracts
{
    public interface IAccountOwnedEntity
    {
        // specifies the Id of the user account
        int OwnerAccountId { get; }
    }
}
