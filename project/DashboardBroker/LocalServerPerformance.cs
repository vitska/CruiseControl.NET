using System;
using ThoughtWorks.CruiseControl.Core.Util;

namespace Cruise.DashboardBroker
{
    public class LocalServerPerformance:ServerPerformance, IScheduleProcess {
        internal LocalServerPerformance() {
            BackScheduler.Instance.Add(this, 2000);
        }

        public void ScheduleRun() {
            var hp = new HostPerformance();
            lock(_locker) {
                _memory = hp.Memory;
                _disk = hp.Disk;
                _cpu = hp.Cpu;
                _lastchange = DateTime.Now;
            }
        }
    }
}
