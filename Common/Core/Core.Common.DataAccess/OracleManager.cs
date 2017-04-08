using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.DataAccess
{
    public class OracleManager
    {

        public static int ExecuteScalar(OracleConnection cn, string sql, IList<OracleParameter> parameters)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();

            var cmd = new OracleCommand(sql, cn);
            foreach (var p in parameters) cmd.Parameters.Add(p);

            var result = cmd.ExecuteScalar();

            return Convert.ToInt32(result);
        }

        public static int ExecuteSql(OracleConnection cn, string sql, IList<OracleParameter> parameters)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();

            var cmd = new OracleCommand(sql, cn);
            foreach (var p in parameters) cmd.Parameters.Add(p);

            return cmd.ExecuteNonQuery();
        }

        public static void ExecuteReader(OracleConnection cn, string sql, 
            IList<OracleParameter> parameters, Action<OracleDataReader> code)
        {
            if (cn.State == ConnectionState.Closed) cn.Open();

            var cmd = new OracleCommand(sql, cn);
            cmd.InitialLONGFetchSize = -1;  //forces RAW column types to return, like m_loc.Id 

            if (parameters != null)
            {
                foreach (var p in parameters)
                    cmd.Parameters.Add(p);
            }
            using (var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
            {
                while (reader.Read())
                {
                    code.Invoke(reader);
                }
            }

        }
    }
}
