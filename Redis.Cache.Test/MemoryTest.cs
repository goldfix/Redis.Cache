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
