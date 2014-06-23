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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redis.Cache
{
    public static class Utility
    {
        private readonly static string _No_TTL = "ND";     // new DateTime(1985, 10, 26, 06, 0, 0).ToString("yyMMddTHHmmss");

        public static readonly TimeSpan NO_EXPIRATION = TimeSpan.Zero;

        public enum TypeStorage : short
        {
            UseList=1,
            UseKeyValue=2,
            UseHash=3
        }

        [Flags]
        public enum StatusItem : short
        {
            None=0,
            Compressed=1,
            Serialized=2
        }

        static Utility()
        { }

        /// <summary>
        /// Check and convert native type supported from Redis.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object ConvertRedisValueToObject(StackExchange.Redis.RedisValue value, Type type, StatusItem statusItem)
        {
            object result = null;

            if (typeof(String) == type)
            {
                if ((statusItem & StatusItem.Compressed) == StatusItem.Compressed)
                {
                    byte[] tmp = Utility.Deflate((Byte[])value, System.IO.Compression.CompressionMode.Decompress);
                    result = System.Text.Encoding.UTF8.GetString(tmp);                   
                }
                else
                {
                    result = (String)value;
                }
            }
            else if (typeof(Int16) == type)
            {
                result = (Int16)value;
            }
            else if (typeof(Int32) == type)
            {
                result = (Int32)value;
            }
            else if (typeof(Int64) == type)
            {
                result = (Int64)value;
            }
            else if (typeof(Boolean) == type)
            {
                result = (Boolean)value;
            }
            else if (typeof(Single) == type)
            {
                result = (Single)value;
            }
            else if (typeof(Double) == type)
            {
                result = (Double)value;
            }
            else
            {
                result = (Byte[])value;
                if ((statusItem & StatusItem.Compressed) == StatusItem.Compressed)
                {
                    result = Utility.Deflate((Byte[])result, System.IO.Compression.CompressionMode.Decompress);
                }
                else if ((statusItem & StatusItem.Serialized) == StatusItem.Serialized)
                {
                    result = Utility.DeSerialize((Byte[])result);          //If not supported De-Serialize Object...
                }
                else
                {
                    //result = (Byte[])value;
                    //continue...
                }
            }

            return result;
        }
        /// <summary>
        /// Check and convert native type supported from Redis.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static StackExchange.Redis.RedisValue ConvertObjToRedisValue(object value, out StatusItem statusItem)
        {
            statusItem = StatusItem.None;
            StackExchange.Redis.RedisValue result = StackExchange.Redis.RedisValue.Null;

            if (typeof(Byte[]) == value.GetType())
            {   
                if (Properties.Settings.Default.UseCompression)
                {
                    result = Utility.Deflate((Byte[])value, System.IO.Compression.CompressionMode.Compress);
                    statusItem = StatusItem.Compressed;
                }
                else
                {
                    result = (Byte[])value;
                }
            }
            else if (typeof(String) == value.GetType())
            {
                if (Properties.Settings.Default.UseCompression)             //&& ((String)value).Length > 512
                {
                    byte[] tmp = System.Text.Encoding.UTF8.GetBytes((String)value);
                    result = Utility.Deflate(tmp, System.IO.Compression.CompressionMode.Compress);
                    statusItem = StatusItem.Compressed;
                }
                else
                {
                    result = (String)value;
                }
            }
            else if (typeof(Int16) == value.GetType())
            {
                result = (Int16)value;
            }
            else if (typeof(Int32) == value.GetType())
            {
                result = (Int32)value;
            }
            else if (typeof(Int64) == value.GetType())
            {
                result = (Int64)value;
            }
            else if (typeof(Boolean) == value.GetType())
            {
                result = (Boolean)value;
            }
            else if (typeof(Single) == value.GetType())
            {
                result = (Single)value;
            }
            else if (typeof(Double) == value.GetType())
            {
                result = (Double)value;
            }
            else
            {
                if (Properties.Settings.Default.UseCompression)
                {
                    byte[] tmp = Utility.Serialize(value);      //If not supported Serialize Object...
                    result = Utility.Deflate(tmp, System.IO.Compression.CompressionMode.Compress);
                    statusItem = StatusItem.Compressed | StatusItem.Serialized;
                }
                else
                {
                    result = Utility.Serialize(value);      //If not supported Serialize Object...
                    statusItem = StatusItem.Serialized;
                }
            }
            return result;
        }

        internal static StatusItem StatusItemDeSerialize(string v)
        {
            string r = ((string)v).Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[4];
            StatusItem result = (StatusItem)Enum.Parse(typeof(StatusItem), r, true);
            return result;
        }

        /// <summary>
        /// Convert to string DateTime and TimeStamp. 
        /// Calculate new DateTime relative to TimeStamp
        /// </summary>
        /// <param name="ttlSLI"></param>
        /// <param name="ttlABS"></param>
        /// <param name="forceUpdateDtABS"></param>
        /// <returns></returns>
        internal static string TTLSerialize(TimeSpan ttlSLI, TimeSpan ttlABS, DateTime forceUpdateDtABS, Utility.StatusItem statusItem)
        {
            DateTime dtResult = DateTime.Now.ToUniversalTime().Add(ttlSLI);     //Convert to UTC and add TimeStamp to Sliding Datetime
            string str_dtSLI = dtResult.ToString("yyMMddTHHmmss");            //Convert to String. For Datetime Format is: yyMMddTHHmmss
            string str_tsSLI = ttlSLI.TotalSeconds.ToString();                  //Convert to String. Fot TimeStamp Format is: hhmmss

            if (ttlSLI == Utility.NO_EXPIRATION)
            {
                str_dtSLI = _No_TTL;
                str_tsSLI = _No_TTL;
            }

            dtResult = DateTime.Now.ToUniversalTime().Add(ttlABS);              //Convert to UTC and add TimeStamp to Absolute Datetime
            string str_dtABS = dtResult.ToString("yyMMddTHHmmss");            //Convert to String. For Datetime Format is: yyMMddTHHmmss
            string str_tsABS = ttlABS.TotalSeconds.ToString();                  //Convert to String. Fot TimeStamp Format is: hhmmss
            if (ttlABS == Utility.NO_EXPIRATION)
            {
                str_dtABS = _No_TTL;
                str_tsABS = _No_TTL;
            }

            if (forceUpdateDtABS != DateTime.MaxValue)                          //This check is necessary if update Sliding TTL (call from "Get Function"). Reset Absolute Datetime.
            {
                str_dtABS = forceUpdateDtABS.ToString("yyMMddTHHmmss");
            }

            string strResult = str_dtSLI + "|" + str_tsSLI + "|" + str_dtABS + "|" + str_tsABS + "|" + Convert.ToInt32(statusItem);     //Concatenate string: Sliding DateTime + Sliding TTL + Absolute DateTime + Absolute TTL 
            return strResult;
        }

        /// <summary>
        /// Convert string to TimeStamp
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        internal static TimeSpan[] TTL_TS_DeSerialize(string ttl)
        {
            if (string.IsNullOrWhiteSpace(ttl))
            {
                throw new ArgumentException("Parameter is invalid.", "ttl", null);
            }

            string[] dts = ttl.Split(new char[] { '|' });               //Read and split string. Convert to Array -> Sliding DateTime | Sliding TTL | Absolute DateTime | Absolute TTL
            TimeSpan tsSLI = Utility.NO_EXPIRATION;                     //Set Default value for Sliding TTL
            TimeSpan tsABS = Utility.NO_EXPIRATION;                     //Set Default value for Absolute TTL

            if (string.Compare(dts[1], _No_TTL, true) != 0)
            {
                //tsSLI =  TimeSpan.ParseExact(dts[1], "hhmmss", null);   //Convert to TimeStamp
                tsSLI = TimeSpan.FromSeconds(Convert.ToInt32(dts[1]));
            }
            if (string.Compare(dts[3], _No_TTL, true) != 0)
            {
                //tsABS = TimeSpan.ParseExact(dts[3], "hhmmss", null);    //Convert to TimeStamp
                tsABS = TimeSpan.FromSeconds(Convert.ToInt32(dts[3]));
            }

            TimeSpan[] result = new TimeSpan[2] { tsSLI, tsABS };
            return result;
        }

        internal static DateTime[] TTL_DT_DeSerialize(string ttl)
        {
            if (string.IsNullOrWhiteSpace(ttl))
            {
                throw new ArgumentException("Parameter is invalid.", "ttl", null);
            }

            string[] dts = ttl.Split(new char[] { '|' });           //Read and split string. Convert to Array -> Sliding DateTime | Sliding TTL | Absolute DateTime | Absolute TTL
            DateTime dtSLI = DateTime.MaxValue;                     //Set Default value for Sliding DateTime
            DateTime dtABS = DateTime.MaxValue;                     //Set Default value for Absolute DateTime

            if (string.Compare(dts[0], _No_TTL, true) != 0)
            {
                dtSLI = DateTime.ParseExact(dts[0], "yyMMddTHHmmss", null);       //Convert to DateTime
            }
            if (string.Compare(dts[2], _No_TTL, true) != 0)
            {
                dtABS = DateTime.ParseExact(dts[2], "yyMMddTHHmmss", null);       //Convert to DateTime
            }

            DateTime[] result = new DateTime[2] { dtSLI, dtABS };
            return result;
        }

        #region Check TTL_Is_Expired
        /// <summary>
        /// Utility function for check if Item Cache is Expired.
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        internal static bool TTL_Is_Expired(DateTime slidingExpiration_DT, DateTime absoluteExpiration_DT)
        {
            DateTime dtNow = DateTime.Now.ToUniversalTime();
            if ((dtNow > slidingExpiration_DT) || (dtNow > absoluteExpiration_DT))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region Compress / DeCompress
        internal static byte[] Deflate(byte[] source_byte, System.IO.Compression.CompressionMode type_deflate)
        {
            byte[] dest_byte = null;

            using (System.IO.MemoryStream dest_mem = new System.IO.MemoryStream())
            {
                using (System.IO.MemoryStream source_mem = new System.IO.MemoryStream(source_byte))
                {
                    if (source_mem.CanSeek)
                    {
                        source_mem.Seek(0, System.IO.SeekOrigin.Begin);
                    }

                    if (type_deflate == System.IO.Compression.CompressionMode.Compress)
                    {
                        using (System.IO.Compression.DeflateStream deflate = new System.IO.Compression.DeflateStream(dest_mem, type_deflate))
                        {
                            source_mem.CopyTo(deflate);
                            deflate.Flush();
                        }
                    }
                    else
                    {
                        using (System.IO.Compression.DeflateStream deflate = new System.IO.Compression.DeflateStream(source_mem, type_deflate))
                        {
                            deflate.CopyTo(dest_mem);
                            deflate.Flush();
                        }
                    }

                    source_mem.Flush();
                }
                if (dest_mem.CanSeek)
                {
                    dest_mem.Seek(0, System.IO.SeekOrigin.Begin);
                }
                dest_byte = dest_mem.ToArray();
                dest_mem.Flush();
            }

            return dest_byte;
        }
        #endregion

        #region Serialize / DeSerialize
        internal static byte[] Serialize(object obj)
        {
            if (obj==null)
            {
                throw new ArgumentException("Parameter is invalid.", "obj", null);
            }
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                bf.Serialize(ms, obj);
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                byte[] result = ms.ToArray();
                ms.Flush();
                return result;
            }
        }

        internal static object DeSerialize(byte[] obj)
        {
            if (obj == null || obj.Length<=0)
            {
                throw new ArgumentException("Parameter is invalid.", "obj", null);
            }
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(obj))
            {
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                ms.Seek(0, System.IO.SeekOrigin.Begin);
                object result = bf.Deserialize(ms);
                ms.Flush();
                return result;
            }
        }
        #endregion
    }
}
