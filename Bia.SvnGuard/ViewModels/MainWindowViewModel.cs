using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Bia.SvnGuard.Configuration;
using Bia.SvnGuard.Properties;
using Bia.SvnGuard.Services;

namespace Bia.SvnGuard.ViewModels
{
    [UsedImplicitly]
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly Configuration.Configuration _configuration;
        private readonly FileSystemService _fileSystem;
        private string _svnUtilitiesPath;
        private string _stylecopPath;
        private string _repositoriesPath;
        private string _stylecopSettings;
        private DelegateCommand _selectSvnUtilitiesPathCommand;
        private DelegateCommand _selectStylecopPathCommand;
        private DelegateCommand _selectRepositoriesPathCommand;
        private DelegateCommand _applyCommand;
        private DelegateCommand _selectStylecopSettingsCommand;
        private readonly IEnumerable<RepositoryConfigurationElement> _repositories;

        public MainWindowViewModel(Configuration.Configuration configuration, FileSystemService fileSystem)
        {
            _configuration = configuration;
            _fileSystem = fileSystem;
            _svnUtilitiesPath = _configuration.SvnLookPath;
            _stylecopPath = _configuration.StylecopPath;
            _repositoriesPath = _configuration.RepositoriesPath;
            _repositories = _configuration.RepositoriesConfig.Repositories.Cast<RepositoryConfigurationElement>();
            _stylecopSettings = _configuration.StylecopSettings;
        }

        public string SvnUtilitiesPath
        {
            get { return _svnUtilitiesPath; }
            set
            {
                _svnUtilitiesPath = value;
                _configuration.SvnLookPath = _svnUtilitiesPath;
                OnPropertyChanged();
            }
        }

        public string StylecopPath
        {
            get { return _stylecopPath; }
            set
            {
                _stylecopPath = value;
                _configuration.StylecopPath = _stylecopPath;
                OnPropertyChanged();
            }
        }

        public string RepositoriesPath
        {
            get { return _repositoriesPath; }
            set
            {
                _repositoriesPath = value;
                _configuration.RepositoriesPath = _repositoriesPath;
                OnPropertyChanged();
            }
        }

        public string StylecopSettings
        {
            get { return _stylecopSettings; }
            set
            {
                _stylecopSettings = value;
                _configuration.StylecopSettings = _stylecopSettings;
                OnPropertyChanged();
            }
        }

        public ICommand SelectSvnUtilitiesPathCommand
        {
            get
            {
                if (_selectSvnUtilitiesPathCommand == null)
                {
                    _selectSvnUtilitiesPathCommand = new DelegateCommand(SelectSvnUtilitiesPath);
                }

                return _selectSvnUtilitiesPathCommand;
            }
        }

        public ICommand SelectStylecopPathCommand
        {
            get
            {
                if (_selectStylecopPathCommand == null)
                {
                    _selectStylecopPathCommand = new DelegateCommand(SelectStylecopPath);
                }

                return _selectStylecopPathCommand;
            }
        }

        public ICommand SelectRepositoriesPathCommand
        {
            get
            {
                if (_selectRepositoriesPathCommand == null)
                {
                    _selectRepositoriesPathCommand = new DelegateCommand(SelectRepositoriesPath);
                }

                return _selectRepositoriesPathCommand;
            }
        }

        public ICommand SelectStylecopSettingsCommand
        {
            get
            {
                if (_selectStylecopSettingsCommand == null)
                {
                    _selectStylecopSettingsCommand = new DelegateCommand(SelectStylecopSettings);
                }

                return _selectStylecopSettingsCommand;
            }
        }

        public ICommand ApplyCommand
        {
            get
            {
                if (_applyCommand == null)
                {
                    _applyCommand = new DelegateCommand(Apply);
                }

                return _applyCommand;
            }
        }

        private void Apply()
        {
            foreach (var repositoryConfigurationElement in _repositories)
            {
                var preCommitHookScript = Path.Combine(RepositoriesPath, repositoryConfigurationElement.Name, "hooks",
                    "pre-commit.cmd");
                var hookBody = string.Format(
                    BatTemplates.PreCommit,
                    _configuration.SvnLookPath,
                    _configuration.StylecopWrapper,
                    _configuration.StylecopPath,
                    _configuration.StylecopSettings,
                    _configuration.TempFolder);
                File.WriteAllText(preCommitHookScript, hookBody);
            }
        }

        public IEnumerable<RepositoryConfigurationElement> Repositories
        {
            get { return _repositories; }
        }

        private void SelectSvnUtilitiesPath()
        {
            var path = _fileSystem.SelectFolder();
            if (path != null)
            {
                SvnUtilitiesPath = path;
            }
        }

        private void SelectStylecopPath()
        {
            var path = _fileSystem.SelectFolder();
            if (path != null)
            {
                StylecopPath = path;
            }
        }

        private void SelectRepositoriesPath()
        {
            var path = _fileSystem.SelectFolder();
            if (path != null)
            {
                RepositoriesPath = path;
            }
        }

        private void SelectStylecopSettings()
        {
            var path = _fileSystem.SelectFile();
            if (path != null)
            {
                StylecopSettings = path;
            }
        }
    }
}
