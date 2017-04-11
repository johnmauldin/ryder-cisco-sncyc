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

namespace Accellos.Data.Batch
{
    [Export(typeof(IRecountRyderCiscoSncycCnt))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RecountRyderCiscoSncycCnt : IRecountRyderCiscoSncycCnt
    {
        public void Execute(RecountLRyderCiscoSncycCntParams data)
        {
            var ctx = new AccellosContext();

            StringBuilder copy = new StringBuilder();
            StringBuilder delete = new StringBuilder();

            copy.Append(@"INSERT INTO l_ryder_cisco_sncyc_cnt_hist
SELECT s.cust_code, s.loc_code, s.item_code, 
 s.serial, s.pros_date_time, s.username, s.bulk_item, s.item_type, 'RC'
FROM l_ryder_cisco_sncyc_cnt s
WHERE 1=1
");

            delete.Append(@"DELETE FROM l_ryder_cisco_sncyc_cnt
WHERE 1=1
");

            var copyParams = new List<OracleParameter>();
            var deleteParams = new List<OracleParameter>();

            if (!string.IsNullOrWhiteSpace(data.CustCode))
            {
                copy.Append("AND cust_code = :1 ");
                delete.Append("AND cust_code = :1 ");
                copyParams.Add(new OracleParameter(":1", OracleDbType.Varchar2, data.CustCode, ParameterDirection.Input));
                deleteParams.Add(new OracleParameter(":1", OracleDbType.Varchar2, data.CustCode, ParameterDirection.Input));

            }

            if (!string.IsNullOrWhiteSpace(data.LocCode))
            {
                copy.Append("AND loc_code = :2 ");
                delete.Append("AND loc_code = :2 ");
                copyParams.Add(new OracleParameter(":2", OracleDbType.Varchar2, data.LocCode, ParameterDirection.Input));
                deleteParams.Add(new OracleParameter(":2", OracleDbType.Varchar2, data.LocCode, ParameterDirection.Input));
            }

            if (!string.IsNullOrWhiteSpace(data.ItemCode))
            {
                copy.Append("AND item_code = :3 ");
                delete.Append("AND item_code = :3 ");
                copyParams.Add(new OracleParameter(":3", OracleDbType.Varchar2, data.ItemCode, ParameterDirection.Input));
                deleteParams.Add(new OracleParameter(":3", OracleDbType.Varchar2, data.ItemCode, ParameterDirection.Input));
            }

            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                using (var tx = cn.BeginTransaction())
                {
                    try
                    {
                        OracleManager.ExecuteSql(tx, copy.ToString(), copyParams);

                        OracleManager.ExecuteSql(tx, delete.ToString(), deleteParams);

                        tx.Commit();
                    }
                    catch (Exception)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Execute()
        {
            //TODO: how to avoid implementation of the parameterless interface
            throw new NotImplementedException();
        }
    }
}
