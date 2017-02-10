using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cruise.DashboardBroker {
    public interface IChangeable {
        UInt64 Change { get; }
    }
}
