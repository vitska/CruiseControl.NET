using System;

namespace Cruise.DashboardBroker {
    public class EntityBase : IIdentifable, IChangeable {
        readonly string _id;
        public virtual string Id {
            get { return _id; }
        }

        protected DateTime _lastchange = UtilExtensions.HashFixed;
        public virtual ulong Change { get {  return _lastchange.ToHash(); } }
        protected bool ResetLastChange(bool bif) {
            if(bif) {
                _lastchange = DateTime.Now;
            }
            return bif;
        }

        public EntityBase(string id) {
            _id = id;
        }
   }
}
