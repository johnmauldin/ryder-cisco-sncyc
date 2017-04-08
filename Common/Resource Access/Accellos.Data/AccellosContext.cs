using System;
using System.Collections.Generic;
//using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Accellos.Business.Entities;

using Core.Common.Contracts;
using Core.Common.Data;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Data;

namespace Accellos.Data
{
    public class AccellosContext : IDbContext
    {
        public AccellosContext()
            //: base("name=Accellos")
        {
            //Database.SetInitializer<AccellosContext>(null);
        }

        public DbConnection DbConnection
        {
            get { return new OracleConnection(Settings.AccellosConnString); } 
        }

        //public DbSet<Location> AccountSet { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
          //  modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Ignore<ExtensionDataObject>();
            //modelBuilder.Ignore<IIdentifiableEntity>();

            //modelBuilder.Entity<Location>().HasKey<int>(e => e.AccountId).Ignore(e => e.EntityId).Ignore(e => e.OwnerAccountId);
            //modelBuilder.Entity<Car>().HasKey<int>(e => e.CarId).Ignore(e => e.EntityId);
            //modelBuilder.Entity<Rental>().HasKey<int>(e => e.RentalId).Ignore(e => e.EntityId).Ignore(e => e.OwnerAccountId);
            //modelBuilder.Entity<Reservation>().HasKey<int>(e => e.ReservationId).Ignore(e => e.EntityId).Ignore(e => e.OwnerAccountId);
            //modelBuilder.Entity<Car>().Ignore(e => e.CurrentlyRented);
        //}

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }


    }
}
