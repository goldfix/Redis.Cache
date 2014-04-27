using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Redis.Cache.Test
{
    [TestClass]
    public class ItemCacheTest
    {
        [TestMethod]
        public void Add()
        {
            ItemCache<DateTime> ic = new ItemCache<DateTime>("DT1", DateTime.Now, Utility.NO_EXPIRATION, Utility.NO_EXPIRATION);
            ic.Save(true);   
        }
        [TestMethod]
        public void Add_2()
        {
            string test = DateTime.Now.ToString();
            ItemCache<string>.AddItem("k1", test, new TimeSpan(1, 2, 3), Redis.Cache.Utility.NO_EXPIRATION, true);
            ItemCache<string> t1 = ItemCache<string>.GetItem("k1");
            Assert.AreEqual(t1.Value, test);
        }
        [TestMethod]
        public void Get()
        {
            DateTime dt = DateTime.Now;
            ItemCache<DateTime> ic = new ItemCache<DateTime>("DT1", dt, Utility.NO_EXPIRATION, new TimeSpan(1, 0, 0));                      //Serialization Object
            ic.Save(true);
            ItemCache<byte[]> ic_b = new ItemCache<byte[]>("DT3", Utility.Serialize(dt), Utility.NO_EXPIRATION, new TimeSpan(1, 0, 0));     //Byte[] Object
            ic_b.Save(true);

            ItemCache<DateTime> ic_2 = ItemCache<DateTime>.GetItem("DT1");
            Assert.AreEqual<DateTime>(dt, ic_2.Value);

            ItemCache<byte[]> ic_b_2 = ItemCache<byte[]>.GetItem("DT3");
            Assert.AreEqual<DateTime>(dt, ((DateTime)Utility.DeSerialize(ic_b_2.Value)));

            ItemCache<DateTime> ic_3 = ItemCache<DateTime>.GetItem("DT2");
            Assert.AreEqual(null, ic_3);
        }
        [TestMethod]
        public void Delete()
        {
            DateTime dt = new DateTime(2012, 02, 01);
            ItemCache<DateTime> ic = new ItemCache<DateTime>("DT2", dt, Utility.NO_EXPIRATION, Utility.NO_EXPIRATION);
            ic.Save(true);

            ItemCache<DateTime>.DeleteItem("DT2");
            ItemCache<DateTime> test = ItemCache<DateTime>.GetItem("DT2");
            Assert.AreEqual(test, null);
        }
    }
}
