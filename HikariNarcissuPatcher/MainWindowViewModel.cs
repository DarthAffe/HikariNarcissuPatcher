using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace HikariNarcissuPatcher
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Properties & Fields

        private Patcher _patcher;

        private bool _showStartGame;
        public bool ShowStartGame
        {
            get { return _showStartGame; }
            private set
            {
                _showStartGame = value;
                this.OnPropertyChanged("ShowStartGame");
            }
        }

        #endregion

        #region Commands

        private ActionCommand _exitCommand;
        public ActionCommand ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new ActionCommand(Exit)); }
        }

        private ActionCommand _websiteCommand;
        public ActionCommand WebsiteCommand
        {
            get { return _websiteCommand ?? (_websiteCommand = new ActionCommand(OpenWebsite)); }
        }

        private ActionCommand _startCommand;
        public ActionCommand StartCommand
        {
            get { return _startCommand ?? (_startCommand = new ActionCommand(Start)); }
        }

        private ActionCommand _installCommand;
        public ActionCommand InstallCommand
        {
            get { return _installCommand ?? (_installCommand = new ActionCommand(Install)); }
        }

        private ActionCommand _uninstallCommand;
        public ActionCommand UninstallCommand
        {
            get { return _uninstallCommand ?? (_uninstallCommand = new ActionCommand(Uninstall)); }
        }

        #endregion

        #region constructor

        public MainWindowViewModel()
        {
            this._patcher = new Patcher();
            if (!_patcher.InitializeGamePath())
                Exit();

            ShowStartGame = _patcher.InitializeSteamPath();
        }

        #endregion

        #region Methods

        private void Start()
        {
            _patcher.StartGame();
        }

        private void Install()
        {
            _patcher.Install();
        }

        private void Uninstall()
        {
            _patcher.Uninstall();
        }

        private void OpenWebsite()
        {
            Process.Start("http://www.hikari-translations.de");
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
