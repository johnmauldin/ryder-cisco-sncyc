using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Common;
using Accellos.Data.Contracts;
using Accellos.Data.Repositories;
using Accellos.Business.Entities.Query;

namespace Accellos.Data.Tests
{
    [TestClass]
    public class LocationTests
    {
        [TestMethod]
        public void GetMLocation()
        {

            MLocRepository repo = new MLocRepository();

            //var loc = repo.Get("WRAP1");

            var locs = repo.GetByExample(new MLocParams 
            { 
                CompCode = "G1",
                LocCode = "WRAP1" 
            });

            MCompHRepository repo2 = new MCompHRepository();
            var com = repo2.Get("G1");
            
        }
    }
}
