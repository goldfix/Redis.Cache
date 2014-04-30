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
    public class DataStorageTest
    {
        [TestMethod]
        public void Test_KeyValue()
        {
            //STRING_KEY / STRING_VALUE == DATA CACHE
            //STRING_KEY / STRING_VALUE == TTL ABS|TTL SLI.
            
            // 239.8MB x 38 Sec.
            // 128MB (compression active) x 52 Sec.

            for (int i = 0; i < 100001; i++)
            {
                string k = Guid.NewGuid().ToString();
                string v = k + " :: " + Properties.Settings.Default.Value_Text;
                //byte[] v = Utility.Deflate(Utility.Serialize(k + " :: " + Properties.Settings.Default.Value_Text), System.IO.Compression.CompressionMode.Compress);

                DateTime dt_TTL_ABS = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0,0,10));
                DateTime dt_TTL_SLI = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 5));

                string dt_TTL_ABS_str = dt_TTL_ABS.ToString("yyyyMMddTHHmmss");
                string dt_TTL_SLI_str = dt_TTL_SLI.ToString("yyyyMMddTHHmmss");

                Redis.Cache.RedisDal dal = new RedisDal();
                dal.AddItem("DATA_" + k, v); // add data
                dal.AddItem("TTL_" + k, dt_TTL_ABS_str + "|" + dt_TTL_SLI_str); // add ttl
            }
        }

        [TestMethod]
        public void Test_KeyValue_2()
        {
            //STRING_KEY / STRING_VALUE == TTL ABS|TTL SLI|DATA.

            // 228.8MB

            for (int i = 0; i < 100001; i++)
            {
                string k = Guid.NewGuid().ToString();
                string v = k + " :: " + Properties.Settings.Default.Value_Text;
                DateTime dt_TTL_ABS = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 10));
                DateTime dt_TTL_SLI = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 5));

                string dt_TTL_ABS_str = dt_TTL_ABS.ToString("yyyyMMddTHHmmss");
                string dt_TTL_SLI_str = dt_TTL_SLI.ToString("yyyyMMddTHHmmss");

                Redis.Cache.RedisDal dal = new RedisDal();
                dal.AddItem(k, dt_TTL_ABS_str + "|" + dt_TTL_SLI_str + "|" + v); // add data with TTL
            }
        }

        //[TestMethod]
        //public void Test_Set()
        //{
        //    //SET /     DATA == DATA CACHE
        //    //          TTL  == TTL ABS|TTL SLI
        //    // 250.1MB

        //    for (int i = 0; i < 100001; i++)
        //    {
        //        string k = Guid.NewGuid().ToString();
        //        string v = k + " :: " + Properties.Settings.Default.Value_Text;
        //        DateTime dt_TTL_ABS = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 10));
        //        DateTimeOffset dt_TTL_SLI = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 5));

        //        string dt_TTL_ABS_str = dt_TTL_ABS.ToString("yyyyMMddTHHmmss");
        //        string dt_TTL_SLI_str = dt_TTL_SLI.ToString("yyyyMMddTHHmmss");

        //        Redis.Cache.RedisDal dal = new RedisDal();
        //        dal.AddSetItem(k, v);
        //        dal.AddSetItem(k, dt_TTL_ABS_str + "|" + dt_TTL_SLI_str);
        //    }
        //}


        [TestMethod]
        public void Test_List()
        {
            //LIST /    DATA == DATA CACHE
            //          TTL  == TTL ABS|TTL SLI
            // 243.4MB  X 22 Sec.
            // 132.3MB  X 45 Sec.   (with compression)

            
            for (int i = 0; i < 100001; i++)
            {
                string k = Guid.NewGuid().ToString();
                //string v = k + " :: " + Properties.Settings.Default.Value_Text;
                byte[] v = Utility.Deflate(Utility.Serialize(k + " :: " + Properties.Settings.Default.Value_Text), System.IO.Compression.CompressionMode.Compress);
                DateTime dt_TTL_ABS = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 10));
                DateTime dt_TTL_SLI = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 5));

                string dt_TTL_ABS_str = dt_TTL_ABS.ToString("yyyyMMddTHHmmss");
                string dt_TTL_SLI_str = dt_TTL_SLI.ToString("yyyyMMddTHHmmss");

                Redis.Cache.RedisDal dal = new RedisDal();
                dal.AddListItem(k, v, dt_TTL_ABS_str + "|" + dt_TTL_SLI_str);
            }
        }



        [TestMethod]
        public void Test_Hash()
        {
            //HASH /    K / DATA == DATA CACHE
            //          K / TTL  == TTL ABS|TTL SLI
            // 245.2MB

            string k_hash = "123456";
            for (int i = 0; i < 50001; i++)
            {
                string k = Guid.NewGuid().ToString();
                string v = k + " :: " + Properties.Settings.Default.Value_Text;
                DateTime dt_TTL_ABS = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 10));
                DateTime dt_TTL_SLI = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 5));

                string dt_TTL_ABS_str = dt_TTL_ABS.ToString("yyyyMMddTHHmmss");
                string dt_TTL_SLI_str = dt_TTL_SLI.ToString("yyyyMMddTHHmmss");

                Redis.Cache.RedisDal dal = new RedisDal();
                dal.AddHashItem(k_hash, "DATA_" + k, v);
                dal.AddHashItem(k_hash, "TTL_" + k, dt_TTL_ABS_str + "|" + dt_TTL_SLI_str);
            }

            k_hash = "789012";
            for (int i = 0; i < 50001; i++)
            {
                string k = Guid.NewGuid().ToString();
                string v = k + " :: " + Properties.Settings.Default.Value_Text;
                DateTime dt_TTL_ABS = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 10));
                DateTime dt_TTL_SLI = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 5));

                string dt_TTL_ABS_str = dt_TTL_ABS.ToString("yyyyMMddTHHmmss");
                string dt_TTL_SLI_str = dt_TTL_SLI.ToString("yyyyMMddTHHmmss");

                Redis.Cache.RedisDal dal = new RedisDal();
                dal.AddHashItem(k_hash, "DATA_" + k, v);
                dal.AddHashItem(k_hash, "TTL_" + k, dt_TTL_ABS_str + "|" + dt_TTL_SLI_str);
            }
        }

        [TestMethod]
        public void Test_Hash_2()
        {
            //HASH /    K / DATA == DATA CACHE
            //          K / TTL  == TTL ABS|TTL SLI
            //          ...
            // 259.3MB

            string k_hash = "123456";
            for (int i = 0; i < 100001; i++)
            {
                string k = Guid.NewGuid().ToString();
                string v = k + " :: " + Properties.Settings.Default.Value_Text;
                DateTime dt_TTL_ABS = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 10));
                DateTime dt_TTL_SLI = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 5));

                string dt_TTL_ABS_str = dt_TTL_ABS.ToString("yyyyMMddTHHmmss");
                string dt_TTL_SLI_str = dt_TTL_SLI.ToString("yyyyMMddTHHmmss");

                Redis.Cache.RedisDal dal = new RedisDal();
                dal.AddHashItem(k, "DATA", v);
                dal.AddHashItem(k, "TTL", dt_TTL_ABS_str + "|" + dt_TTL_SLI_str);
            }
        }


        [TestMethod]
        public void Test_HashBalanced()
        {
            //HASH /    K / DATA == DATA CACHE
            //          K / TTL  == TTL ABS|TTL SLI
            // 245.5MB

            int k_hash = 0;
            for (int i = 0; i < 100001; i++)
            {


                if ((i % 50) == 0)
                {
                    k_hash++;
                }


                string k = Guid.NewGuid().ToString();
                string v = k + " :: " + Properties.Settings.Default.Value_Text;
                DateTime dt_TTL_ABS = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 10));
                DateTime dt_TTL_SLI = DateTime.Now.ToUniversalTime().Add(new TimeSpan(0, 0, 5));

                string dt_TTL_ABS_str = dt_TTL_ABS.ToString("yyyyMMddTHHmmss");
                string dt_TTL_SLI_str = dt_TTL_SLI.ToString("yyyyMMddTHHmmss");

                Redis.Cache.RedisDal dal = new RedisDal();
                dal.AddHashItem(k_hash.ToString(), "DATA_" + k, v);
                dal.AddHashItem(k_hash.ToString(), "TTL_" + k, dt_TTL_ABS_str + "|" + dt_TTL_SLI_str);
            }
        }
    }
}
