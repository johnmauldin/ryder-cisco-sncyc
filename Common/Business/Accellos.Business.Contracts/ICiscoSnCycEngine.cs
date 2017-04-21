using Accellos.Business.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Business.Contracts
{
    public interface ICiscoSnCycEngine
    {
        bool IsAdminUser(string user);

        MCustH GetCustomer(string custCode);

        MLoc GetLocation(string compCode, string locCode);

        MItemH GetProduct(string compCode, string custCode, string itemCode);

        bool IsCustomerValid(MCustH customer);

        bool IsLocationValid(MLoc location);

        bool IsLocationCounted(string locCode);

        bool IsProductValid(MItemH item);

        bool IsBulkFlagValid(string flag);

        int GetCalculatedInventory(string compCode, string custCode, string locCode, string itemCode);

        bool IsValidSerialNo(string sn);

        bool SerialNoExists(string custCode, string locCode, string itemCode, string serialno);

        bool IsValidItemType(string itemType);

        bool AddSerialNo(MItemH item, MLoc location, string serialno, string itemType, string bulk, string userName);

        int GetScannedCount(string custCode, string locCode, string itemCode);

        void ArchiveSerials();

        void RescanSerials(string custCode, string locCode, string itemCode);
    }
}
