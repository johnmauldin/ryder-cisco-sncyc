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
    [Export(typeof(ILRyderCiscoSncycCntRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //dont use Singleton, which is the default
    public class LRyderCiscoSncycCntRepository : DataRepositoryBase<LRyderCiscoSncycCnt, string>, ILRyderCiscoSncycCntRepository
    {
        #region GetByExample
        public IList<LRyderCiscoSncycCnt> GetByExample(LRyderCiscoSncycCntParams example)
        {
            var ctx = new AccellosContext();
            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append(@"SELECT cust_code, loc_code, item_code, serial, pros_date_time, 
username, bulk_item, item_type
FROM l_ryder_cisco_sncyc_cnt  WHERE 1=1 ");

                var parameters = new List<OracleParameter>();

                if (!string.IsNullOrWhiteSpace(example.CustCode))
                {
                    sql.Append("AND cust_code = :1 ");
                    parameters.Add(new OracleParameter(":1", OracleDbType.Varchar2, example.CustCode, ParameterDirection.Input));
                }

                if (!string.IsNullOrWhiteSpace(example.LocCode))
                {
                    sql.Append("AND loc_code = :2 ");
                    parameters.Add(new OracleParameter(":2", OracleDbType.Varchar2, example.LocCode, ParameterDirection.Input));
                }

                if (!string.IsNullOrWhiteSpace(example.ItemCode))
                {
                    sql.Append("AND item_code = :3 ");
                    parameters.Add(new OracleParameter(":3", OracleDbType.Varchar2, example.ItemCode, ParameterDirection.Input));
                }

                if (!string.IsNullOrWhiteSpace(example.Serial))
                {
                    sql.Append("AND serial = :4 ");
                    parameters.Add(new OracleParameter(":4", OracleDbType.Varchar2, example.Serial, ParameterDirection.Input));
                }

                IList<LRyderCiscoSncycCnt> entities = new List<LRyderCiscoSncycCnt>();

                OracleManager.ExecuteReader(cn, sql.ToString(), parameters,
                     (reader) => entities.Add(getEntityFromReader(reader))
                );

                return entities;
            }
        }
#endregion

        #region AddEntity
        protected override LRyderCiscoSncycCnt AddEntity(AccellosContext entityContext, LRyderCiscoSncycCnt entity)
        {
            string sql = @"INSERT INTO l_ryder_cisco_sncyc_cnt (
cust_code,
loc_code,
item_code,
serial,
pros_date_time,
username,
bulk_item,
item_type) VALUES
(:1, :2, :3, :4, :5, :6, :7, :8)";

            IList<OracleParameter> parameters = new List<OracleParameter>
            {
                new OracleParameter(":1", OracleDbType.Varchar2, entity.CustCode, ParameterDirection.Input),
                new OracleParameter(":2", OracleDbType.Varchar2, entity.LocCode, ParameterDirection.Input),
                new OracleParameter(":3", OracleDbType.Varchar2, entity.ItemCode, ParameterDirection.Input),
                new OracleParameter(":4", OracleDbType.Varchar2, entity.Serial, ParameterDirection.Input),
                new OracleParameter(":5", OracleDbType.Date, entity.ProsDateTime, ParameterDirection.Input),
                new OracleParameter(":6", OracleDbType.Varchar2, entity.UserName, ParameterDirection.Input),
                new OracleParameter(":7", OracleDbType.Varchar2, entity.BulkItem, ParameterDirection.Input),
                new OracleParameter(":8", OracleDbType.Varchar2, entity.ItemType, ParameterDirection.Input)
                
            };

            using (var cn = (OracleConnection)entityContext.DbConnection)
            {
                
                var result = OracleManager.ExecuteSql(cn, sql, parameters);
                return entity;
            }
            
        }
        #endregion

        #region GetSerialCount
        public int GetSerialCount(string custCode, string locCode, string itemCode)
        {
            var ctx = new AccellosContext();

            string sql = @"SELECT COUNT(serial) cnt FROM l_ryder_cisco_sncyc_cnt
WHERE cust_code = :1 AND loc_code = :2 AND item_code = :3";
            IList<OracleParameter> parameters = new List<OracleParameter>
            {
                new OracleParameter(":1", OracleDbType.Varchar2, custCode, ParameterDirection.Input),
                new OracleParameter(":2", OracleDbType.Varchar2, locCode, ParameterDirection.Input),
                new OracleParameter(":3", OracleDbType.Varchar2, itemCode, ParameterDirection.Input)
            };

            using (var cn = (OracleConnection)ctx.DbConnection)
            {
                return OracleManager.ExecuteScalar(cn, sql, parameters);
            }
        }
        #endregion

        protected override LRyderCiscoSncycCnt UpdateEntity(AccellosContext entityContext, LRyderCiscoSncycCnt entity)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<LRyderCiscoSncycCnt> GetEntities(AccellosContext entityContext)
        {
            throw new NotImplementedException();
        }

        protected override LRyderCiscoSncycCnt GetEntity(AccellosContext entityContext, int id)
        {
            throw new NotImplementedException();
        }

        private LRyderCiscoSncycCnt getEntityFromReader(OracleDataReader reader)
        {
            var entity = new LRyderCiscoSncycCnt
            {
                CustCode = reader.GetString(reader.GetOrdinal("cust_code")),
                ItemCode = reader.GetString(reader.GetOrdinal("item_code")),
                LocCode = reader.GetString(reader.GetOrdinal("loc_code")),
                Serial = reader.GetString(reader.GetOrdinal("serial")),
                ProsDateTime = reader.GetDateTime(reader.GetOrdinal("pros_date_time")),
                UserName = reader.GetString(reader.GetOrdinal("username")),
                BulkItem = reader.GetString(reader.GetOrdinal("bulk_item")),
                ItemType = reader.GetString(reader.GetOrdinal("item_type"))
            };

            return entity;
        }

    }
}
