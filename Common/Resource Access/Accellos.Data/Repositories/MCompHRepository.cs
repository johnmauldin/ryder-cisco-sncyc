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
    [Export(typeof(IMCompHRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //dont use Singleton, which is the default in MEF
    public class MCompHRepository : ReadOnlyRepositoryBase<MCompH, string>, IMCompHRepository
    {

        #region GetByExample
        public IList<MCompH> GetByExample(MCompHParams example)
        {
            var ctx = new AccellosContext();
            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT comp_code, comp_name, global_code, comp_stat FROM m_comp_h  WHERE 1=1 ");

                var parameters = new List<OracleParameter>();

                if (!string.IsNullOrWhiteSpace(example.CompCode))
                {
                    sql.Append("AND comp_code = :1 ");
                    parameters.Add(new OracleParameter(":1", OracleDbType.Varchar2, example.CompCode, ParameterDirection.Input));
                }
                
                IList<MCompH> companies = new List<MCompH>();

                OracleManager.ExecuteReader(cn, sql.ToString(), parameters,
                     (reader) => companies.Add(getEntityFromReader(reader))
                );

                return companies;
            }
        }
        #endregion

        #region Base
        protected override IEnumerable<MCompH> GetEntities(AccellosContext entityContext)
        {
            throw new NotImplementedException();
        }

        protected override MCompH GetEntity(AccellosContext entityContext, string id)
        {
            using (OracleConnection cn = (OracleConnection)entityContext.DbConnection)
            {
                cn.Open();

                string sql = @"SELECT comp_code, comp_name, global_code, comp_stat 
FROM m_comp_h 
WHERE comp_code = :1";

                var parameters = new List<OracleParameter>{ 
                    new OracleParameter(":1", OracleDbType.Varchar2, id, ParameterDirection.Input)
                };

                MCompH company = null;

                OracleManager.ExecuteReader(cn, sql, parameters,
                     (reader) => company = getEntityFromReader(reader)
                );

                return company;
            }
        }
        #endregion

        private MCompH getEntityFromReader(OracleDataReader reader)
        {
            var company = new MCompH
                {
                    CompCode = reader.GetString(reader.GetOrdinal("comp_code")),
                    CompName = reader.GetString(reader.GetOrdinal("comp_name")),
                    CompStat = reader.GetString(reader.GetOrdinal("comp_stat")),
                    GlobalCode = reader.SafeGetString(reader.GetOrdinal("global_code"))
                };

                
            return company;
        }

        
    }
}
