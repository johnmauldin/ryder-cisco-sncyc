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
    [Export(typeof(IMCustHRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //dont use Singleton, which is the default in MEF
    public class MCustHRepository : ReadOnlyRepositoryBase<MCustH, string>, IMCustHRepository
    {

        #region GetByExample
        public IList<MCustH> GetByExample(MCustHParams example)
        {
            var ctx = new AccellosContext();
            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT cust_code, cust_name, cust_stat, comp_code FROM m_cust_h  WHERE 1=1 ");

                var parameters = new List<OracleParameter>();

                if (!string.IsNullOrWhiteSpace(example.CustCode))
                {
                    sql.Append("AND cust_code = :1 ");
                    parameters.Add(new OracleParameter(":1", OracleDbType.Varchar2, example.CustCode, ParameterDirection.Input));
                }
                if (!string.IsNullOrWhiteSpace(example.CompCode))
                {
                    sql.Append("AND comp_code = :2 ");
                    parameters.Add(new OracleParameter(":2", OracleDbType.Varchar2, example.CompCode, ParameterDirection.Input));
                }

                IList<MCustH> entities = new List<MCustH>();

                OracleManager.ExecuteReader(cn, sql.ToString(), parameters,
                     (reader) => entities.Add(getEntityFromReader(reader))
                );

                return entities;
            }
        }
        #endregion

        #region Base
        protected override IEnumerable<MCustH> GetEntities(AccellosContext entityContext)
        {
            throw new NotImplementedException();
        }

        protected override MCustH GetEntity(AccellosContext entityContext, string id)
        {
            using (OracleConnection cn = (OracleConnection)entityContext.DbConnection)
            {
                cn.Open();

                string sql = @"SELECT cust_code, cust_name, cust_stat, comp_code 
FROM m_cust_h 
WHERE cust_code = :1";

                var parameters = new List<OracleParameter>{ 
                    new OracleParameter(":1", OracleDbType.Varchar2, id, ParameterDirection.Input)
                };

                MCustH entity = null;

                OracleManager.ExecuteReader(cn, sql, parameters,
                     (reader) => entity = getEntityFromReader(reader)
                );

                return entity;
            }
        }
        #endregion

        private MCustH getEntityFromReader(OracleDataReader reader)
        {
            var cust = new MCustH
                {
                    CustCode = reader.GetString(reader.GetOrdinal("cust_code")),
                    CustName = reader.GetString(reader.GetOrdinal("cust_name")),
                    CustStat = reader.GetString(reader.GetOrdinal("cust_stat")),
                    CompCode = reader.GetString(reader.GetOrdinal("comp_code"))
                };


            return cust;
        }

        
    }
}
