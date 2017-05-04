using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using ThoughtWorks.CruiseControl.Remote;

namespace Cruise.DashboardBroker
{
    using Remote;

    public class BuildServer:ServerBase, IScheduleProcess {

        //List<Project> _projects = new List<Project>();
        IDictionary<string,Project> _projects = new ConcurrentDictionary<string, Project>();

        public override IDictionary<string,object> StaticObject { get {
            var so = base.StaticObject;
            so["projects"] = _projects.Values.ToIdDictionary(i => ((Project)i).StaticObject);
            return so;
        } }

        public bool HasProject(string projid) {
            return _projects.ContainsKey(projid);
        }

        public void ProjectControlCommand(string projid, string cmd) {
            var proj = _projects[projid];
            switch(cmd) {
                case "force": _client.ForceBuild(proj.Name); break;
                case "stop": _client.Stop(proj.Name); break;
                case "start": _client.Start(proj.Name); break;
                case "abort": _client.AbortBuild(proj.Name); break;
            }
        }

        IBuildServerClient _client;

        public BuildServer(string name, IBuildServerClient client) : base(name, null) {
            _client = client;
            _perf = new BuildServerPerformance();
            BackScheduler.Instance.Add(this, 1000);
        }

        public override IDictionary<string,object> GetChanges(ulong hash) {
            var chresult = base.GetChanges(hash);
            var chproj = _projects.Values.Where(p => p.Change >= hash);
            if(!chproj.Any())return chresult;
            chresult["projects"] = chproj.ToIdDictionary(i => ((Project)i).DynamicObject);
            return chresult;
        }

        public void Add(Project p) {
            _projects.Add(p.Id, p);
            ResetLastChange(true);
        }

        static Project.BuildStatusType GetBuildStatus(ProjectStatus proj)
        {
            switch (proj.BuildStatus)
            {
                case IntegrationStatus.Cancelled:
                case IntegrationStatus.Exception:
                case IntegrationStatus.Failure:
                    return Project.BuildStatusType.Failure;

                case ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Success:
                    return Project.BuildStatusType.Success;

                case ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Unknown:
                    return Project.BuildStatusType.Unknown;
            }
            return Project.BuildStatusType.Unknown;
        }

        static Project.CCStatusType GetCruiseStatus(ProjectStatus proj)
        {
            switch (proj.Status)
            {
                case ProjectIntegratorState.Running:
                    return Project.CCStatusType.Running;

                case ProjectIntegratorState.Stopped:
                    return Project.CCStatusType.Stopped;

                case ProjectIntegratorState.Stopping:
                    return Project.CCStatusType.Stopping;

                default:
                    return Project.CCStatusType.Unknown;
            }
        }

        static Project.CCActivityType GetCruiseActivity(ProjectStatus proj)
        {
            if (proj.Activity.IsBuilding()) return Project.CCActivityType.Building;
            if (proj.Activity.IsPending()) return Project.CCActivityType.Pending;
            if (proj.Activity.IsSleeping()) return Project.CCActivityType.Sleeping;
            if (proj.Activity.IsCheckingModifications()) return Project.CCActivityType.CheckingModifications;
            return Project.CCActivityType.Unknown;
        }

        public void ScheduleRun() {
            //---------- offline mode
            //if(_projects.Count < 1) {
            //    Add(new Project(this, "Project1"));
            //}
            //return;

            ((BuildServerPerformance)_perf).Update(_client.GetServerPerformanceStatus());

            foreach (var proj in _client.GetProjectsStatus()) {
                var id = Project.GetProjectId(Name,proj.Name);
                if( //(_projects.Count < 1) && 
                    !_projects.Keys.Any(pk => pk == id)) {
                    Add(new Project(this, proj.Name));
                }
                if(!_projects.ContainsKey(id))continue;
                _projects[id].NextBuild = proj.NextBuildTime;
                _projects[id].Label = proj.LastBuildLabel;
                var ccactivity = GetCruiseActivity(proj);
                if (
                    !_projects[id].AvgBuildTimeIsSet ||
                    ((_projects[id].CCActivity == Project.CCActivityType.Building) && (_projects[id].CCActivity != ccactivity))
                    )
                {
                    _projects[id].AvgBuildTime = _client.GetAverageBuildTime(proj.Name);
                    //"log20170424193441Lbuild.282.xml,log20170424193210Lbuild.281.xml,log20170424152429Lbuild.280.xml,log20170424151131Lbuild.279.xml,log20170424150444Lbuild.278.xml"
                }
                _projects[id].CCActivity = GetCruiseActivity(proj);
                _projects[id].BuildStatus = GetBuildStatus(proj);
                _projects[id].CCStatus = GetCruiseStatus(proj);
            }

        }
    }
}
