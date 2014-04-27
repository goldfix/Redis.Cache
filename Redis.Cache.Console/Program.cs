using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Cache.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Redis.Cache.ItemCache<string> t = new ItemCache<string>();

        }
    }
}
