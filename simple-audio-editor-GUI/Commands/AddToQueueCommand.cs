using System;
using System.Windows.Input;

namespace simple_audio_editor_GUI.Commands
{
    public class AddToQueueCommand : ICommand
    {
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Executed?.Invoke(null, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;

        public event EventHandler Executed;
    }
}