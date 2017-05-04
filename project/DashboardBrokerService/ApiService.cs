using Nancy.Hosting.Self;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Linq;


namespace Cruise.DashboardBroker.Service {
    public class ApiService : ServiceBase {

        public ApiService()
        {
            this.ServiceName = "CruiseDashboardBroker";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        NancyHost _host;

        protected override void OnStart(string [] args)
        {
            _host = new NancyHost(new Uri(System.Configuration.ConfigurationManager.AppSettings["url"]));
            _host.Start();
        }
 
        protected override void OnStop()
        {
            _host.Dispose();
        }

        public void DebugStart()
        {
            OnStart(new string[0]);
            Thread.Sleep(Timeout.Infinite);
            OnStop();
        }

        static void Main(string[] args) {
            if(args.Any(a => a.ToLower() == "-service")) {
                Run(new ApiService());
                return;
            }
            new ApiService().DebugStart();
        }
    }
}
