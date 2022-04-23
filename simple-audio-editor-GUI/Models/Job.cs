using System;
using System.ComponentModel;
using System.Printing;
using System.Runtime.CompilerServices;
using simple_audio_editor;
using simple_audio_editor_GUI.Annotations;

namespace simple_audio_editor_GUI.Models
{
    public class Job : INotifyPropertyChanged
    {
        public static readonly string WAITING = "Waiting";
        public static readonly string SUCCESS = "Finished";
        public static readonly string FAILED = "Failed";

        private string _status;

        public FFmpegOptions Options { get; }

        public string Status
        {
            get => _status;
            private set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public Job(FFmpegOptions options)
        {
            Options = options;
            Status = JobStatus.Waiting.ToString();
        }

        public void SetStatus(JobStatus status)
        {
            Status = status.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            var trimArgs = "";
            if (Options.TrimFlag)
            {
                foreach (var time in Options.TrimTimes)
                {
                    trimArgs += $"\t{time.Start} - {time.End}s\n";
                }
            }

            return $"Input:      {Options.Input}\n" +
                   $"Output:     {Options.Output}\n" +
                   $"Volume:     {Options.Volume}\n" +
                   $"Bit Rate:   {Options.BitRate}\n" +
                   $"Trim args:  \n{trimArgs}\n" +
                   $"Status:     {Status}";
        }
    }

    public enum JobStatus
    {
        Waiting,
        Processing,
        Success,
        Failed
    }
}