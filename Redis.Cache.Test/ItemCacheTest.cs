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
        public void Add_Datetime()
        {
            DateTime dt_1 = DateTime.Now;
            ItemCache<DateTime> ic_1 = new ItemCache<DateTime>();
            ic_1.Key = "Add_Datetime";
            ic_1.Value = dt_1;
            ic_1.Save(true);

            ItemCache<DateTime> ic_2 = ItemCache<DateTime>.GetItem("Add_Datetime");
            Assert.AreEqual<DateTime>(dt_1, ic_2.Value);
        }
        [TestMethod]
        public void Add_String()
        {
            string obj_1 = Properties.Settings.Default.Value_Text_long;
            ItemCache<string> ic_1 = new ItemCache<string>();
            ic_1.Key = "Add_String";
            ic_1.Value = obj_1;
            ic_1.Save(true);

            ItemCache<string> ic_2 = ItemCache<string>.GetItem("Add_String");
            Assert.AreEqual<string>(obj_1, ic_2.Value);
        }
        [TestMethod]
        public void Add_Datetime_TTLSli()
        {
            DateTime dt_1 = DateTime.Now;
            ItemCache<DateTime> ic_1 = new ItemCache<DateTime>();
            ic_1.Key = "Add_Datetime_TTLSli";
            ic_1.Value = dt_1;
            ic_1.SlidingExpiration = new TimeSpan(0, 0, 20);
            ic_1.Save(true);

            System.Threading.Thread.Sleep(5000);
            ItemCache<DateTime> ic_2 = ItemCache<DateTime>.GetItem("Add_Datetime_TTLSli");
            Assert.AreEqual<DateTime>(dt_1, ic_2.Value);

            System.Threading.Thread.Sleep(20000);
            ItemCache<DateTime> ic_3 = ItemCache<DateTime>.GetItem("Add_Datetime_TTLSli");
            Assert.AreEqual(ic_3, null);
        }
        [TestMethod]
        public void Add_Datetime_TTLAbs()
        {
            DateTime dt_1 = DateTime.Now;
            ItemCache<DateTime> ic_1 = new ItemCache<DateTime>();
            ic_1.Key = "Add_Datetime_TTLAbs";
            ic_1.Value = dt_1;
            ic_1.AbsoluteExpiration = new TimeSpan(0, 0, 20);
            ic_1.Save(true);

            System.Threading.Thread.Sleep(5000);
            ItemCache<DateTime> ic_2 = ItemCache<DateTime>.GetItem("Add_Datetime_TTLAbs");
            Assert.AreEqual<DateTime>(dt_1, ic_2.Value);

            System.Threading.Thread.Sleep(20000);
            ItemCache<DateTime> ic_3 = ItemCache<DateTime>.GetItem("Add_Datetime_TTLAbs");
            Assert.AreEqual(ic_3, null);
        }
        [TestMethod]
        public void Add_Datetime_TTLAbsSli()
        {
            DateTime dt_1 = DateTime.Now;
            ItemCache<DateTime> ic_1 = new ItemCache<DateTime>();
            ic_1.Key = "Add_Datetime_TTLAbsSli";
            ic_1.Value = dt_1;
            ic_1.SlidingExpiration = new TimeSpan(0, 0, 10);
            ic_1.AbsoluteExpiration = new TimeSpan(0, 0, 25);
            ic_1.Save(true);

            System.Threading.Thread.Sleep(5000);
            ItemCache<DateTime> ic_2 = ItemCache<DateTime>.GetItem("Add_Datetime_TTLAbsSli");
            Assert.AreEqual<DateTime>(dt_1, ic_2.Value);

            System.Threading.Thread.Sleep(5000);
            ItemCache<DateTime> ic_3 = ItemCache<DateTime>.GetItem("Add_Datetime_TTLAbsSli");
            Assert.AreEqual<DateTime>(dt_1, ic_3.Value);

            System.Threading.Thread.Sleep(10000);
            ItemCache<DateTime> ic_4 = ItemCache<DateTime>.GetItem("Add_Datetime_TTLAbsSli");
            Assert.AreEqual(ic_4, null);
        }

        [TestMethod]
        public void Add_Datetime_TTLAbsSli_Delete()
        {
            DateTime dt_1 = DateTime.Now;
            ItemCache<DateTime> ic_1 = new ItemCache<DateTime>();
            ic_1.Key = "Add_Datetime_TTLAbsSli_Delete";
            ic_1.Value = dt_1;
            ic_1.SlidingExpiration = new TimeSpan(0, 0, 10);
            ic_1.AbsoluteExpiration = new TimeSpan(0, 0, 25);
            ic_1.Save(true);

            System.Threading.Thread.Sleep(8000);
            ItemCache<DateTime> ic_2 = ItemCache<DateTime>.GetItem("Add_Datetime_TTLAbsSli_Delete");
            Assert.AreEqual<DateTime>(dt_1, ic_2.Value);

            ItemCache<DateTime>.DeleteItem("Add_Datetime_TTLAbsSli_Delete");
            ItemCache<DateTime> ic_3 = ItemCache<DateTime>.GetItem("Add_Datetime_TTLAbsSli_Delete");
            Assert.AreEqual(ic_3, null);
        }

        [TestMethod]
        public void Add_Datetime_TTLAbsSli_Exist()
        {
            DateTime dt_1 = DateTime.Now;
            ItemCache<DateTime> ic_1 = new ItemCache<DateTime>();
            ic_1.Key = "Add_Datetime_TTLAbsSli_Exist";
            ic_1.Value = dt_1;
            ic_1.SlidingExpiration = new TimeSpan(0, 0, 10);
            ic_1.AbsoluteExpiration = new TimeSpan(0, 0, 25);
            ic_1.Save(true);

            System.Threading.Thread.Sleep(5000);
            Assert.AreEqual(true, ItemCache<DateTime>.ExistItem("Add_Datetime_TTLAbsSli_Exist"));
        }


    }
}
