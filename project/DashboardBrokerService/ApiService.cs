using Nancy.Hosting.Self;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Linq;
using System.Diagnostics;

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
            var url = System.Configuration.ConfigurationManager.AppSettings["url"];
            _host = new NancyHost(
                 new HostConfiguration { UrlReservations = new UrlReservations { CreateAutomatically = true } },
                new Uri(url)
             );
            _host.Start();
            ProcessStartInfo sInfo = new ProcessStartInfo(url);  
            Process.Start(sInfo);
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
