using System;
using System.Collections.Generic;

namespace Cruise.DashboardBroker {
    public class Project : EntityBase {
        readonly BuildServer _server;
        readonly string _name;
        public string Name { get { lock(_locker) { return _name; } } }
        readonly Dictionary<string,object> _staticObject;

        readonly object _locker = new object();

        int _avgBuildTime = -1;
        public bool AvgBuildTimeIsSet { get { return _avgBuildTime != -1; } }
        public int AvgBuildTime {
            get { lock(_locker) { return _avgBuildTime; } }
            set { lock (_locker) { if (ResetLastChange(_avgBuildTime != value)) _avgBuildTime = value; } }
        }

        DateTime _nextBuild = DateTime.UtcNow;
        public DateTime NextBuild { 
            get { lock(_locker) { return _nextBuild; } }  
            set { lock(_locker) { if(ResetLastChange(_nextBuild != value))_nextBuild = value; } }
        }

        string _label = string.Empty;
        public string Label
        {
            get { lock (_locker) { return _label; } }
            set { lock (_locker) { if (ResetLastChange(_label != value)) _label = value; } }
        }

        public enum BuildStatusType {Unknown = 0, Success = 1, Failure = 2};
        BuildStatusType _buildStatus = BuildStatusType.Unknown;
        public BuildStatusType BuildStatus { 
            get { lock(_locker) { return _buildStatus; } } 
            set { lock(_locker) { if(ResetLastChange(_buildStatus != value))_buildStatus = value; } } 
        }

        public enum CCStatusType {Unknown = 0, Stopped = 1, Stopping = 2, Running = 3};
        CCStatusType _ccStatus = CCStatusType.Unknown;
        public CCStatusType CCStatus { 
            get { lock(_locker) { return _ccStatus; } } 
            set { lock(_locker) { if(ResetLastChange(_ccStatus != value))_ccStatus = value; } } 
        }

        public enum CCActivityType {Unknown = 0, Sleeping = 1, Building = 2, CheckingModifications = 3, Pending = 4};
        CCActivityType _ccActivity = CCActivityType.Unknown;
        public CCActivityType CCActivity { 
            get { lock(_locker) { return _ccActivity; } } 
            set { lock(_locker) { if(ResetLastChange(_ccActivity != value))_ccActivity = value; } } 
        }

//        public override ulong Change { get {
////            return DateTime.Now.ToHash();  
//            return Math.Max( _lastchange.ToHash(), (_perf == null)? 0 : _perf.Change ); 
//        } }

        public IDictionary<string,object> DynamicObject { get {
            //_nextBuild = DateTime.UtcNow.AddMinutes(1);
            return new Dictionary<string,object> { 
                { "abt", AvgBuildTime },
                { "nb", _server.ToTimeCode(NextBuild) },
                { "bs", (int)BuildStatus },
                { "ccs", (int)CCStatus },
                { "cca", (int)CCActivity },
                { "lb", Label }
            };
        } }

        public IDictionary<string,object> StaticObject { get {
            return _staticObject;
        } }

        public static string GetProjectId(string servername, string projectname) {
            return (servername.GetHashCode() + projectname.GetHashCode()).ToString("x");
        }

        public Project(BuildServer s, string Name) : base(GetProjectId(s.Name, Name)) {
            _server = s;
            _name = Name;
            _staticObject = new Dictionary<string,object> { 
                { "name", _name } //,
                //{ "server", _server.Name } 
            };
        }
    }
}
