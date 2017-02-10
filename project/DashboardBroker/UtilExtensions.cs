using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cruise.DashboardBroker {
    public static class UtilExtensions {
        public static readonly DateTime HashFixed = new DateTime(2016,1,1);
        public static ulong ToHash(this DateTime value) {
            return Convert.ToUInt64( (value - HashFixed).TotalSeconds );
        }

        public static Dictionary<string,object> ToIdDictionary<TResult>(this IEnumerable<IIdentifable> list, Func<IIdentifable,TResult> selector) {
            var res = new Dictionary<string,object>();
            foreach(var i in list) res[i.Id] = selector(i);
            return res;
        }

        public static uint ToUnixTime(this DateTime val) {
            return (uint)(val.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))).TotalSeconds;
        }
    }
}
