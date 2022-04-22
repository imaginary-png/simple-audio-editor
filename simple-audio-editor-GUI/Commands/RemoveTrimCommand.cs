using System;
using System.Windows.Input;
using simple_audio_editor;

namespace simple_audio_editor_GUI.Commands
{
    public class RemoveTrimCommand : ICommand
    {
        private Action<TrimTime> _action;

        public RemoveTrimCommand(Action<TrimTime> action)
        {
            _action = action;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is TrimTime)
            {
                _action?.Invoke((TrimTime)parameter);
            }
        }

        public event EventHandler? CanExecuteChanged;
    }
}