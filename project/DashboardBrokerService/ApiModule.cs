using System;
using Nancy;
using Nancy.Json;

namespace Cruise.DashboardBroker.Service {
    public class ApiModule : NancyModule {
        public ApiModule()
        {
            Get("/", _ => View["index"]);
            Get("/status/{update}", p =>
            {
                return Response.AsJson( ServerFederation.Instance.GetStatusChanges((ulong)p.update) );
            });
            Get("/config", p =>
            {
                return Response.AsJson( ServerFederation.Instance.StaticObject );
            });
            Post("/ctrl/{cmd}/{projid}", p =>
            {
                return Response.AsJson( ServerFederation.Instance.Command((string)p.cmd,(string)p.projid) );
            });
        }
    }
}
