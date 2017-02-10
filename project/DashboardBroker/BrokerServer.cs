using System;

namespace Cruise.DashboardBroker {
    public class BrokerServer:ServerBase,IChangeable {
        public const string DefaultId = "__broker";

        //public object GetChanges(ulong hash) {
        //    return new {
        //        perf = Perf
        //    };
        //}

        //DateTime _lastchange = UtilExtensions.HashFixed;
        public override ulong Change { get {  return Math.Max( _lastchange.ToHash(), _perf.Change ); } }

        public BrokerServer() : base(string.Format("Broker[{0}]", Environment.MachineName), new LocalServerPerformance()) {
        }

    }
}
