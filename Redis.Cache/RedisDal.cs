using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redis.Cache
{
    class RedisDal
    {
        private readonly static StackExchange.Redis.ConnectionMultiplexer _cm = StackExchange.Redis.ConnectionMultiplexer.Connect(Properties.Settings.Default.RedisConnectionString);
        private StackExchange.Redis.IDatabase _db = null;
        public RedisDal()
        {
            _db = _cm.GetDatabase(Properties.Settings.Default.RedisDatabase);
        }

        public bool SetTTL(string key, TimeSpan ttl)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Parameter is invalid.", "key", null);
            }

            try
            {
                return _db.KeyExpire(key, ttl);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { }
        }
        public bool DeleteTTL(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Parameter is invalid.", "key", null);
            }

            try
            {
                return _db.KeyPersist(key);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { }
        }

        public bool ItemDelete(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Parameter is invalid.", "key", null);
            }

            try
            {
                bool result = _db.KeyDelete(key);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { }
        }

        public bool ItemExist(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Parameter is invalid.", "key", null);
            }

            try
            {
                return _db.KeyExists(key);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { }
        }

        #region List Methods
        public long AddListItem(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Parameter is invalid.", "key or value", null);
            }

            try
            {
                long result = _db.ListRightPush(key, new StackExchange.Redis.RedisValue[] { value });
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { }
        }
        public long UpdateTTL_ListItem(string key, string ttl)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(ttl))
            {
                throw new ArgumentException("Parameter is invalid.", "key or ttl", null);
            }

            try
            {
                StackExchange.Redis.RedisValue[] _v = new StackExchange.Redis.RedisValue[1];
                _v[0] = ttl;
                _db.ListSetByIndex(key, 0, _v[0]);
                return 1;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { }
        }
        public long AddListItem(string key, object value, string value_ttl)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value_ttl))
            {
                throw new ArgumentException("Parameter is invalid.", "key or value_ttl", null);
            }
            if (value == null)
            {
                throw new ArgumentException("Parameter is invalid.", "value", null);
            }

            try
            {
                StackExchange.Redis.RedisValue[] _v = new StackExchange.Redis.RedisValue[2];
                _v[0] = value_ttl;
                _v[1] = Utility.ConvertObjToRedisValue(value);
                return _db.ListRightPush(key, _v);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { }
        }
        public object[] GetListItem(string key, Type type)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Parameter is invalid.", "key", null);
            }

            try
            {
                StackExchange.Redis.RedisValue[] values = _db.ListRange(key, 0, 1);
                if (values != null && values.Length > 0)
                {
                    object[] results = new object[2];
                    results[0] = Utility.ConvertRedisValueToObject(values[0], typeof(string));
                    results[1] = Utility.ConvertRedisValueToObject(values[1], type);
                    return results; 
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { }
        }
        #endregion

        #region Hash Methods
        public bool AddHashItem(string key, string key_hash, byte[] value)
        {
            return _db.HashSet(key, key_hash, value);
        }
        public bool AddHashItem(string key, string key_hash, string value)
        {
            return _db.HashSet(key, key_hash, value);
        }
        #endregion

        #region Key / Value Methods
        public bool AddItem(string key, byte[] value)
        {
            return _db.StringSet(key, value);
        }
        public bool AddItem(string key, string value)
        {
            return _db.StringSet(key, value);
        }
        #endregion
    }
}
