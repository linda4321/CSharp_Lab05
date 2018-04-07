using Lab05_Mahur.modelViews;
using System;
using System.Windows.Controls;

namespace Lab05_Mahur.views
{
    /// <summary>
    /// Interaction logic for ProcessesList.xaml
    /// </summary>
    public partial class ProcessesListView : UserControl
    {
        public ProcessesListView(Action showProcessDetailsWindow, Action<bool> showLoader)
        {
            InitializeComponent();
            DataContext = new ProcessesListViewModel(showProcessDetailsWindow, showLoader);
        }
    }
}
