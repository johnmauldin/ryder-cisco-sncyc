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
    [Export(typeof(ICLocRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //dont use Singleton, which is the default in MEF
    public class CLocRepository : ReadOnlyRepositoryBase<CLoc, string>, ICLocRepository
    {

        #region GetByExample
        public IList<CLoc> GetByExample(CLocParams location)
        {
            var ctx = new AccellosContext();
            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append(@"SELECT comp_code, loc_code, cust_code, on_hand_qty, invt_access, invt_lev1, hold_code 
FROM c_loc WHERE 1=1 ");

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
                if (!string.IsNullOrWhiteSpace(location.CustCode))
                {
                    sql.Append("AND cust_code = :3 ");
                    parameters.Add(new OracleParameter(":3", OracleDbType.Varchar2, location.CustCode, ParameterDirection.Input));
                }
                if (!string.IsNullOrWhiteSpace(location.InvtLev1))
                {
                    sql.Append("AND invt_lev1 = :4 ");
                    parameters.Add(new OracleParameter(":4", OracleDbType.Varchar2, location.InvtLev1, ParameterDirection.Input));
                }

                var locations = new List<CLoc>();

                OracleManager.ExecuteReader(cn, sql.ToString(), parameters,
                     (reader) => locations.Add(getEntityFromReader(reader))
                );

                return locations;
            }
        }
        #endregion

        #region Base
        protected override IEnumerable<CLoc> GetEntities(AccellosContext entityContext)
        {
            throw new NotImplementedException();
        }

        protected override CLoc GetEntity(AccellosContext entityContext, string id)
        {
            using (OracleConnection cn = (OracleConnection)entityContext.DbConnection)
            {
                cn.Open();

                string sql = @"SELECT comp_code, cust_code, loc_code, 
on_hand_qty, invt_access, invt_lev1, hold_code 
FROM c_loc 
WHERE loc_code = :1";

                var parameters = new List<OracleParameter>{ 
                    new OracleParameter(":1", OracleDbType.Varchar2, id, ParameterDirection.Input)
                };

                CLoc loc = null;

                OracleManager.ExecuteReader(cn, sql, parameters, 
                     (reader) => loc = getEntityFromReader(reader)
                );

                return loc;
            }
        }
        #endregion

        private CLoc getEntityFromReader(OracleDataReader reader) {
            var loc = new CLoc
                {
                    CompCode = reader.GetString(reader.GetOrdinal("comp_code")),
                    LocCode = reader.GetString(reader.GetOrdinal("loc_code")),
                    CustCode = reader.GetString(reader.GetOrdinal("cust_code")),
                    OnHandQty = reader.GetInt32(reader.GetOrdinal("on_hand_qty")),
                    InvtAccess = reader.GetString(reader.GetOrdinal("invt_access")),
                    InvtLev1 = reader.GetString(reader.GetOrdinal("invt_lev1")),
                    HoldCode = reader.GetString(reader.GetOrdinal("hold_code"))
                };

                
                return loc;
        }

        
    }
}
