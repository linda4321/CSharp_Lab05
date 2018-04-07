using System.Diagnostics;

namespace Lab05_Mahur
{
    internal static class TotalProcessorUsage
    {
        private static PerformanceCounter _totalCPU = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private static PerformanceCounter _availableMemory = new PerformanceCounter("Memory", "Available MBytes");
 
        internal static double CurrTotalCPU
        {
            get { return _totalCPU.NextValue(); }
        }

        internal static double CurrAvailableMemory
        {
            get { return _availableMemory.NextValue(); }
        }
    }
}
