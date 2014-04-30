/*
Copyright (c) 2014, pietro partescano
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the Redis.Cache.Py nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

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
        public void Add_3()
        {
            string test = Properties.Settings.Default.Value_Text;
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
