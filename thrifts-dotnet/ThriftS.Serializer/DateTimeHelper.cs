using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThriftS.Serializer
{
    internal static class DateTimeHelper
    {
        public static long ToUnixTimestamp(DateTime target)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
            return Convert.ToInt64((target - date).TotalSeconds);
        }

        public static DateTime ToDateTime(long timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0);
            return dateTime.AddSeconds(timestamp);
        }
    }
}
