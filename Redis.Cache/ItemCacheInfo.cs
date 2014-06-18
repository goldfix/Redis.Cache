using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redis.Cache
{
    class ItemCacheInfo<T>
    {
        private void _Init()
        {
            this.TS_SlidingExpiration = Utility.NO_EXPIRATION;
            this.TS_AbsoluteExpiration = Utility.NO_EXPIRATION;
            this.DT_SlidingExpiration = DateTime.MaxValue;
            this.DT_AbsoluteExpiration = DateTime.MaxValue;
            this.StatusItem = Utility.StatusItem.None;
            this.Data = default(T);
        }

        public ItemCacheInfo()
        {
            _Init();
        }

        public ItemCacheInfo(string info, T data)
        {
            _Init();

            TimeSpan[] ttl_Ts = Utility.TTL_TS_DeSerialize(info);
            DateTime[] ttl_Dt = Utility.TTL_DT_DeSerialize(info);
            
            this.TS_SlidingExpiration = ttl_Ts[0];
            this.TS_AbsoluteExpiration = ttl_Ts[1];
            this.DT_SlidingExpiration = ttl_Dt[0];
            this.DT_AbsoluteExpiration = ttl_Dt[1];

            this.StatusItem = Utility.StatusItemDeSerialize(info);

            this.Data = data;
        }

        public TimeSpan TS_SlidingExpiration
        { get; set; }
        public TimeSpan TS_AbsoluteExpiration
        { get; set; }
        public DateTime DT_SlidingExpiration
        { get; set; }
        public DateTime DT_AbsoluteExpiration
        { get; set; }
        public Utility.StatusItem StatusItem
        { get; set; }
        public T Data
        { get; set; }

        public string SerializedInfo
        { get; set; }
        public StackExchange.Redis.RedisValue SerializedData
        { get; set; }

        public void SerializeInfo()
        {
            Utility.StatusItem _StatusItem = Utility.StatusItem.None;
            this.SerializedData = Utility.ConvertObjToRedisValue(this.Data, out _StatusItem);
            this.StatusItem = _StatusItem;

            this.SerializedInfo = Utility.TTLSerialize(this.TS_SlidingExpiration, this.TS_AbsoluteExpiration, this.DT_AbsoluteExpiration);
        }
        public void DeSerializeInfo()
        {
            TimeSpan[] ttl_Ts = Utility.TTL_TS_DeSerialize(this.SerializedInfo);
            DateTime[] ttl_Dt = Utility.TTL_DT_DeSerialize(this.SerializedInfo);

            this.TS_SlidingExpiration = ttl_Ts[0];
            this.TS_AbsoluteExpiration = ttl_Ts[1];
            this.DT_SlidingExpiration = ttl_Dt[0];
            this.DT_AbsoluteExpiration = ttl_Dt[1];

            this.StatusItem = Utility.StatusItemDeSerialize(this.SerializedInfo);

            this.Data = (T)Utility.ConvertRedisValueToObject(this.SerializedData, typeof(T), this.StatusItem);
        }
    }
}
