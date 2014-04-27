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
