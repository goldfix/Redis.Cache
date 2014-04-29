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
            DateTime dt = DateTime.Now;
            Redis.Cache.ItemCache<DateTime> ic = new ItemCache<DateTime>("dt_1", dt);
            ic.Save(true);

            ItemCache<DateTime> dt_result = Redis.Cache.ItemCache<DateTime>.GetItem("dt_1");

            System.Console.WriteLine((dt == dt_result.Value).ToString());
        }
    }
}
