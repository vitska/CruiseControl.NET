using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Cruise.DashboardBroker.Remote;
//using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using Objection.NetReflectorPlugin;
using Objection;
//using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.Remote;

namespace Cruise.DashboardBroker
{
    using Config;

    public class ServerFederation
    {
        IServer _thisserver = new BrokerServer();
        IDictionary<string, BuildServer> _buildservers = new ConcurrentDictionary<string, BuildServer>();
        public IEnumerable<IServer> Servers { get {
            yield return _thisserver;
            foreach(var s in _buildservers)
                yield return s.Value;
        } }

        public IDictionary<string,object> StaticObject { get {
            return new Dictionary<string,object> { {
                "servers", Servers.Select(s => s.StaticObject)
            }};
        } }

        public IDictionary<string,object> GetStatusChanges(ulong hash) {
            return new Dictionary<string,object> {
                { "ch", Servers.Max(s => s.Change) },
                { "s", Servers.Where(s => s.Change >= hash).ToIdDictionary(s => ((IServer)s).GetChanges(hash)) }
            };
        }


        public IDictionary<string,object> Command(string cmd, string projid) {
            var server = _buildservers.Values.SingleOrDefault(s => s.HasProject(projid));
            server.ProjectControlCommand(projid, cmd);
            return new Dictionary<string,object> {};
        }

        public static readonly ServerFederation Instance = new ServerFederation();

        string[] Projects { get; }

        internal ServerFederation() {
            //------ offline mode --------
            //var os = new BuildServer("Offline server", null);
            //_buildservers.Add(os.Id, os);
            //return;

            //var dcfg = new DashboardConfigurationLoader(new ObjectionNetReflectorInstantiator(new ObjectionStore()));
            //var mw = new ServerAggregatingCruiseManagerWrapper(dcfg.RemoteServices, new CruiseServerClientFactory());
            var dbc = XmlUtils.FromFile<Dashboard>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dashboard.config"));
            //dbc.remoteServices.servers = new Server[] { new Server() };
            //var dbcs = XmlUtils.ToString(dbc);
            foreach(var sl in dbc.RemoteServices.Servers) {
                CCClient c = new CCClient(sl.Url);
                var s = new BuildServer(sl.Name, c);
                _buildservers.Add(s.Id, s);
            }
            //s.Add(new Project(s, "NITA.119.oscs.app [dev]") { CCStatus = Project.CCStatusType.Running, BuildStatus = Project.BuildStatusType.Failure });
            //s.Add(new Project(s, "NITA.119.oscs.app [online]") { CCActivity = Project.CCActivityType.Building, BuildStatus = Project.BuildStatusType.Success  });
            //s.Add(new Project(s, "NITA.119.oscs.app [test]") { CCStatus = Project.CCStatusType.Stopping } );
            //s.Add(new Project(s, "NITA.119.oscs.Binc [dev]") { CCActivity = Project.CCActivityType.Sleeping });
            //s.Add(new Project(s, "NITA.119.oscs.Binc [online]") { CCStatus = Project.CCStatusType.Stopped });
            //s.Add(new Project(s, "NITA.119.oscs.Binc [test]") { CCActivity = Project.CCActivityType.Unknown, CCStatus = Project.CCStatusType.Unknown });
            //_buildservers.Add(s.Id, s);
        }
    }
}
