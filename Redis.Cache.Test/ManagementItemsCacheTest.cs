using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Redis.Cache.Test
{
    [TestClass]
    public class ManagementItemsCacheTest
    {
        [TestMethod]
        public void Add()
        {
            string k = Guid.NewGuid().ToString();
            Redis.Cache.ManagementItemsCache m = new Cache.ManagementItemsCache();
            long result = m.Add<string>(k, "B", TimeSpan.Zero, TimeSpan.Zero, true);
            Assert.AreEqual<long>(result, 2);
        }
        [TestMethod]
        public void Get_String()
        {
            Redis.Cache.ManagementItemsCache m = new Cache.ManagementItemsCache();

            string k = Guid.NewGuid().ToString();
            long result = m.Add<string>(k, "B", TimeSpan.Zero, TimeSpan.Zero, true);
            Assert.AreEqual<long>(result, 2);

            string result_2 = m.GetValue <string>(k);
            Assert.AreEqual<string>(result_2, "B");

            result_2 = m.GetValue<string>(k + "1");
            Assert.AreEqual<string>(result_2, null);
        }
        [TestMethod]
        public void Get_DateTime()
        {
            Redis.Cache.ManagementItemsCache m = new Cache.ManagementItemsCache();

            //Test Serialization
            string k = Guid.NewGuid().ToString();
            DateTime dt = DateTime.Now;
            long result = m.Add<DateTime>(k, dt, TimeSpan.Zero, TimeSpan.Zero, true);
            Assert.AreEqual<long>(result, 2);

            DateTime result_3 = m.GetValue<DateTime>(k);
            Assert.AreEqual<DateTime>(result_3, dt);

            DateTime result_4 = m.GetValue<DateTime>(k + "1");
            Assert.AreEqual<DateTime>(result_3, DateTime.MinValue);
        }

        [TestMethod]
        public void Exist_And_Delete()
        {
            Redis.Cache.ManagementItemsCache m = new Cache.ManagementItemsCache();

            //Test Serialization
            string k = Guid.NewGuid().ToString();
            DateTime dt = DateTime.Now;
            long result = m.Add<DateTime>(k, dt, TimeSpan.Zero, TimeSpan.Zero, true);
            Assert.AreEqual<long>(result, 2);

            Assert.AreEqual<bool>(m.Exist(k), true);
            Assert.AreEqual<bool>(m.Delete(k), true);
            Assert.AreEqual<bool>(m.Exist(k), false);
        }

        [TestMethod]
        public void Test_TTL_SLI()
        {
            Redis.Cache.ManagementItemsCache m = new Cache.ManagementItemsCache();

            string k = Guid.NewGuid().ToString();
            long result = m.Add<string>(k, "TTL_1", new TimeSpan(0, 0, 5), TimeSpan.Zero, true);
            Assert.AreEqual<long>(result, 2);

            //Wait 2 sec.
            System.Threading.Thread.Sleep(2000);
            string result_1 = m.GetValue<String>(k);
            Assert.AreEqual<String>(result_1, "TTL_1");

            //Wait 6 sec.
            System.Threading.Thread.Sleep(15000);
            string result_2 = m.GetValue<String>(k);
            Assert.AreEqual<String>(result_2, null);
        }

        [TestMethod]
        public void Test_TTL_ABS()
        {
            Redis.Cache.ManagementItemsCache m = new Cache.ManagementItemsCache();

            string k = Guid.NewGuid().ToString();
            long result = m.Add<string>(k, "TTL_1", TimeSpan.Zero, new TimeSpan(0, 0, 15), true);
            Assert.AreEqual<long>(result, 2);

            //Wait 2 sec.
            System.Threading.Thread.Sleep(2000);
            string result_1 = m.GetValue<String>(k);
            Assert.AreEqual<String>(result_1, "TTL_1");

            //Wait 6 sec.
            System.Threading.Thread.Sleep(15000);
            string result_2 = m.GetValue<String>(k);
            Assert.AreEqual<String>(result_2, null);
        }

        [TestMethod]
        public void Test_TTL_SLI_ABS()
        {
            Redis.Cache.ManagementItemsCache m = new Cache.ManagementItemsCache();

            string k = Guid.NewGuid().ToString();
            long result = m.Add<string>(k, "TTL_1", new TimeSpan(0, 0, 5), new TimeSpan(0, 0, 10), true);
            Assert.AreEqual<long>(result, 2);

            //Wait 2 sec.
            //System.Threading.Thread.Sleep(2000);
            string result_1 = m.GetValue<String>(k);
            Assert.AreEqual<String>(result_1, "TTL_1");

            //Wait 6 sec.
            //System.Threading.Thread.Sleep(10000);
            string result_2 = m.GetValue<String>(k);
            Assert.AreEqual<String>(result_2, null);
        }
    }
}
