using GraphPlotter.ViewModels;
using System.Windows;

namespace GraphPlotter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
            Closing += MainWindowClosing;
        }

        private void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = DataContext as MainViewModel;
            vm.SaveState();
        }

        //TODO сохранять командой, sinc доделать
    }
}
