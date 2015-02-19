using System.Windows;
using Bia.SvnGuard.Services;
using Ninject;

namespace Bia.SvnGuard
{
    public partial class App : Application
    {
        private IKernel _container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureContainer();
            ComposeMainWindow();
            Current.MainWindow.Show();
        }

        private void ComposeMainWindow()
        {
            Current.MainWindow = _container.Get<MainWindow>();
        }

        private void ConfigureContainer()
        {
            _container = new StandardKernel();
            _container.Bind<Configuration.Configuration>().ToSelf().InSingletonScope();
            _container.Bind<FileSystemService>().ToSelf().InSingletonScope();
        }
    }
}
