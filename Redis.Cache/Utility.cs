using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redis.Cache
{
    public static class Utility
    {
        private readonly static string _No_TTL = "ND";     // new DateTime(1985, 10, 26, 06, 0, 0).ToString("yyyyMMddTHHmmss");

        public static readonly TimeSpan NO_EXPIRATION = TimeSpan.Zero;

        public enum TypeStorage : short
        {
            UseList=1,
            UseKeyValue=2,
            UseHash=3
        }

        static Utility()
        { }

        /// <summary>
        /// Check and convert native type supported from Redis.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object ConvertRedisValueToObject(StackExchange.Redis.RedisValue value, Type type)
        {
            object result = null;
            if (typeof(Byte[]) == type)
            {
                if (Properties.Settings.Default.UseCompression)
                {
                    result = Utility.Deflate((Byte[])value, System.IO.Compression.CompressionMode.Decompress);
                }
                else
                {
                    result = (Byte[])value;
                }
            }
            else if (typeof(String) == type)
            {
                if (Properties.Settings.Default.UseCompression)         //&& ((String)value).Length>512
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
                if (Properties.Settings.Default.UseCompression)
                {
                    byte[] tmp = Utility.Deflate((Byte[])value, System.IO.Compression.CompressionMode.Decompress);
                    result = Utility.DeSerialize(tmp);          //If not supported De-Serialize Object...
                }
                else
                {
                    result = Utility.DeSerialize((Byte[])value);          //If not supported De-Serialize Object...
                }
            }
            return result;
        }
        /// <summary>
        /// Check and convert native type supported from Redis.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static StackExchange.Redis.RedisValue ConvertObjToRedisValue(object value)
        {
            StackExchange.Redis.RedisValue result = StackExchange.Redis.RedisValue.Null;
            if (typeof(Byte[]) == value.GetType())
            {   
                if (Properties.Settings.Default.UseCompression)
                {
                    result = Utility.Deflate((Byte[])value, System.IO.Compression.CompressionMode.Compress);
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
                }
                else
                {
                    result = Utility.Serialize(value);      //If not supported Serialize Object...
                }
            }
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
        internal static string TTLSerialize(TimeSpan ttlSLI, TimeSpan ttlABS, DateTime forceUpdateDtABS)
        {
            DateTime dtResult = DateTime.Now.ToUniversalTime().Add(ttlSLI);     //Convert to UTC and add TimeStamp to Sliding Datetime
            string str_dtSLI = dtResult.ToString("yyyyMMddTHHmmss");            //Convert to String. For Datetime Format is: yyyyMMddTHHmmss
            string str_tsSLI = ttlSLI.ToString("hhmmss");                       //Convert to String. Fot TimeStamp Format is: hhmmss

            if (ttlSLI == Utility.NO_EXPIRATION)
            {
                str_dtSLI = _No_TTL;
                str_tsSLI = _No_TTL;
            }

            dtResult = DateTime.Now.ToUniversalTime().Add(ttlABS);              //Convert to UTC and add TimeStamp to Absolute Datetime
            string str_dtABS = dtResult.ToString("yyyyMMddTHHmmss");            //Convert to String. For Datetime Format is: yyyyMMddTHHmmss
            string str_tsABS = ttlABS.ToString("hhmmss");                       //Convert to String. Fot TimeStamp Format is: hhmmss
            if (ttlABS == Utility.NO_EXPIRATION)
            {
                str_dtABS = _No_TTL;
                str_tsABS = _No_TTL;
            }

            if (forceUpdateDtABS != DateTime.MaxValue)                          //This check is necessary if update Sliding TTL (call from "Get Function"). Reset Absolute Datetime.
            {
                str_dtABS = forceUpdateDtABS.ToString("yyyyMMddTHHmmss");
            }

            string strResult = str_dtSLI + "|" + str_tsSLI + "|" + str_dtABS + "|" + str_tsABS;     //Concatenate string: Sliding DateTime + Sliding TTL + Absolute DateTime + Absolute TTL 
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
                tsSLI =  TimeSpan.ParseExact(dts[1], "hhmmss", null);   //Convert to TimeStamp
            }
            if (string.Compare(dts[3], _No_TTL, true) != 0)
            {
                tsABS = TimeSpan.ParseExact(dts[3], "hhmmss", null);    //Convert to TimeStamp
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
                dtSLI = DateTime.ParseExact(dts[0], "yyyyMMddTHHmmss", null);       //Convert to DateTime
            }
            if (string.Compare(dts[2], _No_TTL, true) != 0)
            {
                dtABS = DateTime.ParseExact(dts[2], "yyyyMMddTHHmmss", null);       //Convert to DateTime
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
        internal static bool TTL_Is_Expired(string ttl)
        {
            DateTime[] dt = Utility.TTL_DT_DeSerialize(ttl);
            return TTL_Is_Expired(dt);
        }
        /// <summary>
        /// Utility function for check if Item Cache is Expired.
        /// </summary>
        /// <param name="ttl"></param>
        /// <returns></returns>
        internal static bool TTL_Is_Expired(DateTime[] ttl)
        {
            DateTime dtNow = DateTime.Now.ToUniversalTime();
            if ((dtNow > ttl[0]) || (dtNow > ttl[1]))
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
