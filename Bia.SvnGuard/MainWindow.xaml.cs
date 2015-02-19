using System.Windows;
using Bia.SvnGuard.ViewModels;

namespace Bia.SvnGuard
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
