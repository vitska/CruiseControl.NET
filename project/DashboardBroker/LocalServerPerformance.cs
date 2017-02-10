using System;
using System.IO;
using System.Diagnostics;

namespace Cruise.DashboardBroker {
    public class LocalServerPerformance:ServerPerformance, IScheduleProcess {
        internal LocalServerPerformance() {
            BackScheduler.Instance.Add(this, 2000);
        }

        private static byte GetDiskUsagePercent(string rootDirectory) {
            foreach(DriveInfo drive in DriveInfo.GetDrives()) {
                if(drive.IsReady && rootDirectory.StartsWith(drive.RootDirectory.FullName)) {
                    var dtotal = drive.TotalSize / (1024L * 1024L);
                    var dfree = drive.TotalFreeSpace / (1024L * 1024L);
                    return Convert.ToByte(((dtotal - dfree) * 100) / dtotal);
                }
            }
            return 0;
        }

        static readonly PerformanceCounter cpuCounter = new PerformanceCounter("Processor","% Processor Time","_Total");

        public byte getCurrentCpuUsage(){
            return Convert.ToByte(cpuCounter.NextValue());
        }

        public void ScheduleRun() {
            var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
            lock(_locker) {
                var _memoryMb = Convert.ToUInt32(ci.AvailablePhysicalMemory  / (1024L * 1024L));
                uint totalmb = Convert.ToUInt32( ci.TotalPhysicalMemory / (1024L * 1024L) );
                _memory = Convert.ToByte(((totalmb - _memoryMb) * 100) / totalmb);
            }
            var fs = GetDiskUsagePercent(AppDomain.CurrentDomain.BaseDirectory);
            lock(_locker) {
                _disk = fs;
            }
            var cpu = getCurrentCpuUsage();
            lock(_locker) {
                _cpu = cpu;
            }
            lock(_locker) {
                _lastchange = DateTime.Now;
            }
        }
    }
}
