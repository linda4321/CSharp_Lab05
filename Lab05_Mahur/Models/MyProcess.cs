using Lab05_Mahur.Annotations;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.Runtime.CompilerServices;
using System.Text;

namespace Lab05_Mahur.Models
{
    internal class MyProcess : INotifyPropertyChanged
    {
        private Process _process;

        private string _name;
        private int _id;
        private bool _isActive;
        private double _cpuUsage;
        private double _memoryUsage;
        private double _privateMemory;
        private double _virtualMemory;
        private int _nTheards;
        private string _userName;
        private string _filePath;
        private DateTime _startTime;

        public string Name { get { return _name; } }
        public int Id { get { return _id; } }
        public bool IsActive { get { return _isActive; } }
        public double CPUUsage { get { return _cpuUsage; } }
        public double MemoryUsage { get { return _memoryUsage; } }
        public double PrivateMemory { get { return _privateMemory; } }
        public double VirtualMemory { get { return _virtualMemory; } }
        public int NThreads { get { return _nTheards; } }
        public string UserName { get { return _userName; } }
        public string FilePath { get { return _filePath; } }
        public DateTime StartTime { get { return _startTime; } }

        public ProcessModuleCollection Modules { get { return _process.Modules ; } }
        public ProcessThreadCollection Threads { get { return _process.Threads; } }

        internal MyProcess(Process process)
        {
            _process = process;
            _name = _process.ProcessName;
            _id = _process.Id;
            _isActive = _process.Responding;
            using (PerformanceCounter theCPUCounter = new PerformanceCounter("Process", "% Processor Time", _name))
            {
                //theCPUCounter.NextValue();
                //  System.Threading.Thread.Sleep(250);
                _cpuUsage = theCPUCounter.NextValue();
            };

            _memoryUsage = _process.WorkingSet64 / 1024;
            _privateMemory = _process.PrivateMemorySize64 / 1024;
            _virtualMemory = _process.VirtualMemorySize64;

            _nTheards = _process.Threads.Count;
            _userName = GetProcessOwner();
            try
            {
                _filePath = _process.MainModule.FileName;
            }
            catch (Exception)
            {
                _filePath = "Access denied";
            }
            try
            {
                _startTime = _process.StartTime;
            }
            catch (Exception)
            {
                _startTime = DateTime.Today;
            }
        }

        internal void Update(MyProcess newProcess)
        {
            _process = newProcess._process;
            _isActive = newProcess.IsActive;
            OnPropertyChanged("IsActive");
            _cpuUsage = newProcess.CPUUsage;
            OnPropertyChanged("CPUUsage");
            _memoryUsage = newProcess.MemoryUsage;
            OnPropertyChanged("MemoryUsage");
            _privateMemory = newProcess.PrivateMemory;
            OnPropertyChanged("PrivateMemory");
            _virtualMemory = newProcess.VirtualMemory;
            OnPropertyChanged("VirtualMemory");
            _nTheards = newProcess.NThreads;
            OnPropertyChanged("NThreads");
        }

        internal Object GetPropertyValueByString(string propertyName)
        {
            return GetType().GetProperty(propertyName).GetValue(this);
        }

        internal void KillProcess()
        {
            _process.Kill();
        }

        private string GetProcessOwner()
        {
            string query = "Select * From Win32_Process Where ProcessID = " + Id;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
            ManagementObjectCollection processList = searcher.Get();

            foreach (ManagementObject obj in processList)
            {
                string[] argList = new string[] { string.Empty, string.Empty };
                int returnVal = Convert.ToInt32(obj.InvokeMethod("GetOwner", argList));
                if (returnVal == 0)
                {
                    return argList[1] + "\\" + argList[0];
                }
            }
            return "NO OWNER";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Name: " + Name);
            sb.AppendLine();
            sb.Append("Id: " + Id);
            sb.AppendLine();
            sb.Append("Is active: " + IsActive);
            sb.AppendLine();
            sb.Append("CPU usage: " + CPUUsage);
            sb.AppendLine();
            sb.Append("Memory usage: " + MemoryUsage);
            sb.AppendLine();
            sb.Append("Private memory usage: " + PrivateMemory);
            sb.AppendLine();
            sb.Append("Virtual memory usage: " + VirtualMemory);
            sb.AppendLine();
            sb.Append("Number of threads: " + NThreads);
            sb.AppendLine();
            sb.Append("User name: " + UserName);
            sb.AppendLine();
            sb.Append("Filepath: " + FilePath);
            sb.AppendLine();
            sb.Append("Start time: " + StartTime);
            sb.AppendLine();
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            MyProcess p = (MyProcess)obj;
            if (p.Id != Id)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            return 1969571243 + _id.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
