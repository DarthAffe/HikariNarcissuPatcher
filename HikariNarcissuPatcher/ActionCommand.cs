using System;
using System.Windows.Input;

namespace HikariNarcissuPatcher
{
    public class ActionCommand : ICommand
    {
        #region Properties & Fields

        private readonly Func<bool> _canExecute;
        private readonly Action _command;

        #endregion

        #region Constructor

        public ActionCommand(Action command, Func<bool> canExecute = null)
        {
            this._command = command;
            this._canExecute = canExecute;
        }

        #endregion

        #region Methods

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            if (_command != null)
                _command();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged.Invoke(this, new EventArgs());
        }

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion
    }
}
