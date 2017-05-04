using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Cruise.DashboardBroker.Remote;

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
            var dbc = XmlUtils.FromFile<Dashboard>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dashboard.config"));
            foreach(var sl in dbc.RemoteServices.Servers) {
                CCClient c = new CCClient(sl.Url);
                var s = new BuildServer(sl.Name, c);
                _buildservers.Add(s.Id, s);
            }
        }
    }
}
