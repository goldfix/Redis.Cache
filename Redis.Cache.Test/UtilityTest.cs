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


        [TestMethod]
        public void TestCompression()
        {
            System.Collections.Generic.Dictionary<string, string> dic = new System.Collections.Generic.Dictionary<string, string>();

            for (int i = 0; i < 10001; i++)
            {
                dic.Add(Guid.NewGuid().ToString(), Guid.NewGuid().ToString() + "::" + Properties.Settings.Default.Value_Text + "::" + Guid.NewGuid().ToString());
            }

            byte[] source_byte = Utility.Serialize(dic);
            byte[] dest_byte = Utility.Deflate(source_byte, System.IO.Compression.CompressionMode.Compress);


            byte[] dest_byte_2 = Utility.Deflate(dest_byte, System.IO.Compression.CompressionMode.Decompress);

            for (int i = 0; i < source_byte.Length; i++)
            {
                if (source_byte[i] != dest_byte_2[i])
                {
                    Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("Byte Array Error...");
                    break;
                }
            }

        }
    }
}
