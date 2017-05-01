using Accellos.Business.Contracts;
using Accellos.Business.Entities;
using Accellos.Business.Entities.Query;
using Accellos.Data.Contracts;
using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cisco.Sncyc.Business.BusinessEngines
{
    [Export(typeof(ILoginEngine))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //dont use Singleton, which is the default in MEF
    public class LoginEngine : ILoginEngine
    {
        [Import]
        private IReadOnlyRepositoryFactory _readOnlyRepositoryFactory = null;

        public LoginEngine()
        {

        }

        public LoginEngine(IReadOnlyRepositoryFactory readOnlyRepositoryFactory)
        {
            _readOnlyRepositoryFactory = readOnlyRepositoryFactory;
        }

        public void Authenticate(string opcode, string password)
        {
            var repo = _readOnlyRepositoryFactory.GetDataRepository<IMOpRepository>();

            var user = repo.Get(opcode.ToUpper());

            if (user == null)
                throw new ArgumentException("Invalid user");

            if (CiscoSnCycEngine.IsAdminUser(opcode))
            {
                var setting = ConfigurationManager.AppSettings["adminPassword"];
                if (password != setting)
                    throw new ArgumentException("Invalid Password");
            }

        }


    }
}
