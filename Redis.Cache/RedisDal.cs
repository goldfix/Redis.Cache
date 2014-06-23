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

        public long UpdateTTL_Item(string key, string ttl)
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

        public long AddListItem(string key, StackExchange.Redis.RedisValue value, string value_ttl)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value_ttl))
            {
                throw new ArgumentException("Parameter is invalid.", "key or value_ttl", null);
            }
            if (value == StackExchange.Redis.RedisValue.Null || value == StackExchange.Redis.RedisValue.EmptyString)
            {
                throw new ArgumentException("Parameter is invalid.", "value", null);
            }

            try
            {
                StackExchange.Redis.RedisValue[] _v = new StackExchange.Redis.RedisValue[2];
                _v[0] = value_ttl;
                _v[1] = value;
                return _db.ListRightPush(key, _v);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            { }
        }

        public StackExchange.Redis.RedisValue[] GetListItem<T>(string key)
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
                    return values;
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
