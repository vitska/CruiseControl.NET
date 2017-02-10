using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.TinyIoc;

namespace Cruise.DashboardBroker.Service {
    public class ServiceBootstrapper : DefaultNancyBootstrapper {
        protected override void ApplicationStartup(TinyIoCContainer container,IPipelines pipelines) {
            base.ApplicationStartup(container,pipelines);
            pipelines.RegisterCompressionCheck();
            this.Conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("st", "Static"));
        }

        public override void Configure(Nancy.Configuration.INancyEnvironment environment)
        {
            environment.Diagnostics(
                enabled: true,
                path:"diag",
                password: "password");
            environment.Tracing(
                enabled: true,
                displayErrorTraces: true);
        }
    }
}
