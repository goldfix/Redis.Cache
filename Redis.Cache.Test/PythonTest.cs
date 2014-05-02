using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Redis.Cache.Test
{
    [TestClass]
    public class PythonTest
    {
        [TestMethod]
        public void InitPythonTest()
        {
            Redis.Cache.ItemCache<string>.AddItem("k_string", "Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 メンズア", true);
            Redis.Cache.ItemCache<Int16>.AddItem("k_int16", 12345, true);
            Redis.Cache.ItemCache<Int32>.AddItem("k_int32", 1234567890, true);
            Redis.Cache.ItemCache<double>.AddItem("k_double", 12345.06789, true);

            byte[] tmp = System.Text.Encoding.UTF8.GetBytes("Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 メンズア");
            Redis.Cache.ItemCache<byte[]>.AddItem("k_byte[]",tmp, true);

            byte[] tmp_compress = Utility.Deflate(tmp, System.IO.Compression.CompressionMode.Compress);
            Redis.Cache.ItemCache<byte[]>.AddItem("k_compress_byte[]", tmp_compress, true);
        }
    }
}
