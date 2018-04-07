using Lab05_Mahur.ViewModel;
using System;
using System.Windows;

namespace Lab05_Mahur.views
{
    /// <summary>
    /// Interaction logic for ProcessDetails.xaml
    /// </summary>
    public partial class ProcessDetailsWindow : Window
    {
        public ProcessDetailsWindow(Action close, Action<bool> showLoader)
        {
            InitializeComponent();
            DataContext = new ProcessDetailsViewModel(CurrentProcess.Current, close, showLoader);

            WindowStyle = WindowStyle.None;
        }
    }
}
