using Microsoft.Win32;
using simple_audio_editor;
using simple_audio_editor_GUI.Annotations;
using simple_audio_editor_GUI.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace simple_audio_editor_GUI.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly string _default_Input = "";
        private readonly string _default_Output = "";
        private readonly double _default_Volume = 1.0;
        private readonly int _default_Bit_Rate = 128;
        private readonly int _default_Trim_Time = 0;

        private string _input;
        private string _output;
        private double _volume;
        private int _bitRate;
        private int _trimStart;
        private int _trimEnd;

        private ObservableCollection<FFmpegOptions> _ffmpegOptions { get; set; }
        private FFmpegProcess _ffmpegProcess { get; set; }
        
        public ObservableCollection<TrimTime> TrimList { get; set; }

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

        public int TrimStart
        {
            get => _trimStart;
            set
            {
                if (value == _trimStart) return;
                _trimStart = value;
                OnPropertyChanged(nameof(TrimStart));
            }
        }

        public int TrimEnd
        {
            get => _trimEnd;
            set
            {
                if (value == _trimEnd) return;
                _trimEnd = value;
                OnPropertyChanged(nameof(TrimEnd));
            }
        }

        public ICommand AddJobButtonPressed { get; set; }
        public ICommand AddTrimTimeButtonPressed { get; set; }
        public ICommand OpenFileButtonClicked { get; set; }
        public ICommand RemoveTrimPressed { get; set; }
        

        public MainWindowViewModel()
        {
            _ffmpegOptions = new ObservableCollection<FFmpegOptions>();
            TrimList = new ObservableCollection<TrimTime>();
            _ffmpegProcess = new FFmpegProcess();

            Input = _default_Input;
            Output = _default_Output;
            Volume = _default_Volume;
            BitRate = _default_Bit_Rate;
            TrimStart = _default_Trim_Time;
            TrimEnd = _default_Trim_Time;

            AddJobButtonPressed = new CustomCommand(AddJobButton_Executed);
            OpenFileButtonClicked = new CustomCommand(OpenFile_Executed);
            AddTrimTimeButtonPressed = new CustomCommand(AddTrimTime_Executed);
            RemoveTrimPressed = new GenericCommand<TrimTime>(RemoveTrimTime_Executed);

        }


        #region Command stuff

        public void AddJobButton_Executed()
        {
            //create new ffmpegoptions here.
            var opt = new FFmpegOptions();
            
            if (TrimList.Count > 0)
            {
                opt = new FFmpegOptions(Input, Output, TrimList, Volume, BitRate);
                _ffmpegOptions.Add(opt);
                //_ffmpegOptions.Add(new FFmpegOptions(Input, Output, TrimList, Volume, BitRate));
            }
            else
            {
                opt = new FFmpegOptions(Input, Output, TrimList, Volume, BitRate);
                _ffmpegOptions.Add(opt);
                //_ffmpegOptions.Add(new FFmpegOptions(Input, Output, TrimList, Volume, BitRate));
            }

            //clear current input, output, etc to default values.
            ResetAllInputs();


            var optionAsText= "";
            var argAsText = "";

            foreach (var f in _ffmpegOptions)
            {
                optionAsText += $"Options: In: {f.Input}, Out: {f.Output}, Volume: {f.Volume}, BitRate: {f.BitRate}";
                argAsText = FFmpegArgsBuilder.Create(opt);

            }

            var win = new Window();
            win.Content = new TextBox { Text = "\n\n"+ optionAsText + "\n\n" + argAsText };
            win.SizeToContent = SizeToContent.WidthAndHeight;
            win.ShowDialog();
        }
        
        public void Test_Executed()
        {
            //create new ffmpegoptions here.
            
            if (TrimList.Count > 0)
            {
                _ffmpegOptions.Add(new FFmpegOptions(Input, Output, TrimList, Volume, BitRate));
            }
            else
            {
                _ffmpegOptions.Add(new FFmpegOptions(Input, Output, TrimList, Volume, BitRate));
            }
            //clear current input, output, etc to default values.

            var text = "";

            foreach (var t in TrimList)
            {
                text += $"Trim: {t.Start} - {t.End}\n";
            }

            text += "\n\n";
            foreach (var f in _ffmpegOptions)
            {
                text += $"Options: In: {f.Input}, Out: {f.Output}, Volume: {f.Volume}, BitRate: {f.BitRate}";
            }

            var win = new Window();
            win.Content = new TextBox { Text = text+ "\n\n"+$"Input = {Input}\n\nOutput = {Output}\n\nVolume = {Volume}\n\nBitRate = {BitRate}\n\nTrim {TrimStart} - {TrimEnd}" };
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
                Output = GenerateOutputPath(Input);

            }
        }

        public void AddTrimTime_Executed()
        {
            if (TrimStart < TrimEnd)
            {
                var trimTime = new TrimTime(TrimStart, TrimEnd);
                TrimList.Add(trimTime);
            }
            TrimStart = 0;
            TrimEnd = 0;
        }

        public void RemoveTrimTime_Executed(TrimTime time)
        {
            TrimList.Remove(time);
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        #region Helpers

        private string GenerateOutputPath(string inputPath)
        {
            var inputSplit = inputPath.Split('\\');
            var outputPath = "";
            for (int i = 0; i <= inputSplit.Length - 2; i++)
            {
                outputPath += inputSplit[i] + "\\";
            }

            outputPath += "Output" + "\\" + inputSplit[^1];

            return outputPath;
        }
        
        private async void StartFFmpegProcess()
        {
            _ffmpegProcess.OptionsQueue = _ffmpegOptions;
            await _ffmpegProcess.Start();
        }

        private void ResetAllInputs()
        {
            Input = _default_Input;
            Output = _default_Output;
            Volume = _default_Volume;
            BitRate = _default_Bit_Rate;
            TrimStart = _default_Trim_Time;
            TrimEnd = _default_Trim_Time;
        }
        

        #endregion
    }
}