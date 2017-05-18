using Accellos.Business.Contracts;
using Accellos.Business.Entities;
using Accellos.Business.Entities.Query;
using Accellos.Data.Contracts;
using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cisco.Sncyc.Business.BusinessEngines
{
    [Export(typeof(ICiscoSnCycEngine))]
    [PartCreationPolicy(CreationPolicy.NonShared)] //dont use Singleton, which is the default in MEF
    public class CiscoSnCycEngine : ICiscoSnCycEngine
    {
        [Import]
        private IReadOnlyRepositoryFactory _readOnlyRepositoryFactory = null;

        [Import]
        private IDataRepositoryFactory _dataRepositoryFactory = null;

        [Import]
        private IBatchQueryFactory _batchQueryFactory = null;

        public CiscoSnCycEngine()
        {

        }

        /// <summary>
        /// Unit Testing hook
        /// </summary>
        /// <param name="readOnlyRepositoryFactory"></param>
        public CiscoSnCycEngine(IReadOnlyRepositoryFactory readOnlyRepositoryFactory,
            IDataRepositoryFactory dataRepositoryFactory,
            IBatchQueryFactory batchQueryFactory)
         {
             _readOnlyRepositoryFactory = readOnlyRepositoryFactory;
             _dataRepositoryFactory = dataRepositoryFactory;
             _batchQueryFactory = batchQueryFactory;
         }

        public static bool IsAdminUser(string user)
        {
#if (DEBUG)
            return true;
#endif        
            var setting = ConfigurationManager.AppSettings["adminUsers"];

            return setting.ToLower().Split(',').Contains(user.ToLower());
        }

        #region GenerateLocSerialsMovedReport
        public string GenerateLocSerialsMovedReport(string custCode, string locCode)
        {
            var repo = _dataRepositoryFactory
                .GetDataRepository<ILRyderCiscoSncycCntRepository>();

            var items = repo.GetScannedQtyMismatches("H2", custCode, locCode);

            if (items.Count == 0)
                throw new Exception("Location Serials Moved Report has no data");

            var path = string.Format("{0}\\{1}_{2}.csv", 
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "loc_serials_moved", DateTime.Now.ToString("yyyyMMddhhmm"));
            
            using (var fs = File.Create(path))
            using (StreamWriter bw = new StreamWriter(fs))
            {
                bw.WriteLine("\"CustomerCode\",\"LocationCode\",\"ItemCode\",\"LocOnHandQty\",\"CycleCountQty\",\"Difference\"");
                foreach (var item in items)
                {
                    bw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",{3},{4},{5}", 
                        item.CustomerCode, 
                        item.LocationCode,
                        item.ItemCode,
                        item.LocOnHandQty,
                        item.CycleCountQty,
                        item.Difference));
                }
                
                bw.Close();
            }

            return Path.GetFileName(path);
        }
        #endregion

        #region GenerateLocsNotCountedReport
        public string GenerateLocsNotCountedReport(string custCode)
        {
            var repo = _dataRepositoryFactory
                .GetDataRepository<ILRyderCiscoSncycCntRepository>();

            var items = repo.GetLocationsNotCounted("H2", custCode);

            if (items.Count == 0)
                throw new Exception("Locations Not Counted Report has no data");

            var path = string.Format("{0}\\{1}_{2}.csv",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "locs_not_counted", DateTime.Now.ToString("yyyyMMddhhmm"));

            using (var fs = File.Create(path))
            using (StreamWriter bw = new StreamWriter(fs))
            {
                bw.WriteLine("\"CustomerCode\",\"LocationCode\",\"ItemCode\",\"LocOnHandQty\"");
                foreach (var item in items)
                {
                    bw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",{3}",
                        item.CustomerCode,
                        item.LocationCode,
                        item.ItemCode,
                        item.OnHandQty
                        )
                    );
                }
                bw.Close();
            }

            return Path.GetFileName(path);
        }
        #endregion

        #region GenerateLocsCountedReport
        public string GenerateLocsCountedReport(string custCode)
        {
            var repo = _dataRepositoryFactory
                .GetDataRepository<ILRyderCiscoSncycCntRepository>();

            var items = repo.GetLocationsCounted("H2", custCode);

            if (items.Count == 0)
                throw new Exception("Locations Counted Report has no data");

            var path = string.Format("{0}\\{1}_{2}.csv",
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "locs_counted", DateTime.Now.ToString("yyyyMMddhhmm"));

            using (var fs = File.Create(path))
            using (StreamWriter bw = new StreamWriter(fs))
            {
                bw.WriteLine("\"CustomerCode\",\"LocationCode\",\"ItemCode\",\"SerialCount\"");
                foreach (var item in items)
                {
                    bw.WriteLine(string.Format("\"{0}\",\"{1}\",\"{2}\",{3}",
                        item.CustomerCode,
                        item.LocationCode,
                        item.ItemCode,
                        item.SerialCount
                        )
                    );
                }
                bw.Close();
            }

            return Path.GetFileName(path);
        }
        #endregion

        public string CleanProductCode(string prodCode)
        {
            var clean = string.Empty;

            if (prodCode.StartsWith("1P"))
                clean = prodCode.Substring(2);
            else
                clean = prodCode;

            return clean;
        }

        public MCustH GetCustomer(string custCode)
        {
            var repo = _readOnlyRepositoryFactory.GetDataRepository<IMCustHRepository>();
            var entities = repo.GetByExample(new MCustHParams 
            { 
                CompCode = "H2", 
                CustCode = custCode 
            });
            return entities.FirstOrDefault();
        }

        public bool IsCustomerValid(MCustH customer)
        {
            return (customer != null && customer.IsCisco);
        }

        public MLoc GetLocation(string compCode, string locCode)
        {
            var repo = _readOnlyRepositoryFactory.GetDataRepository<IMLocRepository>();
            var locations = repo.GetByExample(new MLocParams 
            {  
                CompCode = compCode, 
                LocCode = locCode
            });

            return locations.FirstOrDefault(); 
        }

        public bool IsLocationValid(MLoc location)
        {
            return (location != null && location.IsActive);
        }

        public bool IsLocationCounted(string locCode)
        {
            return false;
        }

        public bool IsBulkFlagValid(string flag)
        {
            return flag.Equals("Y") ||
                    flag.Equals("N");
        }

        public MItemH GetProduct(string compCode, string custCode, string itemCode)
        {
            var repo = _readOnlyRepositoryFactory.GetDataRepository<IMItemHRepository>();
            var items = repo.GetByExample(new MItemHParams
            {
                CompCode = compCode,
                CustCode = custCode,
                ItemCode = itemCode
            });

            return items.FirstOrDefault(); 
        }

        public bool IsProductValid(MItemH item)
        {
            if (item == null || !item.IsActive)
                return false;

            if (!item.Serialized)
                throw new Exception("Not a serialized item.");

            return true;
        }

        public int GetCalculatedInventory(string compCode, string custCode, string locCode, string itemCode)
        {
            var repo = _readOnlyRepositoryFactory.GetDataRepository<ICLocRepository>();
            var items = repo.GetByExample(new CLocParams
            {
                CompCode = compCode,
                CustCode = custCode,
                LocCode = locCode,
                InvtLev1 = itemCode
            });

            if (items.Count == 0)
                throw new ArgumentException(string.Format("CLoc not found: {0}, {1}", locCode, itemCode));
            return items.Sum(i => i.OnHandQty); 
        }

        public bool IsValidSerialNo(string sn)
        {
            if (sn.StartsWith("S"))
            {
                return true;
            }
            return false;
        }

        public bool SerialNoExists(string custCode, string locCode, string itemCode, string serialno)
        {
            var repo = _dataRepositoryFactory
                .GetDataRepository<ILRyderCiscoSncycCntRepository>();

            var exists = repo.GetByExample(new LRyderCiscoSncycCntParams
            {
                CustCode = custCode,
                LocCode = locCode,
                ItemCode = itemCode,
                Serial = serialno
            }).Count > 0;

            //if (exists)
              //  throw new InvalidOperationException(string.Format("{0} already scanned", serialno));

            return exists;
        }

        public bool AddSerialNo(MItemH item, MLoc location, string serialno, string itemType, string bulk, string userName)
        {
            var repo = _dataRepositoryFactory
                .GetDataRepository<ILRyderCiscoSncycCntRepository>();
            
            var saved = repo.Add(new LRyderCiscoSncycCnt
            {
                CustCode = item.CustCode,
                Serial = serialno,
                LocCode = location.LocCode,
                ItemCode = item.ItemCode,
                ProsDateTime = DateTime.Now,
                UserName = userName,
                BulkItem = bulk,
                ItemType = itemType
            });

            return true;
        }

        public int GetScannedCount(string custCode, string locCode, string itemCode)
        {
            var repo = _dataRepositoryFactory
                .GetDataRepository<ILRyderCiscoSncycCntRepository>();

            return repo.GetSerialCount(custCode, locCode, itemCode);
        }

        public bool IsValidItemType(string itemType)
        {
            return (itemType.Equals("NB") || itemType.Equals("RF"));
        }

        public void ArchiveSerials()
        {
            if (ConfigurationManager.AppSettings["archiveSerials"] == "0")
                return;

            var query = _batchQueryFactory.GetBatchQuery<IArchiveRyderCiscoSncycCnt>();

            try
            {
                int days;
                int.TryParse(ConfigurationManager.AppSettings["archiveSerialsDays"], out days);
                query.Days = days;
                query.Execute();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RescanSerials(string custCode, string locCode, string itemCode)
        {
            var query = _batchQueryFactory.GetBatchQuery
                <IRecountRyderCiscoSncycCnt>();

            query.Execute(new RecountLRyderCiscoSncycCntParams 
            { 
                CustCode = custCode,
                LocCode = locCode,
                ItemCode = itemCode
            });
        }
    }
}
