using System;
using System.Windows.Input;
using simple_audio_editor;

namespace simple_audio_editor_GUI.Commands
{
    public class GenericCommand<T> : ICommand
    {
        private Action<T> _action;

        public GenericCommand(Action<T> action)
        {
            _action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _action?.Invoke((T)parameter);
        }

        public event EventHandler? CanExecuteChanged;
    }
}