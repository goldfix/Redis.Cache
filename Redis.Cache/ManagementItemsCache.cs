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
    class ManagementItemsCache
    {
        #region Private Fields
        private Utility.TypeStorage _TypeStorage = (Utility.TypeStorage)Enum.Parse(typeof(Utility.TypeStorage), Properties.Settings.Default.TypeStorage, true);
        private TimeSpan _DefaultSlidingExpiration = Properties.Settings.Default.DefaultSlidingExpiration;
        private TimeSpan _DefaultAbsoluteExpiration = Properties.Settings.Default.DefaultAbsoluteExpiration;
        #endregion

        #region Public Constructors
        public ManagementItemsCache()
        {
            
        }
        #endregion

        #region Public Properties
        public TimeSpan DefaultAbsoluteExpiration
        {
            get
            {
                return _DefaultAbsoluteExpiration;
            }
            private set
            {
                _DefaultAbsoluteExpiration = value;
            }
        }

        public TimeSpan DefaultSlidingExpiration
        {
            get
            {
                return _DefaultSlidingExpiration;
            }
            private set
            {
                _DefaultSlidingExpiration = value;
            }
        }
        #endregion

        private bool _SetTTL(string key, TimeSpan slidingExpiration, TimeSpan absoluteExpiration, RedisDal dal)
        {
            bool result = false;
            if (slidingExpiration != Utility.NO_EXPIRATION || absoluteExpiration != Utility.NO_EXPIRATION)
            {
                if (slidingExpiration != Utility.NO_EXPIRATION)
                {
                    //SET TTL
                    result = dal.SetTTL(key, slidingExpiration);
                }
                else
                {
                    if (absoluteExpiration != Utility.NO_EXPIRATION)
                    {
                        //SET TTL
                        result = dal.SetTTL(key, absoluteExpiration);
                    }
                    else
                    {
                        //ND
                    }
                }
            }
            else
            {
                //ND
            }
            return result;
        }

        public long Add<T>(string key, T value, TimeSpan slidingExpiration, TimeSpan absoluteExpiration, bool forceOverWrite)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Parameter is invalid.", "key", null);
            }
            if ((slidingExpiration != Utility.NO_EXPIRATION && absoluteExpiration != Utility.NO_EXPIRATION) && (slidingExpiration >= absoluteExpiration))
            {
                throw new RedisCacheException("Sliding Expiration is greater or equal than Absolute Expiration.", null);
            }

            RedisDal dal = new RedisDal();
            if (_TypeStorage == Utility.TypeStorage.UseList)
            {
                if (!forceOverWrite)
                {
                    if (dal.ItemExist(key))
                    {
                        throw new RedisCacheException("This Item Exists.", null);
                    }
                    else
                    {
                        //Continue...
                    }
                }
                else
                {
                    if (dal.ItemExist(key))
                    {
                        dal.ItemDelete(key);
                    }
                    else
                    {
                        //Continue...
                    }
                }

                ItemCacheInfo<T> itemCacheInfo = new ItemCacheInfo<T>();
                itemCacheInfo.Data = value;
                itemCacheInfo.AbsoluteExpiration_TS = absoluteExpiration;
                itemCacheInfo.SlidingExpiration_TS = slidingExpiration;
                itemCacheInfo.SerializeInfo();

                long result =  dal.AddListItem(key, itemCacheInfo.Serialized_Data, itemCacheInfo.Serialized_TTL);
                _SetTTL(key, slidingExpiration, absoluteExpiration, dal);
                return result;
            }
            else if (_TypeStorage == Utility.TypeStorage.UseHash)
            {
                throw new System.NotImplementedException();
            }
            if (_TypeStorage == Utility.TypeStorage.UseKeyValue)
            {
                throw new System.NotImplementedException();
            }
            else
            {
                throw new System.NotImplementedException();
            }
        }
        /// <summary>
        /// Return a single value. Use Internal Unit Test.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Parameter is invalid.", "key", null);
            }

            ItemCache<T> result = GetItemCache<T>(key);
            if (result == null)
            {
                return default(T);
            }
            else
            {
                return result.Value;
            }
        }
        public ItemCache<T> GetItemCache<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Parameter is invalid.", "key", null);
            }

            RedisDal dal = new RedisDal();
            StackExchange.Redis.RedisValue[] results = dal.GetListItem<T>(key);

            if (results != null && results.Length > 1)
            {
                ItemCacheInfo<T> itemCacheInfo = new ItemCacheInfo<T>();
                itemCacheInfo.Serialized_TTL = (string)results[0];
                itemCacheInfo.Serialized_Data = results[1];
                itemCacheInfo.DeSerializeInfo();

                if (Utility.TTL_Is_Expired(itemCacheInfo.SlidingExpiration_DT, itemCacheInfo.AbsoluteExpiration_DT))
                {
                    dal.ItemDelete(key);
                    return null;
                }
                else
                {
                    //Update SLI TTL on Redis...
                    if (itemCacheInfo.SlidingExpiration_DT != DateTime.MaxValue)
                    {
                        itemCacheInfo.UpdateSerialized_TTL();       //Update TTL

                        dal.UpdateTTL_Item(key, itemCacheInfo.Serialized_TTL);
                        dal.SetTTL(key, itemCacheInfo.SlidingExpiration_TS);
                    }
                    ItemCache<T> result = new ItemCache<T>();
                    result.SlidingExpiration = itemCacheInfo.SlidingExpiration_TS;
                    result.AbsoluteExpiration = itemCacheInfo.AbsoluteExpiration_TS;
                    result.Key = key;
                    result.Value = itemCacheInfo.Data;
                    return result;
                }
            }
            else
            {
                return null;
            }
        }

        public bool Exist(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Parameter is invalid.", "key", null);
            }

            RedisDal dal = new RedisDal();
            return dal.ItemExist(key);
        }

        public bool Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Parameter is invalid.", "key", null);
            }

            RedisDal dal = new RedisDal();
            return dal.ItemDelete(key);
        }
    }
}
