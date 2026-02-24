using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GKPanStats
{
    public class SystemMonitor
    {
        private TimeSpan _prevCpuTotal;
        private TimeSpan _prevCpuIdle;
        private bool _hasPrevCpu;
        private long _prevNetIn;
        private long _prevNetOut;
        private DateTime _prevNetTime = DateTime.MinValue;
        private PerformanceCounter? _cpuCounter;
        public int LastBatteryPct { get; private set; }

        public SystemMonitor()
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _cpuCounter.NextValue();
            }
            catch
            {
                _cpuCounter = null;
            }
        }

        public int ReadCPU()
        {
            try
            {
                if (_cpuCounter != null)
                    return (int)Math.Round(_cpuCounter.NextValue());
            }
            catch { }
            return 0;
        }

        public (string used, string total, int pct) ReadRAM()
        {
            try
            {
                var mi = new MEMORYSTATUSEX();
                mi.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
                if (GlobalMemoryStatusEx(ref mi))
                {
                    var total = mi.ullTotalPhys;
                    var avail = mi.ullAvailPhys;
                    var used = total - avail;
                    var pct = total > 0 ? (int)(used * 100 / total) : 0;
                    return (FormatBytes(used), FormatBytes(total), pct);
                }
            }
            catch { }
            return ("--", "--", 0);
        }

        public (int pct, string used, string total) ReadDisk()
        {
            try
            {
                long totalSize = 0, totalFree = 0;
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady && drive.DriveType == DriveType.Fixed)
                    {
                        totalSize += drive.TotalSize;
                        totalFree += drive.TotalFreeSpace;
                    }
                }
                var used = totalSize - totalFree;
                var pct = totalSize > 0 ? (int)(used * 100 / totalSize) : 0;
                return (pct, FormatBytes((ulong)used), FormatBytes((ulong)totalSize));
            }
            catch { }
            return (0, "--", "--");
        }

        public (string down, string up) ReadNetwork()
        {
            try
            {
                long totalIn = 0, totalOut = 0;
                foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up &&
                        ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        var stats = ni.GetIPStatistics();
                        totalIn += stats.BytesReceived;
                        totalOut += stats.BytesSent;
                    }
                }

                var now = DateTime.UtcNow;
                var elapsed = (now - _prevNetTime).TotalSeconds;
                long speedIn = 0, speedOut = 0;

                if (elapsed > 0 && _prevNetTime != DateTime.MinValue)
                {
                    speedIn = (long)((totalIn - _prevNetIn) / elapsed);
                    speedOut = (long)((totalOut - _prevNetOut) / elapsed);
                    if (speedIn < 0) speedIn = 0;
                    if (speedOut < 0) speedOut = 0;
                }

                _prevNetIn = totalIn;
                _prevNetOut = totalOut;
                _prevNetTime = now;

                return (FormatSpeed(speedIn), FormatSpeed(speedOut));
            }
            catch { }
            return ("--", "--");
        }

        public string ReadBattery(string label, string noBattery)
        {
            try
            {
                var ps = SystemInformation.PowerStatus;
                if (ps.BatteryChargeStatus == BatteryChargeStatus.NoSystemBattery)
                    return $"{label}: {noBattery}";

                var pct = (int)(ps.BatteryLifePercent * 100);
                LastBatteryPct = pct;
                var charging = ps.PowerLineStatus == PowerLineStatus.Online;
                return $"{label}: {pct}%{(charging ? " \u26A1" : "")}";
            }
            catch { }
            return $"{label}: --";
        }

        private string FormatBytes(ulong bytes)
        {
            if (bytes >= 1_073_741_824) return $"{bytes / 1_073_741_824.0:F1} GB";
            if (bytes >= 1_048_576) return $"{bytes / 1_048_576.0:F0} MB";
            return $"{bytes / 1024} KB";
        }

        private string FormatSpeed(long bytesPerSec)
        {
            if (bytesPerSec >= 1_000_000) return $"{bytesPerSec / 1_000_000.0:F1} MB/s";
            if (bytesPerSec >= 1_000) return $"{bytesPerSec / 1_000.0:F0} KB/s";
            return $"{bytesPerSec} B/s";
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
    }
}
