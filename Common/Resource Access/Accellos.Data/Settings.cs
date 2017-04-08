using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Data
{
    class Settings
    {
        public static string AccellosConnString { get { return ConfigurationManager.ConnectionStrings["Accellos"].ConnectionString; } }
    }
}
