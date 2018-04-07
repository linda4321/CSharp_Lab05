using Lab05_Mahur.Annotations;
using Lab05_Mahur.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Lab05_Mahur.modelViews
{
    internal class ProcessesListViewModel : INotifyPropertyChanged
    {
        private RelayCommand _showDetailsWindowCommand;
        private RelayCommand _endProcessCommand;
        private RelayCommand _showFolderCommand;
        private RelayCommand _sortByCommand;

        private Action _showProcessDetailsAction;
        private Action<bool> _showLoaderAction;

        private ObservableCollection<MyProcess> _processes;

        private string _lastSortingProperty;
        private int _currentIndex;

        public ObservableCollection<MyProcess> Processes
        {
            get => _processes;
            set
            {
                _processes = value;
                OnPropertyChanged();
            }
        }

        public double TotalCPU
        {
            get => TotalProcessorUsage.CurrTotalCPU;
        }

        public double AvailableMem
        {
            get => TotalProcessorUsage.CurrAvailableMemory;
        }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                if (value != -1)
                {
                    _currentIndex = value;
                    OnPropertyChanged();
                }
            }
        }

        public MyProcess Current
        {
            get
            {
                return CurrentProcess.Current;
            }
            set
            {
                if (value != null)
                {
                    CurrentProcess.Current = value;
                OnPropertyChanged();
                }
            }
        }

        internal ProcessesListViewModel(Action showProcessDetails, Action<bool> showLoaderAction)
        {
            _processes = new ObservableCollection<MyProcess>(ProcessList.GetProcessesDict().Values);
            _showProcessDetailsAction = showProcessDetails;
            _showLoaderAction = showLoaderAction;
            _lastSortingProperty = string.Empty;

            var progress = new Progress<IEnumerable<MyProcess>>(processes =>
            {
                _showLoaderAction.Invoke(true);
                Thread.Sleep(500);
                Processes = new ObservableCollection<MyProcess>(processes);
                _showLoaderAction.Invoke(false);
            });
            Task.Factory.StartNew(() => UpdateProcessList(progress), TaskCreationOptions.LongRunning);

            var progressProcessMetadata = new Progress<Dictionary<int, MyProcess>>(processes =>
           {
               _showLoaderAction.Invoke(true);
                   Thread.Sleep(500);
                   UpdateObservableMetadata(processes);
                   OnPropertyChanged("TotalCPU");
                   OnPropertyChanged("AvailableMem");
               _showLoaderAction.Invoke(false);
           });
            Task.Factory.StartNew(() => UpdateProcessesMetadata(progressProcessMetadata), TaskCreationOptions.LongRunning);

        }

        private void UpdateProcessesMetadata(IProgress<Dictionary<int, MyProcess>> progress)
        {
            while (true)
            {
                progress.Report(ProcessList.GetProcessesDict());
                Thread.Sleep(2000);
            }
        }

        private void UpdateProcessList(IProgress<IEnumerable<MyProcess>> progress)
        {
            while (true)
            {
                Dictionary<int, MyProcess> updatedProcesses = ProcessList.GetProcessesDict();
                IEnumerable<MyProcess> sortedProcesses;
                if (_lastSortingProperty != string.Empty)
                    sortedProcesses = (updatedProcesses.Values.OrderBy(process => process.GetType().GetProperty(_lastSortingProperty).GetValue(process)));
                else
                    sortedProcesses = updatedProcesses.Values;

                FindCurrentProcess();
                progress.Report(sortedProcesses);
                Thread.Sleep(5000);
            }
        }

        private void FindCurrentProcess()
        {
            if (Current != null)
            {
                if (_lastSortingProperty != string.Empty)
                {
                    CurrentIndex = FindProcessIndex(Current,
                        process => process.GetPropertyValueByString(_lastSortingProperty).ToString().CompareTo(
                            Current.GetPropertyValueByString(_lastSortingProperty).ToString()) > 0);
                }
                else
                {
                    CurrentIndex = Processes.IndexOf(Current);
                }
            }
        }

        private void UpdateObservableMetadata(Dictionary<int, MyProcess> updatedProcesses)
        {
            for (int i = 0; i < Processes.Count; i++)
            {
                if (updatedProcesses.TryGetValue(Processes[i].Id, out MyProcess updatedProcess))
                {
                    Processes[i].Update(updatedProcess);
                }
            }
        }

        private int FindProcessIndex(MyProcess myProcess, Predicate<MyProcess> comparator)
        {
            int lo = 0;
            int hi = Processes.Count - 1;
            int mid = 0;
            while (lo <= hi)
            {
                mid = lo + (hi - lo) / 2;
                if (myProcess.Equals(Processes[mid]))
                {
                    return mid;
                }
                else if (comparator(Processes[mid]))
                {
                    hi = mid - 1;
                }
                else
                {
                    lo = mid + 1;
                }
            }
            return -1;
        }

        public RelayCommand ShowDetails
        {
            get { return _showDetailsWindowCommand ?? (_showDetailsWindowCommand = new RelayCommand(ShowDetailsWindow)); }
        }

        public RelayCommand EndProcess
        {
            get { return _endProcessCommand ?? (_endProcessCommand = new RelayCommand(KillProcess)); }
        }

        public RelayCommand ShowFolder
        {
            get { return _showFolderCommand ?? (_showFolderCommand = new RelayCommand(ShowProcessFolder)); }
        }

        public RelayCommand SortBy
        {
            get { return _sortByCommand ?? (_sortByCommand = new RelayCommand(Sort)); }
        }

        private void Sort(object property)
        {
            _lastSortingProperty = property.ToString();
            Processes = new ObservableCollection<MyProcess>(_processes.OrderBy(process => process.GetPropertyValueByString(_lastSortingProperty)));
        }

        private async void ShowProcessFolder(object o)
        {
            MyProcess p = (MyProcess)o;
            _showLoaderAction.Invoke(true);
            await Task.Run((() =>
            {
                if (File.Exists(p.FilePath))
                {
                    FileInfo fileInfo = new FileInfo(p.FilePath);
                    Process.Start("explorer.exe", fileInfo.DirectoryName);
                }
            }));
            _showLoaderAction.Invoke(false);

        }

        private async void KillProcess(object o)
        {
            MyProcess p = (MyProcess)o;
            _showLoaderAction.Invoke(true);

            await Task.Run((() =>
            {
                p.KillProcess();
                Thread.Sleep(500);
            }));
            _showLoaderAction.Invoke(false);
            Processes.Remove(p);
        }

        private void ShowDetailsWindow(object o)
        {
            MyProcess p = (MyProcess)o;
            _showProcessDetailsAction.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
