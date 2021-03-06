﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Redis.Cache.Test
{
    [TestClass]
    public class PythonTest
    {
        [TestMethod]
        public void InitPythonTest()
        {
            Redis.Cache.ItemCache<string>.AddItem("k_string", "Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 メンズア", new TimeSpan(01, 02, 03), new TimeSpan(04, 05, 06), true);
            Redis.Cache.ItemCache<Int16>.AddItem("k_int16", 12345, true);
            Redis.Cache.ItemCache<Int32>.AddItem("k_int32", 1234567890, true);
            Redis.Cache.ItemCache<double>.AddItem("k_double", 12345.06789, true);

            byte[] tmp = System.Text.Encoding.UTF8.GetBytes("Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 Test 1234567890 Test 0987654321 メンズア");
            Redis.Cache.ItemCache<byte[]>.AddItem("k_byte[]",tmp, true);

            byte[] tmp_compress = Utility.Deflate(tmp, System.IO.Compression.CompressionMode.Compress);
            Redis.Cache.ItemCache<byte[]>.AddItem("k_compress_byte[]", tmp_compress, true);

            string s = Properties.Settings.Default.Value_Text_long;
            byte[] str_decompress = System.Text.Encoding.UTF8.GetBytes(s);
            byte[] str_compress = Redis.Cache.Utility.Deflate(str_decompress, System.IO.Compression.CompressionMode.Compress);
            System.IO.File.WriteAllBytes("c:\\tmp\\out_c.bin", str_compress);
        }

        [TestMethod]
        public void LoadPythonDataTest()
        {
            string s = Properties.Settings.Default.Value_Text_long;

            byte[] b = System.IO.File.ReadAllBytes("c:\\tmp\\out_p.bin");
            byte[] b_decompress = Redis.Cache.Utility.Deflate(b, System.IO.Compression.CompressionMode.Decompress);
            string str_fromPython = System.Text.Encoding.UTF8.GetString(b_decompress);
            
            int i = String.Compare(s, str_fromPython, false);
            
            Assert.AreEqual<string>(s, str_fromPython);
        }

        [TestMethod]
        public void SetToPy()
        {
            string test = Properties.Settings.Default.Value_Text;
            Redis.Cache.ItemCache<string> t = new ItemCache<string>("k1", test);
            t.Save(false);
        }


        [TestMethod]
        public void GetFromPy()
        {
            string test = Properties.Settings.Default.Value_Text;
            Redis.Cache.ItemCache<string> t = Redis.Cache.ItemCache<string>.GetItem("k2");

            //Assert.AreEqual<string>(test.Trim(), t.Value.Trim());
            Assert.AreEqual<TimeSpan>(t.SlidingExpiration, new TimeSpan(11, 22, 33));
            Assert.AreEqual<TimeSpan>(t.AbsoluteExpiration, new TimeSpan(44, 55, 66));

        }


        [TestMethod]
        public void GetFromPy_2()
        {
            string test = Properties.Settings.Default.Value_Text_long;
            Redis.Cache.ItemCache<string> t = Redis.Cache.ItemCache<string>.GetItem("K3");

            Assert.AreEqual<string>(test, t.Value);
            Assert.AreEqual<TimeSpan>(t.SlidingExpiration, new TimeSpan(4, 5, 6));
            Assert.AreEqual<TimeSpan>(t.AbsoluteExpiration, new TimeSpan(5, 6, 7));
        }
    }
}
