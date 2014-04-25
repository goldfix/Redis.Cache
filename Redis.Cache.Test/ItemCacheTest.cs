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
            ic.Save();
            
        }
    }
}
