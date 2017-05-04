using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace Cruise.DashboardBroker.Remote
{
    public interface IBuildServerClient {
        IEnumerable<ProjectStatus> GetProjectsStatus();
        //IEnumerable<string> GetMostRecentBuildNames(string projectName, int buildCount);
        int GetAverageBuildTime(string projectName);
        HostPerformanceResponse GetServerPerformanceStatus();
        void ForceBuild(string name);
        void AbortBuild(string name);
        void Start(string name);
        void Stop(string name);
    }
}
