using System;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace Cruise.DashboardBroker
{
    public class BuildServerPerformance : ServerPerformance
    {
        public void Update(HostPerformanceResponse response) {
            if (response == null) return;
            lock (_locker)
            {
                _memory = response.Memory;
                _disk = response.Disk;
                _cpu = response.Cpu;
                _lastchange = DateTime.Now;
            }
        }
    }
}
