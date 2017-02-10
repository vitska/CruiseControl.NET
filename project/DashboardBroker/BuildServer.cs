using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;

namespace Cruise.DashboardBroker {
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

        //public IEnumerator GetEnumerator() {
        //    throw new NotImplementedException();
        //}

        public void ScheduleRun() {
            //---------- offline mode
            //if(_projects.Count < 1) {
            //    Add(new Project(this, "Project1"));
            //}
            //return;

            foreach(var proj in _client.GetProjectsStatus()) {
                var id = Project.GetProjectId(Name,proj.Name);
                if( //(_projects.Count < 1) && 
                    !_projects.Keys.Any(pk => pk == id)) {
                    Add(new Project(this, proj.Name));
                }
                if(!_projects.ContainsKey(id))continue;
                _projects[id].NextBuild = proj.NextBuildTime;
                switch(proj.BuildStatus) {
                    case ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Cancelled:
                    case ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Exception:
                    case ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Failure:
                        _projects[id].BuildStatus = Project.BuildStatusType.Failure;
                        break;
                    case ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Success:
                        _projects[id].BuildStatus = Project.BuildStatusType.Success;
                        break;
                    case ThoughtWorks.CruiseControl.Remote.IntegrationStatus.Unknown:
                        _projects[id].BuildStatus = Project.BuildStatusType.Unknown;
                        break;
                }
                if(proj.Activity.IsBuilding())_projects[id].CCActivity = Project.CCActivityType.Building;
                if(proj.Activity.IsPending())_projects[id].CCActivity = Project.CCActivityType.Pending;
                if(proj.Activity.IsSleeping())_projects[id].CCActivity = Project.CCActivityType.Sleeping;
                if(proj.Activity.IsCheckingModifications())_projects[id].CCActivity = Project.CCActivityType.CheckingModifications;

                switch(proj.Status) {
                    case ThoughtWorks.CruiseControl.Remote.ProjectIntegratorState.Running:
                        _projects[id].CCStatus = Project.CCStatusType.Running;
                        break;
                    case ThoughtWorks.CruiseControl.Remote.ProjectIntegratorState.Stopped:
                        _projects[id].CCStatus = Project.CCStatusType.Stopped;
                        break;
                    case ThoughtWorks.CruiseControl.Remote.ProjectIntegratorState.Stopping:
                        _projects[id].CCStatus = Project.CCStatusType.Stopping;
                        break;
                    default:
                        _projects[id].CCStatus = Project.CCStatusType.Unknown;
                        break;
                }

                //proj.ShowForceBuildButton
                //proj.ShowStartStopButton
            }

        }
    }
}
