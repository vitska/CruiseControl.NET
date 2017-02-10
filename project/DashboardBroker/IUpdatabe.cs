using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cruise.DashboardBroker {
    public interface IUpdatabe {
        DateTime ChangeDate { get; }
        DateTime RemoteDate { get; }
    }
}
