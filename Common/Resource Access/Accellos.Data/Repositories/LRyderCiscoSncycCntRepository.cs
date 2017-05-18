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

#region Reports

        #region GetScannedQtyMismatches
        public IList<ScannedQtyMismatchModel> GetScannedQtyMismatches(string compCode, string custCode, string locCode)
        {
            var ctx = new AccellosContext();
            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                StringBuilder sqlFormat = new StringBuilder();
                sqlFormat.Append(@"SELECT 
 cyc.cust_code, 
 cyc.loc_code, 
 cyc.item_code, 
 loc.invt_lev1,
 cyc.cycle_count_qty,
 loc.on_hand_qty
FROM (select cust_code, loc_code, item_code, count(serial) cycle_count_qty 
  from l_ryder_cisco_sncyc_cnt
  group by cust_code, loc_code, item_code
 ) cyc
JOIN c_loc loc ON loc.comp_code = '{0}' 
 AND cyc.cust_code = loc.cust_code 
 AND cyc.loc_code = loc.loc_code 
 AND cyc.item_code = loc.invt_lev1 
WHERE 1=1 
AND loc.on_hand_qty <> cyc.cycle_count_qty ");

                var parameters = new List<OracleParameter>();

                sqlFormat.Append("AND cyc.cust_code = :1 ");
                parameters.Add(new OracleParameter(":1", OracleDbType.Varchar2, custCode, ParameterDirection.Input));

                sqlFormat.Append("AND cyc.loc_code = :2 ");
                parameters.Add(new OracleParameter(":2", OracleDbType.Varchar2, locCode, ParameterDirection.Input));

                IList<ScannedQtyMismatchModel> entities = new List<ScannedQtyMismatchModel>();

                var sql = string.Format(sqlFormat.ToString(), compCode);

                OracleManager.ExecuteReader(cn, sql, parameters,
                     (reader) => entities.Add(new ScannedQtyMismatchModel
                     {
                         CustomerCode = reader.GetString(reader.GetOrdinal("cust_code")),
                         LocationCode = reader.GetString(reader.GetOrdinal("loc_code")),
                         ItemCode = reader.GetString(reader.GetOrdinal("item_code")),
                         CycleCountQty = reader.GetInt32(reader.GetOrdinal("cycle_count_qty")),
                         LocOnHandQty = reader.GetInt32(reader.GetOrdinal("on_hand_qty"))
                     }
                   )
                );

                return entities;
            }
        }
        #endregion

        #region GetLocationsCounted
        public IList<LocationCountedModel> GetLocationsCounted(string compCode, string custCode)
        {
            var ctx = new AccellosContext();
            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                StringBuilder sqlFormat = new StringBuilder();
                sqlFormat.Append(@"SELECT 
 loc.cust_code, 
 loc.loc_code, 
 loc.invt_lev1 as item_code, 
 cyc.cycle_count_qty
FROM (select l.comp_code, l.cust_code, l.loc_code, l.invt_lev1, l.on_hand_qty 
       from c_loc l
       join m_item_h p 
         on l.comp_code = p.comp_code 
         and l.cust_code = p.cust_code
         and l.invt_lev1 = p.item_code
         where p.item_stat = 'A'
         and p.pros_prof_code = 'SN'
         and l.on_hand_qty > 0) loc
JOIN (select cust_code, loc_code, item_code, count(serial) cycle_count_qty 
  from l_ryder_cisco_sncyc_cnt
  group by cust_code, loc_code, item_code
 ) cyc ON loc.comp_code = '{0}' 
 AND cyc.cust_code = loc.cust_code 
 AND cyc.loc_code = loc.loc_code 
 AND cyc.item_code = loc.invt_lev1 
WHERE 1=1 ");

                var parameters = new List<OracleParameter>();

                sqlFormat.Append("AND loc.cust_code = :1 ");
                parameters.Add(new OracleParameter(":1", OracleDbType.Varchar2, custCode, ParameterDirection.Input));

                IList<LocationCountedModel> entities =
                    new List<LocationCountedModel>();

                var sql = string.Format(sqlFormat.ToString(), compCode);

                OracleManager.ExecuteReader(cn, sql, parameters,
                     (reader) => entities.Add(new LocationCountedModel
                     {
                         CustomerCode = reader.GetString(reader.GetOrdinal("cust_code")),
                         LocationCode = reader.GetString(reader.GetOrdinal("loc_code")),
                         ItemCode = reader.GetString(reader.GetOrdinal("item_code")),
                         SerialCount = reader.GetInt32(reader.GetOrdinal("cycle_count_qty"))
                     }
                   )
                );

                return entities;
            }
        }
        #endregion

        #region GetLocationsNotCounted
        public IList<LocationNotCountedModel> GetLocationsNotCounted(string compCode, string custCode)
        {
            var ctx = new AccellosContext();
            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                StringBuilder sqlFormat = new StringBuilder();
                sqlFormat.Append(@"SELECT 
 loc.cust_code, 
 loc.loc_code, 
 loc.invt_lev1 as item_code, 
 loc.on_hand_qty
FROM (select l.comp_code, l.cust_code, l.loc_code, l.invt_lev1, l.on_hand_qty 
       from c_loc l
       join m_item_h p 
         on l.comp_code = p.comp_code 
         and l.cust_code = p.cust_code
         and l.invt_lev1 = p.item_code
         where p.item_stat = 'A'
         and p.pros_prof_code = 'SN'
         and l.on_hand_qty > 0) loc
LEFT JOIN (select cust_code, loc_code, item_code
  from l_ryder_cisco_sncyc_cnt
  group by cust_code, loc_code, item_code
 ) cyc ON loc.comp_code = '{0}' 
 AND cyc.cust_code = loc.cust_code 
 AND cyc.loc_code = loc.loc_code 
 AND cyc.item_code = loc.invt_lev1 
WHERE 1=1 
AND cyc.item_code IS NULL ");

                var parameters = new List<OracleParameter>();

                sqlFormat.Append("AND loc.cust_code = :1 ");
                parameters.Add(new OracleParameter(":1", OracleDbType.Varchar2, custCode, ParameterDirection.Input));

                IList<LocationNotCountedModel> entities =
                    new List<LocationNotCountedModel>();

                var sql = string.Format(sqlFormat.ToString(), compCode);

                OracleManager.ExecuteReader(cn, sql, parameters,
                     (reader) => entities.Add(new LocationNotCountedModel
                     {
                         CustomerCode = reader.GetString(reader.GetOrdinal("cust_code")),
                         LocationCode = reader.GetString(reader.GetOrdinal("loc_code")),
                         ItemCode = reader.GetString(reader.GetOrdinal("item_code")),
                         OnHandQty = reader.GetInt32(reader.GetOrdinal("on_hand_qty"))
                     }
                   )
                );

                return entities;
            }
        }
        #endregion

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
