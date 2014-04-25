using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Redis.Cache.Test
{
    [TestClass]
    public class MemoryTest
    {
        [TestMethod]
        public void LoadList()
        {
            //list-max-ziplist-entries 512
            //list-max-ziplist-value 64
            //9.1MB :: 10001
            //28.3MB :: 100001
            //---------------------------------------------

            //list-max-ziplist-entries 1024
            //list-max-ziplist-value 1024
            //6.7MB :: 10001
            //28.3MB :: 100001
            //---------------------------------------------

            for (int i = 0; i < 100001; i++)
            {
                string k = Guid.NewGuid().ToString();
                string v = Guid.NewGuid().ToString() + " -- " + Guid.NewGuid().ToString() + " -- " + Guid.NewGuid().ToString();
                Redis.Cache.RedisDal d = new Redis.Cache.RedisDal();
                d.AddItem(k, v);
            }
        }

        [TestMethod]
        public void LoadHash()
        {
            //hash-max-ziplist-entries 512
            //hash-max-ziplist-value 64
            //6.5MB :: 10001
            //54.2MB :: 100001
            //---------------------------------------------

            //hash-max-ziplist-entries 1024
            //hash-max-ziplist-value 1024
            //6.8MB :: 10001
            //31.3MB :: 100001
            //---------------------------------------------


            for (int i = 0; i < 100001; i++)
            {
                string k = Guid.NewGuid().ToString();
                string v = Guid.NewGuid().ToString() + " -- " + Guid.NewGuid().ToString() + " -- " + Guid.NewGuid().ToString();
                Redis.Cache.RedisDal d = new Redis.Cache.RedisDal();
                d.AddHashItem(k, k, v);
            }
        }

        [TestMethod]
        public void LoadHash_2()
        {
            string k_1 = Guid.NewGuid().ToString();

            //hash-max-ziplist-entries 512
            //hash-max-ziplist-value 64
            //6.8MB :: 10001
            //30.8MB :: 100001
            //---------------------------------------------

            //hash-max-ziplist-entries 1024
            //hash-max-ziplist-value 1024
            //6.8MB :: 10001
            //31.3MB :: 100001
            //---------------------------------------------

            for (int i = 0; i < 100001; i++)
            {
                string k = Guid.NewGuid().ToString();
                string v = Guid.NewGuid().ToString() + " -- " + Guid.NewGuid().ToString() + " -- " + Guid.NewGuid().ToString();
                Redis.Cache.RedisDal d = new Redis.Cache.RedisDal();
                d.AddHashItem(k_1, k, v);
            }
        }
    }
}
