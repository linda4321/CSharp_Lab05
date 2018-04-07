using Lab05_Mahur.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lab05_Mahur
{
    internal static class ProcessList
    {
        private static Object objLock = new Object();
        internal static Dictionary<int, MyProcess> GetProcessesDict()
        {
            lock (objLock)
            {
                Dictionary<int, MyProcess> processesDict = new Dictionary<int, MyProcess>();
                Process[] processes = Process.GetProcesses();
                foreach (Process p in processes)
                {
                    MyProcess newProcess = new MyProcess(p);
                    processesDict.Add(newProcess.Id, newProcess);
                }
                return processesDict;
            }
        }
    }
}
