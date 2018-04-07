using FontAwesome.WPF;
using Lab05_Mahur.views;
using System.ComponentModel;
using System.Windows;

namespace Lab05_Mahur
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessesListView _processesListView;
        private ProcessDetailsWindow _processDetailsWindow;

        private ImageAwesome _loader;

        public MainWindow()
        {
            InitializeComponent();
            ShowProcessListView();
        }

        private void ShowProcessListView()
        {
            if (_processesListView == null)
            {
                _processesListView = new ProcessesListView(ShowProcessDetailsWindow, ShowLoader);
            }
            ShowView(_processesListView);
        }

        private void ShowView(UIElement element)
        {
            MainGrid.Children.Clear();
            MainGrid.Children.Add(element);
        }

        private void ShowProcessDetailsWindow()
        {
            if (_processDetailsWindow == null)
            {
                _processDetailsWindow = new ProcessDetailsWindow(CloseProcessDetailsWindow, ShowLoaderDialog);
            }
            _processDetailsWindow.ShowDialog();
        }

        private void CloseProcessDetailsWindow()
        {
            if (_processDetailsWindow != null)
            {
                _processDetailsWindow.Close();
                _processDetailsWindow = null;
            }
        }

        private void ShowLoader(bool isShow)
        {
            LoaderHelper.OnRequestLoader(MainGrid, ref _loader, isShow);
        }

        private void ShowLoaderDialog(bool isShow)
        {
            if (_processDetailsWindow != null)
                LoaderHelper.OnRequestLoader(_processDetailsWindow.MainGrid, ref _loader, isShow);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_processDetailsWindow != null)
                _processDetailsWindow.Close();
            base.OnClosing(e);
        }
    }
}
