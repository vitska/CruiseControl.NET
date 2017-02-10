using System;
using System.Collections;
using System.Collections.Generic;

namespace Cruise.DashboardBroker {
    public class ServerBase : EntityBase, IServer {
        readonly string _name;
        public string Name {
            get { return _name; }
        }

        readonly IDictionary<string,object> _staticObject;

        public ServerBase(string name,ServerPerformance perf) : base(name.GetHashCode().ToString("x")) {
            _name = name;
            _perf = perf;
            _staticObject = new Dictionary<string,object> {
                { "id", Id },
                { "name", _name },
                { "dtb", _dateBase }
            };
        }

        public virtual IDictionary<string,object> StaticObject { get {
            return _staticObject;
        } }

        protected readonly ServerPerformance _perf;
        public object Perf { get {
            return (_perf == null) ? new object() : _perf.GetChanged();
        } }

        public virtual IDictionary<string,object> GetChanges(ulong hash) {
            return new Dictionary<string,object> {
                { "perf", Perf }
            };
        }

        static int _dateBase = (int)DateTime.Now.ToUnixTime();

        public int ToTimeCode(DateTime val) {
            return (int)val.ToUnixTime() - _dateBase;
        }

        public override ulong Change { get {
            return DateTime.Now.ToHash();  
        //    return Math.Max( _lastchange.ToHash(), (_perf == null)? 0 : _perf.Change ); 
        } }

    }
}
