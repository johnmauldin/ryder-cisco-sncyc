using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Data
{
    public interface IDbContext : IDisposable
    {
        DbConnection DbConnection { get; }

        void SaveChanges();
    }
}
