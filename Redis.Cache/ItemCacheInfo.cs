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

            this.UpdateSerialized_TTL();    //Update TTL and generate new ttl string.
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
        public void UpdateSerialized_TTL()
        {
            this.Serialized_TTL = Utility.TTLSerialize(this.SlidingExpiration_TS, this.AbsoluteExpiration_TS, this.AbsoluteExpiration_DT, this.StatusItem);
        }
    }
}
