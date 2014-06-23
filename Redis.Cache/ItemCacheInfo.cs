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
            this.SlidingExpiration_TS = Properties.Settings.Default.DefaultSlidingExpiration;
            this.AbsoluteExpiration_TS = Properties.Settings.Default.DefaultAbsoluteExpiration;
            this.SlidingExpiration_DT = DateTime.MaxValue;
            this.AbsoluteExpiration_DT = DateTime.MaxValue;
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

            this.SlidingExpiration_TS = ttl_Ts[0];
            this.AbsoluteExpiration_TS = ttl_Ts[1];
            this.SlidingExpiration_DT = ttl_Dt[0];
            this.AbsoluteExpiration_DT = ttl_Dt[1];

            this.StatusItem = Utility.StatusItemDeSerialize(info);

            this.Data = data;
        }

        public TimeSpan SlidingExpiration_TS
        { get; set; }
        public TimeSpan AbsoluteExpiration_TS
        { get; set; }
        public DateTime SlidingExpiration_DT
        { get; set; }
        public DateTime AbsoluteExpiration_DT
        { get; set; }
        public Utility.StatusItem StatusItem
        { get; set; }
        public T Data
        { get; set; }
        
        public string Serialized_TTL
        { get; set; }

        public StackExchange.Redis.RedisValue Serialized_Data
        { get; set; }



        public void SerializeInfo()
        {
            Utility.StatusItem _StatusItem = Utility.StatusItem.None;
            this.Serialized_Data = Utility.ConvertObjToRedisValue(this.Data, out _StatusItem);
            this.StatusItem = _StatusItem;

            this.Serialized_TTL = Utility.TTLSerialize(this.SlidingExpiration_TS, this.AbsoluteExpiration_TS, this.AbsoluteExpiration_DT);
        }
        public void DeSerializeInfo()
        {
            TimeSpan[] ttl_Ts = Utility.TTL_TS_DeSerialize(this.Serialized_TTL);
            DateTime[] ttl_Dt = Utility.TTL_DT_DeSerialize(this.Serialized_TTL);

            this.SlidingExpiration_TS = ttl_Ts[0];
            this.AbsoluteExpiration_TS = ttl_Ts[1];
            this.SlidingExpiration_DT = ttl_Dt[0];
            this.AbsoluteExpiration_DT = ttl_Dt[1];

            this.StatusItem = Utility.StatusItemDeSerialize(this.Serialized_TTL);

            this.Data = (T)Utility.ConvertRedisValueToObject(this.Serialized_Data, typeof(T), this.StatusItem);
        }
    }
}
