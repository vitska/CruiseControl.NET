using System;
using System.IO;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    public class HostPerformance : IHostPerformance
    {
        public byte Cpu { get; private set; }
        public byte Memory { get; private set; }
        public byte Disk { get; private set; }

        public HostPerformance()
        {
            Memory = GetMemoryUsage();
            Disk = GetDiskUsagePercent(AppDomain.CurrentDomain.BaseDirectory);
            Cpu = GetCurrentCpuUsage();
        }

        private byte GetMemoryUsage()
        {
            var ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
            var _memoryMb = Convert.ToUInt32(ci.AvailablePhysicalMemory / (1024L * 1024L));
            uint totalmb = Convert.ToUInt32(ci.TotalPhysicalMemory / (1024L * 1024L));
            return Convert.ToByte(((totalmb - _memoryMb) * 100) / totalmb);
        }

        static readonly PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        private byte GetCurrentCpuUsage()
        {
            return Convert.ToByte(cpuCounter.NextValue());
        }

        private static byte GetDiskUsagePercent(string rootDirectory)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && rootDirectory.StartsWith(drive.RootDirectory.FullName))
                {
                    var dtotal = drive.TotalSize / (1024L * 1024L);
                    var dfree = drive.TotalFreeSpace / (1024L * 1024L);
                    return Convert.ToByte(((dtotal - dfree) * 100) / dtotal);
                }
            }
            return 0;
        }
    }
}
