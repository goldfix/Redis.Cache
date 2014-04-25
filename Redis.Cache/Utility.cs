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


        internal static object ConvertRedisValueToObject(StackExchange.Redis.RedisValue value, Type type)
        {
            object result = null;
            if (typeof(Byte[]) == type)
            {
                result = (Byte[])value;
            }
            else if (typeof(String) == type)
            {
                result = (String)value;
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
                result = Utility.DeSerialize((Byte[])value);
            }
            return result;
        }

        internal static StackExchange.Redis.RedisValue ConvertObjToRedisValue(object value)
        {
            StackExchange.Redis.RedisValue result = StackExchange.Redis.RedisValue.Null;
            if (typeof(Byte[]) == value.GetType())
            {
                result = (Byte[])value;
            }
            else if (typeof(String) == value.GetType())
            {
                result = (String)value;
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
                result = Utility.Serialize(value);
            }
            return result;
        }

        internal static string TTLSerialize(TimeSpan ttlSLI, TimeSpan ttlABS, DateTime forceUpdateDtABS)
        {
            DateTime dtResult = DateTime.Now.ToUniversalTime().Add(ttlSLI);
            string str_dtSLI = dtResult.ToString("yyyyMMddTHHmmss");
            string str_tsSLI = ttlSLI.ToString("hhmmss");
            if (ttlSLI == Utility.NO_EXPIRATION)
            {
                str_dtSLI = _No_TTL;
                str_tsSLI = _No_TTL;
            }

            dtResult = DateTime.Now.ToUniversalTime().Add(ttlABS);
            string str_dtABS = dtResult.ToString("yyyyMMddTHHmmss");
            string str_tsABS = ttlABS.ToString("hhmmss");
            if (ttlABS == Utility.NO_EXPIRATION)
            {
                str_dtABS = _No_TTL;
                str_tsABS = _No_TTL;
            }
            if (forceUpdateDtABS != DateTime.MaxValue)
            {
                str_dtABS = forceUpdateDtABS.ToString("yyyyMMddTHHmmss");
            }
            string strResult = str_dtSLI + "|" + str_tsSLI + "|" + str_dtABS + "|" + str_tsABS;
            return strResult;
        }

        internal static TimeSpan[] TTL_TS_DeSerialize(string ttl)
        {
            if (string.IsNullOrWhiteSpace(ttl))
            {
                throw new ArgumentException("Parameter is invalid.", "ttl", null);
            }

            string[] dts = ttl.Split(new char[] { '|' });
            TimeSpan tsSLI = Utility.NO_EXPIRATION;
            TimeSpan tsABS = Utility.NO_EXPIRATION;

            if (string.Compare(dts[1], _No_TTL, true) != 0)
            {
                tsSLI =  TimeSpan.ParseExact(dts[1], "hhmmss", null);
            }
            if (string.Compare(dts[3], _No_TTL, true) != 0)
            {
                tsABS = TimeSpan.ParseExact(dts[3], "hhmmss", null);
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

            string[] dts = ttl.Split(new char[] { '|' });
            DateTime dtSLI = DateTime.MaxValue;
            DateTime dtABS = DateTime.MaxValue;

            if (string.Compare(dts[0], _No_TTL, true) != 0)
            {
                dtSLI = DateTime.ParseExact(dts[0], "yyyyMMddTHHmmss", null);
            }
            if (string.Compare(dts[2], _No_TTL, true) != 0)
            {
                dtABS = DateTime.ParseExact(dts[2], "yyyyMMddTHHmmss", null);
            }

            DateTime[] result = new DateTime[2] { dtSLI, dtABS };
            return result;
        }

        #region Check TTL_Is_Expired
        internal static bool TTL_Is_Expired(string ttl)
        {
            DateTime[] dt = Utility.TTL_DT_DeSerialize(ttl);
            return TTL_Is_Expired(dt);
        }
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
