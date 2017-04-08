﻿using Accellos.Business.Entities;
using Accellos.Business.Entities.Query;
using Core.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accellos.Data.Contracts
{
    public interface ILRyderCiscoSncycCntRepository : IDataRepository<LRyderCiscoSncycCnt>
    {
        IList<LRyderCiscoSncycCnt> GetByExample(LRyderCiscoSncycCntParams example);

        int GetSerialCount(string custCode, string locCode, string itemCode);
    }
}
