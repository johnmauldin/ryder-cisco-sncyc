using Accellos.Business.Entities;
using Accellos.Business.Entities.Query;
using Accellos.Data.Contracts;
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
    [Export(typeof(ICProsMvtRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //dont use Singleton, which is the default in MEF
    public class CProsMvtRepository : ReadOnlyRepositoryBase<CProsMvt, string>, ICProsMvtRepository
    {

        #region GetByExample
        public IList<CProsMvt> GetByExample(CProsMvtParams example)
        {
            var ctx = new AccellosContext();
            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append(@"SELECT loc_code, pros_code, invt_lev1, pros_trans_date, pros_trans_tp 
FROM c_pros_mvt WHERE 1=1 ");

                var parameters = new List<OracleParameter>();

                if (!string.IsNullOrWhiteSpace(example.LocCode))
                {
                    sql.Append("AND loc_code = :1 ");
                    parameters.Add(new OracleParameter(":1", OracleDbType.Varchar2, example.LocCode, ParameterDirection.Input));
                }
                
                if (!string.IsNullOrWhiteSpace(example.InvtLev1))
                {
                    sql.Append("AND invt_lev1 = :2 ");
                    parameters.Add(new OracleParameter(":2", OracleDbType.Varchar2, example.InvtLev1, ParameterDirection.Input));
                }

                if (!string.IsNullOrWhiteSpace(example.ProsCode))
                {
                    sql.Append("AND pros_code = :3 ");
                    parameters.Add(new OracleParameter(":3", OracleDbType.Varchar2, example.ProsCode, ParameterDirection.Input));
                }

                var entities = new List<CProsMvt>();

                OracleManager.ExecuteReader(cn, sql.ToString(), parameters,
                     (reader) => entities.Add(getEntityFromReader(reader))
                );

                return entities;
            }
        }
        #endregion

        private CProsMvt getEntityFromReader(OracleDataReader reader)
        {
            var entity = new CProsMvt
            {                
                LocCode = reader.GetString(reader.GetOrdinal("loc_code")),
                ProsCode = reader.GetString(reader.GetOrdinal("pros_code")),
                ProsTransTp = reader.GetString(reader.GetOrdinal("pros_trans_tp")),
                InvtLev1 = reader.GetString(reader.GetOrdinal("invt_lev1")),
                ProsTransDate = reader.GetDateTime(reader.GetOrdinal("pros_trans_date"))
            };


            return entity;
        }

        protected override IEnumerable<CProsMvt> GetEntities(AccellosContext entityContext)
        {
            throw new NotImplementedException();
        }

        protected override CProsMvt GetEntity(AccellosContext entityContext, string id)
        {
            throw new NotImplementedException();
        }
    }
}
