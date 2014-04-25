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

        private TimeSpan _SlidingExpiration = Properties.Settings.Default.DefaultSlidingExpiration;
        private TimeSpan _AbsoluteExpiration = Properties.Settings.Default.DefaultAbsoluteExpiration;
        

        public ItemCache()
        { }
        public ItemCache(string key, T value)
        {
            _Key = key;
            _Value = value;
        }
        public ItemCache(string key, T value, TimeSpan slidingExpiration, TimeSpan absoluteExpiration)
        {
            _Key = key;
            _Value = value;
            _SlidingExpiration = slidingExpiration;
            _AbsoluteExpiration = absoluteExpiration;
        }

        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        public T Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public TimeSpan AbsoluteExpiration
        {
            get { return _AbsoluteExpiration; }
            set { _AbsoluteExpiration = value; }
        }
        public TimeSpan SlidingExpiration
        {
            get { return _SlidingExpiration; }
            set { _SlidingExpiration = value; }
        }
        public long Save(bool forceOverWrite)
        {
            ManagementItemsCache m = new ManagementItemsCache();
            long result = m.Add<T>(this.Key, this.Value, this.SlidingExpiration, this.AbsoluteExpiration, forceOverWrite);
            return result;
        }
        public static long AddItem(ItemCache<T> itemsCache, bool forceOverWrite)
        {
            return itemsCache.Save(forceOverWrite);
        }
        public static long AddItem(string key, T value, TimeSpan slidingExpiration, TimeSpan absoluteExpiration, bool forceOverWrite)
        {
            ItemCache<T> item = new ItemCache<T>();
            item.AbsoluteExpiration = absoluteExpiration;
            item.Key = key;
            item.SlidingExpiration = slidingExpiration;
            item.Value = value;
            return item.Save(forceOverWrite);
        }
        public static long AddItem(string key, T value, bool forceOverWrite)
        {
            ItemCache<T> item = new ItemCache<T>();
            item.Key = key;
            item.Value = value;
            return item.Save(forceOverWrite);
        }
        public static bool DeleteItem(string key)
        {
            ManagementItemsCache m = new ManagementItemsCache();
            return m.Delete(key);
        }
        public static bool ExistItem(string key)
        {
            ManagementItemsCache m = new ManagementItemsCache();
            return m.Exist(key);
        }
        public static ItemCache<T> GetItem(string key)
        {
            ManagementItemsCache m = new ManagementItemsCache();
            return m.GetItemCache<T>(key);
        }
    }
}
