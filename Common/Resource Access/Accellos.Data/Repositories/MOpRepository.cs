using Accellos.Business.Entities;
using Accellos.Business.Entities.Query;
using Accellos.Data.Contracts;
using Core.Common.Contracts;
using Core.Common.DataAccess;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Data.Repositories
{
    [Export(typeof(IMOpRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //dont use Singleton, which is the default in MEF
    public class MOpRepository : ReadOnlyRepositoryBase<MOp, string>, IMOpRepository
    {

        #region Base
        protected override IEnumerable<MOp> GetEntities(AccellosContext entityContext)
        {
            throw new NotImplementedException();
        }

        protected override MOp GetEntity(AccellosContext entityContext, string id)
        {
            using (OracleConnection cn = (OracleConnection)entityContext.DbConnection)
            {
                cn.Open();

                string sql = @"SELECT op_code, op_name, op_stat, op_pword, comp_code
FROM m_op 
WHERE op_code = :1";

                var parameters = new List<OracleParameter>{ 
                    new OracleParameter(":1", OracleDbType.Varchar2, id, ParameterDirection.Input)
                };

                MOp op = null;

                OracleManager.ExecuteReader(cn, sql, parameters, 
                     (reader) => op = getEntityFromReader(reader)
                );

                return op;
            }
        }
        #endregion

        private MOp getEntityFromReader(OracleDataReader reader) {

            var bytes = reader.GetOracleBinary(reader.GetOrdinal("op_pword")).Value;
            var hex = ByteArrayToString(bytes);

            //var pw = Encoding.GetEncoding("UTF-8").GetString(bytes);
            //var pw = Convert.ToBase64String(bytes);

            var entity = new MOp
                {
                    OpCode = reader.GetString(reader.GetOrdinal("op_code")),
                    CompCode = reader.GetString(reader.GetOrdinal("comp_code")),
                    OPPword = hex,
                    OpStat = reader.GetString(reader.GetOrdinal("op_stat"))
                };
                
                return entity;
        }

        static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        
    }
}
