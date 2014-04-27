using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redis.Cache
{
    public class RedisCacheException:Exception
    {
        /// <summary>
        /// Custom Redis.Cache Exception 
        /// </summary>
        /// <param name="mex"></param>
        /// <param name="inner"></param>
        public RedisCacheException(string mex, Exception inner)
            : base(mex, inner)
        { }
    }
}
