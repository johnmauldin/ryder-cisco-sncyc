﻿using Accellos.Business.Contracts;
using Accellos.Business.Entities;
using Accellos.Business.Entities.Query;
using Accellos.Data.Contracts;
using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

        public CiscoSnCycEngine()
        {

        }

        public CiscoSnCycEngine(IReadOnlyRepositoryFactory readOnlyRepositoryFactory)
         {
             _readOnlyRepositoryFactory = readOnlyRepositoryFactory;
         }

        public MCustH GetCustomer(string custCode)
        {
            var repo = _readOnlyRepositoryFactory.GetDataRepository<IMCustHRepository>();
            return repo.Get(custCode);
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
            return (item != null && item.IsActive);
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

        public bool AddSerialNo(MItemH item, MLoc location, string serialno, string itemType, string bulk, string userName)
        {
            var repo = _dataRepositoryFactory
                .GetDataRepository<ILRyderCiscoSncycCntRepository>();
            
            var exists = repo.GetByExample(new LRyderCiscoSncycCntParams
            {
                CustCode = item.CustCode,
                LocCode = location.LocCode,
                ItemCode = item.ItemCode,
                Serial = serialno
            }).Count > 0;

            if (exists)
                throw new InvalidOperationException(string.Format("{0} already scanned", serialno));

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
    }
}