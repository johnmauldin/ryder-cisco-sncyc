using Accellos.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Contracts
{
    public interface ILoginEngine
    {
        void Authenticate(string opcode, string password);

    }
}
