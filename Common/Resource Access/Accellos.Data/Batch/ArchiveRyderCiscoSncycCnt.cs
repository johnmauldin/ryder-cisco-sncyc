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
    [Export(typeof(IArchiveRyderCiscoSncycCnt))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ArchiveRyderCiscoSncycCnt : IArchiveRyderCiscoSncycCnt
    {
        string FROM = @"FROM l_ryder_cisco_sncyc_cnt s 
JOIN c_pros_mvt m 
 ON s.cust_code = m.cust_code
 AND s.item_code = m.invt_lev1
 AND (s.serial = m.pros_value OR s.serial = ('S' || m.pros_value))
WHERE m.pros_trans_tp = 'CO'
AND m.pros_trans_date BETWEEN sysdate - 2 AND sysdate";

        public void Execute()
        {
            var ctx = new AccellosContext();

            var copyFmt = @"INSERT INTO l_ryder_cisco_sncyc_cnt_hist 
SELECT s.cust_code, s.loc_code, s.item_code, 
 s.serial, s.pros_date_time, s.username, s.bulk_item, s.item_type, 'OC'
{0}
";

            var copy = string.Format(copyFmt, FROM);

            var deleteFmt = @"DELETE FROM l_ryder_cisco_sncyc_cnt WHERE EXISTS (
SELECT s.* {0}
)";

            var delete = string.Format(deleteFmt, FROM);

            var oracleParams = new List<OracleParameter>();

            using (OracleConnection cn = (OracleConnection)ctx.DbConnection)
            {
                cn.Open();

                using (var tx = cn.BeginTransaction()) {

                    try
                    {
                        OracleManager.ExecuteSql(tx, copy, oracleParams);
                        OracleManager.ExecuteSql(tx, delete, oracleParams);

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
    }
}
