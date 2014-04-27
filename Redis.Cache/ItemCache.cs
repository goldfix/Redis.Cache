using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redis.Cache
{
    public class ItemCache<T>
    {
        private string _Key = string.Empty;
        private T _Value = default(T);

        /// <summary>
        /// Default configuration Sliding Expiration. Read from config file.
        /// </summary>
        private TimeSpan _SlidingExpiration = Properties.Settings.Default.DefaultSlidingExpiration;
        /// <summary>
        /// Default configuration Absolute Expiration. Read from config file.
        /// </summary>
        private TimeSpan _AbsoluteExpiration = Properties.Settings.Default.DefaultAbsoluteExpiration;
        
        /// <summary>
        /// Initialization new Item Cache Element.
        /// </summary>
        public ItemCache()
        { }
        /// <summary>
        /// Initialization new Item Cache Element.
        /// </summary>
        /// <param name="key">Cache Item key</param>
        /// <param name="value">Item Cache Generic value</param>
        public ItemCache(string key, T value)
        {
            _Key = key;
            _Value = value;
        }
        /// <summary>
        /// Initialization new Item Cache Element.
        /// </summary>
        /// <param name="key">Cache Item Key</param>
        /// <param name="value">Item Cache generic value</param>
        /// <param name="slidingExpiration">Sliding Expiration TimeStamp. For "No Sliding Expiration" use: Utility.NO_EXPIRATION</param>
        /// <param name="absoluteExpiration">Absolute Expiration TimeStamp. For "No Absolute Expiration" use: Utility.NO_EXPIRATION</param>
        public ItemCache(string key, T value, TimeSpan slidingExpiration, TimeSpan absoluteExpiration)
        {
            _Key = key;
            _Value = value;
            _SlidingExpiration = slidingExpiration;
            _AbsoluteExpiration = absoluteExpiration;
        }
        /// <summary>
        /// Item Cache Key
        /// </summary>
        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
        /// <summary>
        /// Item Cache generic value
        /// </summary>
        public T Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        /// <summary>
        /// Absolute Expiration TimeStamp. For "No Absolute Expiration" use: Utility.NO_EXPIRATION
        /// </summary>
        public TimeSpan AbsoluteExpiration
        {
            get { return _AbsoluteExpiration; }
            set { _AbsoluteExpiration = value; }
        }
        /// <summary>
        /// Sliding Expiration TimeStamp. For "No Sliding Expiration" use: Utility.NO_EXPIRATION
        /// </summary>
        public TimeSpan SlidingExpiration
        {
            get { return _SlidingExpiration; }
            set { _SlidingExpiration = value; }
        }
        /// <summary>
        /// Save new or updated Item Cache.
        /// </summary>
        /// <param name="forceOverWrite">
        /// If 'True', overwrite existing (if exists) Item Cache.
        /// If 'False', if exists Item Cache return exception (RedisCacheException).
        /// </param>
        /// <returns></returns>
        public long Save(bool forceOverWrite)
        {
            ManagementItemsCache m = new ManagementItemsCache();
            long result = m.Add<T>(this.Key, this.Value, this.SlidingExpiration, this.AbsoluteExpiration, forceOverWrite);
            return result;
        }
        /// <summary>
        /// Add new Item Cache or update existing Item Cache.
        /// </summary>
        /// <param name="itemsCache">Item Cache to add or uopdate.</param>
        /// <param name="forceOverWrite">
        /// If 'True', overwrite existing (if exists) Item Cache.
        /// If 'False', if exists Item Cache return exception (RedisCacheException).
        /// </param>
        /// <returns></returns>
        public static long AddItem(ItemCache<T> itemsCache, bool forceOverWrite)
        {
            return itemsCache.Save(forceOverWrite);
        }
        /// <summary>
        /// Add new Item Cache or update existing Item Cache.
        /// </summary>
        /// <param name="key">Item Cache Key</param>
        /// <param name="value">Item Cache generic value</param>
        /// <param name="slidingExpiration">Sliding Expiration TimeStamp. For "No Sliding Expiration" use: Utility.NO_EXPIRATION</param>
        /// <param name="absoluteExpiration">Absolute Expiration TimeStamp. For "No Absolute Expiration" use: Utility.NO_EXPIRATION</param>
        /// <param name="forceOverWrite">
        /// If 'True', overwrite existing (if exists) Item Cache.
        /// If 'False', if exists Item Cache return exception (RedisCacheException).
        /// </param>
        /// <returns></returns>
        public static long AddItem(string key, T value, TimeSpan slidingExpiration, TimeSpan absoluteExpiration, bool forceOverWrite)
        {
            ItemCache<T> item = new ItemCache<T>();
            item.AbsoluteExpiration = absoluteExpiration;
            item.Key = key;
            item.SlidingExpiration = slidingExpiration;
            item.Value = value;
            return item.Save(forceOverWrite);
        }
        /// <summary>
        /// Add new Item Cache or update existing Item Cache.
        /// </summary>
        /// <param name="key">Item Cache Key</param>
        /// <param name="value">Item Cache generic value</param>
        /// <param name="forceOverWrite">
        /// If 'True', overwrite existing (if exists) Item Cache.
        /// If 'False', if exists Item Cache return exception (RedisCacheException).
        /// </param>
        /// <returns></returns>
        public static long AddItem(string key, T value, bool forceOverWrite)
        {
            ItemCache<T> item = new ItemCache<T>();
            item.Key = key;
            item.Value = value;
            return item.Save(forceOverWrite);
        }
        /// <summary>
        /// Delete existing Item Cache.
        /// </summary>
        /// <param name="key">Item Cache Key</param>
        /// <returns></returns>
        public static bool DeleteItem(string key)
        {
            ManagementItemsCache m = new ManagementItemsCache();
            return m.Delete(key);
        }
        /// <summary>
        /// Verify if existing Item Cache.
        /// </summary>
        /// <param name="key">Item Cache Key</param>
        /// <returns></returns>
        public static bool ExistItem(string key)
        {
            ManagementItemsCache m = new ManagementItemsCache();
            return m.Exist(key);
        }
        /// <summary>
        /// Return (if exists) Item Cache.
        /// </summary>
        /// <param name="key">Item Cache Key</param>
        /// <returns></returns>
        public static ItemCache<T> GetItem(string key)
        {
            ManagementItemsCache m = new ManagementItemsCache();
            return m.GetItemCache<T>(key);
        }
    }
}
