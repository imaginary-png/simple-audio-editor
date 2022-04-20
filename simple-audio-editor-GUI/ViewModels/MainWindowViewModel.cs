using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using simple_audio_editor_GUI.Annotations;
using simple_audio_editor_GUI.Commands;

namespace simple_audio_editor_GUI.ViewModels 
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _input;
        private string _output;
        private double _volume;
        private int _bitRate;

        public string Input
        {
            get => _input;
            set
            {
                if (value == _input) return;
                _input = value;
                OnPropertyChanged(nameof(Input));
            }
        }

        public string Output
        {
            get => _output;
            set
            {
                if (value == _output) return;
                _output = value;
                OnPropertyChanged(nameof(Output));
            }
        }

        public double Volume
        {
            get => _volume;
            set
            {
                if (value.Equals(_volume)) return;
                _volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        public int BitRate
        {
            get => _bitRate;
            set
            {
                if (value == _bitRate) return;
                _bitRate = value;
                OnPropertyChanged(nameof(BitRate));
            }
        }

        public ICommand AddButtonPressed { get; set; }

        public MainWindowViewModel()
        {
            Input = "hello";
            Output = "Goodbye";
            Volume = 1.0;
            BitRate = 128;

            AddButtonPressed = new AddToQueueCommand();
            ((AddToQueueCommand) AddButtonPressed).Executed += AddButton_Executed;
        }
        


        
        public void AddButton_Executed(object o, EventArgs e)
        {
            var win = new Window();
            win.Content = new TextBox {Text = $"Volume = {Volume}\n\nBitRate = {BitRate}"};
            win.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}