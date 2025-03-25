using GraphPlotter.ViewModels;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace GraphPlotter
{
    public partial class MainWindow : Window
    {
        private Regex numericRregex = new Regex("^-?[1-9]+\\.?[0-9]*$");

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

        private void TextBlockPreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var newText = textBox.Text.Insert(textBox.CaretIndex, e.Text);

                if (!numericRregex.IsMatch(newText))
                {
                    e.Handled = true;
                }
            }
        }
    }
}