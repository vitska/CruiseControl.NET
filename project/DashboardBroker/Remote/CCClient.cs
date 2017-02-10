using System;
using System.Linq;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote;
//using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
//using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
//using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace Cruise.DashboardBroker.Remote {
    public class CCClient : IBuildServerClient {
        //ServerLocation _location;

        //public class Config:IRemoteServicesConfiguration {
        //    public ServerLocation[] Servers { get; set; }
        //}

        static readonly ICruiseServerClientFactory _ccf = new CruiseServerClientFactory();

        CruiseServerClientBase _client;

        private CruiseServerClientBase GetCruiseManager(string url, string sessionToken)
		{
            //var config = GetServerUrl(serverSpecifier);
            CruiseServerClientBase manager = _ccf.GenerateClient(url,
                new ClientStartUpSettings
                {
                    BackwardsCompatable = false//config.BackwardCompatible
                });
            manager.SessionToken = sessionToken;
            return manager;
		}

        public CCClient(string url) {
            //_location = location;
            _client = GetCruiseManager(url, null);
            //var ServerConfig = mw.GetServerConfiguration(dcfg.RemoteServices.Servers[0].ServerName);
        }

        public IEnumerable<ProjectStatus> GetProjectsStatus() {
            return _client.GetProjectStatus();
            //var ps = _farmsvc.GetProjectStatusListAndCaptureExceptions(_location, null);
            //return ps.StatusAndServerList.Select(sos => sos.ProjectStatus);
        }

        //IProjectSpecifier GetProjectSpecifier(string name) {
        //    return new DefaultProjectSpecifier(_location, name);
        //}

        public void ForceBuild(string name) {
//            var projspec = new DefaultProjectSpecifier(_location, name);
            _client.ForceBuild(name, null);
        }

        public void AbortBuild(string name) {
            _client.AbortBuild(name);
        }

        public void Start(string name) {
            _client.StartProject(name);
        }

        public void Stop(string name) {
            _client.StopProject(name);
        }


    }
}
