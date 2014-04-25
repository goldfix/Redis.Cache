using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redis.Cache
{
    public class RedisCacheException:Exception
    {
        public RedisCacheException(string mex, Exception inner)
            : base(mex, inner)
        { }
    }
}
