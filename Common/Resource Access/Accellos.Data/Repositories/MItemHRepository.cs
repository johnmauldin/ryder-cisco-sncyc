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
    [Export(typeof(IMItemHRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //dont use Singleton, which is the default in MEF
    public class MItemHRepository : ReadOnlyRepositoryBase<MItemH, string>, IMItemHRepository
    {

        #region GetByExample
        public IList<MItemH> GetByExample(MItemHParams example)
        {
            var ctx = new AccellosContext();
            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append(@"SELECT cust_code, item_code, item_stat, comp_code, 
item_des1, pros_prof_code FROM m_item_h  WHERE 1=1 ");

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

                if (!string.IsNullOrWhiteSpace(example.ItemCode))
                {
                    sql.Append("AND item_code = :3 ");
                    parameters.Add(new OracleParameter(":3", OracleDbType.Varchar2, example.ItemCode, ParameterDirection.Input));
                }

                IList<MItemH> entities = new List<MItemH>();

                OracleManager.ExecuteReader(cn, sql.ToString(), parameters,
                     (reader) => entities.Add(getEntityFromReader(reader))
                );

                return entities;
            }
        }
        #endregion

        #region Base
        protected override IEnumerable<MItemH> GetEntities(AccellosContext entityContext)
        {
            throw new NotImplementedException();
        }

        protected override MItemH GetEntity(AccellosContext entityContext, string id)
        {
            using (OracleConnection cn = (OracleConnection)entityContext.DbConnection)
            {
                cn.Open();

                string sql = @"SELECT cust_code, item_code, item_stat, comp_code, item_des1, pros_prof_code  
FROM m_item_h 
WHERE item_code = :1";

                var parameters = new List<OracleParameter>{ 
                    new OracleParameter(":1", OracleDbType.Varchar2, id, ParameterDirection.Input)
                };

                MItemH entity = null;

                OracleManager.ExecuteReader(cn, sql, parameters,
                     (reader) => entity = getEntityFromReader(reader)
                );

                return entity;
            }
        }
        #endregion

        private MItemH getEntityFromReader(OracleDataReader reader)
        {
            var entity = new MItemH
                {
                    CustCode = reader.GetString(reader.GetOrdinal("cust_code")),
                    ItemCode = reader.GetString(reader.GetOrdinal("item_code")),
                    ItemStat = reader.GetString(reader.GetOrdinal("item_stat")),
                    CompCode = reader.GetString(reader.GetOrdinal("comp_code")),
                    ItemDes1 = reader.GetString(reader.GetOrdinal("item_des1")),
                    ProsProfCode = reader.GetString(reader.GetOrdinal("pros_prof_code"))
                     
                };


            return entity;
        }

        
    }
}
