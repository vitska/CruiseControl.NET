using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThoughtWorks.CruiseControl.Remote;

namespace Cruise.DashboardBroker.Remote {
    public interface IBuildServerClient {
        IEnumerable<ProjectStatus> GetProjectsStatus();
        void ForceBuild(string name);
        void AbortBuild(string name);
        void Start(string name);
        void Stop(string name);
    }
}
