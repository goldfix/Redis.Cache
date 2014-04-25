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
                throw new RedisCacheException("Sliding Expiration is greater than Absolute Expiration.", null);
            }

            RedisDal dal = new RedisDal();
            string ttl = Utility.TTLSerialize(slidingExpiration, absoluteExpiration, DateTime.MaxValue);
            if (_TypeStorage == Utility.TypeStorage.UseList)
            {
                if (!forceOverWrite)
                {
                    if (Exist(key))
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
                    if (Exist(key))
                    {
                        Delete(key);
                    }
                    else
                    {
                        //Continue...
                    }
                }

                long result =  dal.AddListItem(key, value, ttl);
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
            return GetItemCache<T>(key).Value;
        }
        public ItemCache<T> GetItemCache<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Parameter is invalid.", "key", null);
            }

            RedisDal dal = new RedisDal();
            object[] results = dal.GetListItem(key, typeof(T));

            if (results != null && results.Length > 0)
            {
                DateTime[] ttl_Dt = Utility.TTL_DT_DeSerialize((string)results[0]);
                TimeSpan[] ttl_Ts = Utility.TTL_TS_DeSerialize((string)results[0]);
                if (Utility.TTL_Is_Expired(ttl_Dt))
                {
                    dal.ItemDelete(key);
                    return null;
                }
                else
                {
                    //Update SLI TTL on Redis...
                    if (ttl_Dt[0] != DateTime.MaxValue)
                    {
                        string ttl = Utility.TTLSerialize(ttl_Ts[0], ttl_Ts[1], ttl_Dt[1]);
                        dal.UpdateTTL_ListItem(key, ttl);
                        dal.SetTTL(key, ttl_Ts[0]);
                    }
                    ItemCache<T> result = new ItemCache<T>();
                    result.SlidingExpiration = ttl_Ts[0];
                    result.AbsoluteExpiration = ttl_Ts[1];
                    result.Key = key;
                    result.Value = (T)results[1];
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
