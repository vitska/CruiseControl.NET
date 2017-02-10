using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cruise.DashboardBroker {
    public interface IServer : IChangeable, IIdentifable {
        string Name { get; }
        object Perf { get; }
        IDictionary<string,object> StaticObject { get; }
        IDictionary<string,object> GetChanges(ulong hash);
    }
}
