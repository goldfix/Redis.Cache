using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Redis.Cache.Test
{
    [TestClass]
    public class UtilityTest
    {
        [TestMethod]
        public void TestSerialize_DeSerialize()
        {
            DateTime dt = new DateTime(2012, 01, 02, 1, 2, 3);
            byte[] t = Redis.Cache.Utility.Serialize(dt);
            DateTime dt_result = (DateTime)Redis.Cache.Utility.DeSerialize(t);
            Assert.AreEqual<DateTime>(dt, dt_result);
        }
    }
}
