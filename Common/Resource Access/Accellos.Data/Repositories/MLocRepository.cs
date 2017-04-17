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
    [Export(typeof(IMLocRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //dont use Singleton, which is the default in MEF
    public class MLocRepository : ReadOnlyRepositoryBase<MLoc, string>, IMLocRepository
    {

        #region GetByExample
        public IList<MLoc> GetByExample(MLocParams location)
        {
            var ctx = new AccellosContext();
            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT comp_code, loc_code, loc_des, loc_stat FROM m_loc WHERE 1=1 ");

                var parameters = new List<OracleParameter>();

                if (!string.IsNullOrWhiteSpace(location.CompCode))
                {
                    sql.Append("AND comp_code = :1 ");
                    parameters.Add(new OracleParameter(":1", OracleDbType.Varchar2, location.CompCode, ParameterDirection.Input));
                }
                if (!string.IsNullOrWhiteSpace(location.LocCode))
                {
                    sql.Append("AND loc_code = :2 ");
                    parameters.Add(new OracleParameter(":2", OracleDbType.Varchar2, location.LocCode, ParameterDirection.Input));
                }

                var locations = new List<MLoc>();

                OracleManager.ExecuteReader(cn, sql.ToString(), parameters,
                     (reader) => locations.Add(getEntityFromReader(reader))
                );

                return locations;
            }
        }
        #endregion

        #region Base
        protected override IEnumerable<MLoc> GetEntities(AccellosContext entityContext)
        {
            throw new NotImplementedException();
        }

        protected override MLoc GetEntity(AccellosContext entityContext, string id)
        {
            using (OracleConnection cn = (OracleConnection)entityContext.DbConnection)
            {
                cn.Open();

                string sql = @"SELECT comp_code, loc_code, loc_des, loc_stat 
FROM m_loc 
WHERE loc_code = :1";

                var parameters = new List<OracleParameter>{ 
                    new OracleParameter(":1", OracleDbType.Varchar2, id, ParameterDirection.Input)
                };

                MLoc loc = null;

                OracleManager.ExecuteReader(cn, sql, parameters, 
                     (reader) => loc = getEntityFromReader(reader)
                );

                return loc;
            }
        }
        #endregion

        private MLoc getEntityFromReader(OracleDataReader reader) {
            var loc = new MLoc
                {
                    CompCode = reader.GetString(reader.GetOrdinal("comp_code")),
                    LocCode = reader.GetString(reader.GetOrdinal("loc_code")),
                    LocDes = reader.IsDBNull(reader.GetOrdinal("loc_des")) ? "" : reader.GetString(reader.GetOrdinal("loc_des")),
                    LocStat = reader.GetString(reader.GetOrdinal("loc_stat"))
                };

                return loc;
        }

        
    }
}
