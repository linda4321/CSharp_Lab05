using Lab05_Mahur.Annotations;
using Lab05_Mahur.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Lab05_Mahur.ViewModel
{
    internal class ProcessDetailsViewModel : INotifyPropertyChanged
    {
        private RelayCommand _closeWindowCommand;
        private Action _closeAction;
        private Action<bool> _showLoaderAction;

        private MyProcess _process;
        private ObservableCollection<ProcessModule> _modules;
        private ObservableCollection<ProcessThread> _threads;

        public ObservableCollection<ProcessModule> Modules
        {
            get => _modules;
            set
            {
                _modules = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProcessThread> Threads
        {
            get => _threads;
            set
            {
                _threads = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get { return _process.Name; }
        }

        public int Id
        {
            get { return _process.Id; }
        }

        public bool IsActive
        {
            get { return _process.IsActive; }
        }

        public double CPUUsage
        {
            get { return _process.CPUUsage; }
        }

        public double MemUsage
        {
            get { return _process.MemoryUsage; }
        }

        public double PrivateMem
        {
            get { return _process.PrivateMemory; }
        }

        public double VirtualMem
        {
            get { return _process.VirtualMemory; }
        }

        public int NThreads
        {
            get { return _process.NThreads; }
        }

        public string UserName
        {
            get { return _process.UserName; }
        }

        public string FilePath
        {
            get { return _process.FilePath; }
        }

        public string StartTime
        {
            get { return _process.StartTime.ToShortDateString(); }
        }

        internal ProcessDetailsViewModel(MyProcess process, Action closeAction, Action<bool> showLoaderAction)
        {
            _process = process;
            _modules = new ObservableCollection<ProcessModule>();
            _threads = new ObservableCollection<ProcessThread>();
            _closeAction = closeAction;
            _showLoaderAction = showLoaderAction;
            FillModulesCollection();
            FillThreadsCollection();

            var progress = new Progress<Process>(processUpdated =>
            {
                _showLoaderAction.Invoke(true);

                Thread.Sleep(500);
                if (processUpdated != null)
                {
                    _process = new MyProcess(processUpdated);
                    OnPropertyChanged("IsActive");
                    OnPropertyChanged("CPUUsage");
                    OnPropertyChanged("MemUsage");
                    OnPropertyChanged("PrivateMem");
                    OnPropertyChanged("VirtualMem");
                    OnPropertyChanged("NThreads");
                    FillModulesCollection();
                    FillThreadsCollection();
                }
                _showLoaderAction.Invoke(false);
            });
            Task.Factory.StartNew(() => UpdateUserListWorker(progress), TaskCreationOptions.LongRunning);
        }

        private void UpdateUserListWorker(IProgress<Process> progress)
        {
            while (true)
            {
                var process = Process.GetProcessById(_process.Id);
                progress.Report(process);
                Thread.Sleep(2000);
            }
        }


        private void FillModulesCollection()
        {
            Modules.Clear();
            foreach (ProcessModule module in _process.Modules)
                Modules.Add(module);
        }

        private void FillThreadsCollection()
        {
            Threads.Clear();
            foreach (ProcessThread thread in _process.Threads)
                _threads.Add(thread);
        }

        public RelayCommand Close
        {
            get { return _closeWindowCommand ?? (_closeWindowCommand = new RelayCommand(o => _closeAction.Invoke())); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
