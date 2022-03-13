using System;
using System.Collections.Generic;
using System.IO;

namespace simple_audio_editor
{
    /// <summary>
    /// Used to set arguments for the FFmpeg command-line. You need to set a valid input and output path, and your ffmpeg.exe path.
    /// Currently handles audio - volume, bit rate, and trimming;
    /// </summary>
    public class FFmpegOptions
    {
        // test inputs
        private string _input;

        /// <summary>
        /// The path for the Input file. If the Path does not exist, the current input path will remain unchanged.
        /// </summary>
        public string Input
        {
            get => _input;
            set
            {
                if (File.Exists(value))
                {
                    _input = value;
                    Console.WriteLine("file exists");
                }
                else
                {
                    Console.WriteLine("File not found.");
                }
            }
        }

        private string _output;
        public string Output
        {
            get => _output;
            set
            {
                if (File.Exists(value))
                {
                    _output = value;
                    Console.WriteLine("file exists");
                }
                else
                {
                    Console.WriteLine("File not found.");
                }
            }
        }

        private string _ffmpegPath;
        public string FFmpegPath
        {
            get => _ffmpegPath;
            set
            {
                if (File.Exists(value))
                {
                    _ffmpegPath = value;
                }
            }
        }

        private double _volume;
        public double Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                _volumeArg = $" \"volume={value}\"";
                //_volumeArg = $"-filter:a \"volume={value}\"";
            }
        } //decimal places and negative numbers don't seem to matter, just makes things quieter. 100 = super damn loud. 1 = 100% volume(normal), 1.5 = 150%, 0.5 = 50%

        private int _bitRate;
        public int BitRate
        {
            get => _bitRate;
            set
            {
                if (value is > 24 and < 400)
                {
                    _bitRate = value;
                    _bitRateArg = $" -b:a {_bitRate}k";
                }
                else
                {
                    _bitRate = 128;
                    _bitRateArg = $" -b:a {_bitRate}k";
                }
            }
        }


        private IList<int> _trimStart = new List<int>() { 600, 1800, 3180 };
        private IList<int> _trimEnd = new List<int>() { 660, 1860, 0 };

        private string _trimArg;
        private string _volumeArg;
        private string _bitRateArg;
        private string _beginningArg;
        private string _trimMapOutArg;
        private string _endArg;

        private string _arguments;

        /// <summary>
        /// Used to set arguments for the FFmpeg command-line. You need to set a valid input and output path, and your ffmpeg.exe path. <br/>
        /// Defaults: Input, Output, FFmpegPath = "". Volume = 1.0. Bit Rate = 128.
        /// </summary>
        public FFmpegOptions()
        {
            Input = "C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\test1.mp3";
            Output = "C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\OUTPUT.mp3";
            FFmpegPath = "C:\\ffmpeg\\bin\\ffmpeg.exe";

            Volume = 1.0;
            BitRate = 128;

            _beginningArg = $"-y -i \"{_input}\" -filter_complex \"";

            _trimMapOutArg = $" -map [outa] ";
            _endArg = $"\"{_output}\"";

            _arguments = _beginningArg + _volumeArg + _bitRateArg + _endArg;


            //_bitRateArg += "-b:a 128k";x

            Console.WriteLine($"-------------------------------\n{_arguments}\n-------------------------------");
        }










        #region old unused , probably delete later

        

        
        private bool SetInput(string input)
        {
            if (File.Exists(input))
            {
                _input = input;
                Console.WriteLine("file exists");
                return true;
            }
            else
            {
                Console.WriteLine("File not found.");
                _input = "";
                return false;
            }
        }

        private bool SetOutput(string output)
        {
            if (File.Exists(output))
            {
                _output = output;
                Console.WriteLine("file exists");
                return true;
            }
            else
            {
                Console.WriteLine("File not found.");
                _output = "";
                return false;
            }
        }

        private bool SetFFmpegPath(string path)
        {
            if (File.Exists(path))
            {
                _ffmpegPath = path;
                return true;
            }

            return false;

        }


        private void SetBitRate(int bitRate)
        {
            if (bitRate is > 24 and < 320)
            {
                _bitRateArg += "-b:a " + bitRate + "k";
            }

        }

        #endregion
    }
}