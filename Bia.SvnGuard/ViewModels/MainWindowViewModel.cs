using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Bia.SvnGuard.Configuration;
using Bia.SvnGuard.Properties;
using Bia.SvnGuard.Services;
using MessageBox = System.Windows.MessageBox;

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
        private string _selectedRepository;
        private DelegateCommand _selectSvnUtilitiesPathCommand;
        private DelegateCommand _selectStylecopPathCommand;
        private DelegateCommand _selectRepositoriesPathCommand;
        private DelegateCommand _applyCommand;
        private DelegateCommand _selectStylecopSettingsCommand;
        private DelegateCommand _addRepositoryCommand;

        public MainWindowViewModel(Configuration.Configuration configuration, FileSystemService fileSystem)
        {
            _configuration = configuration;
            if (_configuration.FirstRun)
            {
                var currentPath = AppDomain.CurrentDomain.BaseDirectory;
                _configuration.StylecopWrapper = Path.Combine(currentPath, "Bia.StylecopWrapper.exe");
                _configuration.TempFolder = Path.Combine(currentPath, "Temp");
                _configuration.FirstRun = false;
            }

            _fileSystem = fileSystem;
            _svnUtilitiesPath = _configuration.SvnLookPath;
            _stylecopPath = _configuration.StylecopPath;
            _repositoriesPath = _configuration.RepositoriesPath;
            Repositories = new ObservableCollection<RepositoryViewModel>(_configuration.RepositoriesConfig.Repositories.Cast<RepositoryConfigurationElement>().Select(e => new RepositoryViewModel(e)));
            AvailableRepositories = new ObservableCollection<string>(_fileSystem.ListFolders(_configuration.RepositoriesPath).Except(Repositories.Select(r => r.Name)));
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

        public string SelectedRepository
        {
            get { return _selectedRepository; }
            set
            {
                _selectedRepository = value;
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

        public ICommand AddRepositoryCommand
        {
            get
            {
                if (_addRepositoryCommand == null)
                {
                    _addRepositoryCommand = new DelegateCommand(AddRepository);
                }

                return _addRepositoryCommand;
            }
        }

        private void AddRepository()
        {
            var configurationElement = new RepositoryConfigurationElement()
            {
                Name = SelectedRepository,
                Enabled = true
            };

            Repositories.Add(new RepositoryViewModel(configurationElement));
            AvailableRepositories.Remove(SelectedRepository);

            _configuration.AddRepository(configurationElement);
        }

        private void Apply()
        {
            int updated = 0;
            int deleted = 0;

            foreach (var repository in Repositories)
            {
                var preCommitHookScript = Path.Combine(RepositoriesPath, repository.Name, "hooks",
                    "pre-commit.cmd");
                if (repository.Enabled)
                {
                    var hookBody = string.Format(
                        BatTemplates.PreCommit,
                        _configuration.SvnLookPath,
                        _configuration.StylecopWrapper,
                        _configuration.StylecopPath,
                        _configuration.StylecopSettings,
                        _configuration.TempFolder);
                    File.WriteAllText(preCommitHookScript, hookBody);
                    updated++;
                }
                else
                {
                    File.Delete(preCommitHookScript);
                    deleted++;
                }
            }

            MessageBox.Show(String.Format("{0} updated, {1} deleted.", updated, deleted), "Done!", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public ObservableCollection<RepositoryViewModel> Repositories { get; set; }

        public ObservableCollection<string> AvailableRepositories { get; set; }

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
