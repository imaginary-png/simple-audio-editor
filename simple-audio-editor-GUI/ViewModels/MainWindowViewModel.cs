using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
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
        public ICommand OpenFileButtonClicked { get; set; }

        public MainWindowViewModel()
        {
            Input = "hello";
            Output = "Goodbye";
            Volume = 1.0;
            BitRate = 128;


            AddButtonPressed = new AddToQueueCommand(AddButton_Executed);
            OpenFileButtonClicked = new OpenFileCommand(OpenFile_Executed);


        }
        


        
        public void AddButton_Executed()
        {
            var win = new Window();
            win.Content = new TextBox {Text = $"Input = {Input}\n\nOutput = {Output}\n\nVolume = {Volume}\n\nBitRate = {BitRate}"};
            win.SizeToContent = SizeToContent.WidthAndHeight;
            win.ShowDialog();
        }

        public void OpenFile_Executed()
        {
            var open = new OpenFileDialog();
            open.Filter = "Mp3 (*.mp3)|*.mp3|Mp4 (*.mp4)|*.mp4|WebM (*.webm)|*.webm";
            open.ShowDialog();
            if (open.FileName != string.Empty)
            {
                Input = open.FileName;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}