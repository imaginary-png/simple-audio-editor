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
        const int DefaultBitRate = 128;

        // test inputs
        private string _input;
        //  I SHOULD REMOVE THE ARG STRINGS AND FORMULATE THEM IN THE PROCESS CLASS BASED ON THE FLAGS / INPUTS IN THE OPTIONS
        //  BECAUSE THE OPTIONS SHOULD ONLY CONTAIN THE OPTIONS SELECTED BY THE USER
        // THIS WAY I CAN FORM THE ARGS TO BE PASSED TO FFMPEG IN ONE METHOD, IN THE PROCESS CLASS.
        //  WHICH SHOULD MAKE IT MUCH EASIER TO UPDATE IN THE FUTURE FOR MORE FUNCTIONALITY.

        /// <summary>
        /// The path for the Input file. If the Path does not exist, the current input path will remain unchanged.
        /// </summary>
        public string Input
        {
            get => _input;
            set => _input = value; //not going to check File.Exists, leave that up to user / ffmpeg.
        }

        private string _output;
        public string Output
        {
            get => _output;
            set => _output = value;
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
                    //throw error
                }
            }
        }


        private IList<int> _trimStart = new List<int>() { 600, 1800, 3180 };
        public IList<int> TrimStart
        {
            get => _trimStart;
            private set => _trimStart = value;
        }

        private IList<int> _trimEnd = new List<int>() { 660, 1860, 0 };
        public IList<int> TrimEnd
        {
            get => _trimEnd;
            private set => _trimEnd = value;
        }

        private string _trimArg;
        private string _volumeArg;
        private string _bitRateArg;
        private string _beginningArg;
        private string _trimMapOutArg;
        private string _endArg;

        private string _arguments;

        public bool InputFlag = false;
        public bool OutputFlag = false;
        public bool VolumeFlag = false;
        public bool BitRateFlag = false;
        public bool TrimFlag = false; //flags for checking if options selected, used for forming arguments to pass.

        /// <summary>
        /// Used to set arguments for the FFmpeg command-line. You need to set a valid input and output path, and your ffmpeg.exe path. <br/>
        /// Defaults: Input, Output, FFmpegPath = "". Volume = 1.0. Bit Rate = 128.
        /// </summary>
        public FFmpegOptions(string input, string output, double volume = 1.0, int bitRate = DefaultBitRate)
        {
            _input = input;
            InputFlag = true;

            _output = output;
            OutputFlag = true;

            _volume = volume;
            VolumeFlag = true;

            _bitRate = bitRate;
            BitRateFlag = true;
        }

        public FFmpegOptions(string input, string output, List<int> trimStart, List<int> trimEnd, double volume = 1.0, int bitRate = DefaultBitRate)
        {
            _input = input;
            InputFlag = true;

            _output = output;
            OutputFlag = true;

            _volume = volume;
            VolumeFlag = true;

            _bitRate = bitRate;
            BitRateFlag = true;

            if (trimStart.Count == trimEnd.Count)
            {
                _trimStart = trimStart;
                _trimEnd = trimEnd;
                TrimFlag = true;
            }
        }


        public void AddTrimSection(int start, int end = 0)
        {
            TrimStart.Add(start);
            TrimEnd.Add(end);
            TrimFlag = true;
        }

    }
}