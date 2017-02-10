using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cruise.DashboardBroker {
    public class ServerPerformance : IChangeable {
        static readonly DateTime _startup = DateTime.Now;

        protected object _locker = new object();

        public DateTime Startup { get { return _startup; } }
        public UInt64 UpTimeS { get { return Convert.ToUInt64( (DateTime.Now - _startup).TotalSeconds ); } }
        public virtual DateTime Time { get; }

        protected byte _cpu;
        public byte Cpu { get { lock(_locker) { return _cpu; } } }

        protected byte _memory;
        public byte Memory { get { lock(_locker) { return _memory; } } }

        protected byte _disk;
        public byte Disk { get { lock(_locker) { return _disk; } } }

        protected DateTime _lastchange = UtilExtensions.HashFixed;
        public ulong Change { get { lock(_locker) { return _lastchange.ToHash(); } } }

        public object GetChanged() {
            return new Dictionary<string,object> {
                { "c", Cpu },
                { "m", Memory },
                { "d", Disk },
                { "u", UpTimeS }
            };
        }
    }
}
