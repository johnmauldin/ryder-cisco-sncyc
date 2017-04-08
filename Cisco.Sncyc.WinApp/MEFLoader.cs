using Accellos.Data.Repositories;
using Cisco.Sncyc.Business.BusinessEngines;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cisco.Sncyc.WinApp
{
    class MEFLoader
    {
        public static CompositionContainer Init()
        {
            AggregateCatalog catalog = new AggregateCatalog();

            // e.g. go out to these assemblies and look for classes that have the Export attribute
            // you can give the catalog any class within the assembly
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(MCompHRepository).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(CiscoSnCycEngine).Assembly));

            CompositionContainer container = new CompositionContainer(catalog);

            return container;
        }
    }
}
