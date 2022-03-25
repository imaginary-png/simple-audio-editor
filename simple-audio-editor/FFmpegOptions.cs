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

        /// <summary>
        /// The path for the Input file. If the Path does not exist, the current input path will remain unchanged.
        /// </summary>
        public string Input { get; set; } //not going to verify file exists, let user handle
        public string Output { get; set; }
        public double Volume { get; set; } //decimal places and negative numbers don't seem to matter, just makes things quieter. 100 = super  loud. 1 = 100% volume(normal), 1.5 = 150%, 0.5 = 50%

        private int _bitRate;
        public int BitRate
        {
            get => _bitRate;
            set
            {
                if (value is > 24 and < 400)
                {
                    _bitRate = value;
                }
                else
                {
                    //throw error?
                }
            }
        }

        public IList<TrimTime> TrimTimes { get; private set; }

        //unnecessary flags - prob remove. or might use in future functionality?
        public bool InputFlag = false; 
        public bool OutputFlag = false;
        public bool VolumeFlag = false;
        public bool BitRateFlag = false;
        public bool TrimFlag = false; //trim flag is used in argsbuilder.

        /// <summary>
        /// Used to set arguments for the FFmpeg command-line. You need to set a valid input and output path, and your ffmpeg.exe path. <br/>
        /// Defaults: Input, Output, FFmpegPath = "". Volume = 1.0. Bit Rate = 128.
        /// </summary>
        public FFmpegOptions(string input, string output, double volume = 1.0, int bitRate = DefaultBitRate)
        {
            Input = input;
            InputFlag = true;

            Output = output;
            OutputFlag = true;

            Volume = volume;
            VolumeFlag = true;

            _bitRate = bitRate;
            BitRateFlag = true;

            TrimTimes = new List<TrimTime>();
        }

        public FFmpegOptions(string input, string output, IList<TrimTime> trimTimes, double volume = 1.0, int bitRate = DefaultBitRate) :
            this(input, output, volume, bitRate)
        {
            TrimTimes = trimTimes;
            TrimFlag = true;
        }

        public void AddTrimSection(int start, int end = 0)
        {
            TrimTimes.Add(new TrimTime() { Start = start, End = end });
            TrimFlag = true;
        }
    }

    public struct TrimTime
    {
        public int Start;
        public int End;

        public TrimTime(int start, int end = 0)
        {
            Start = start;
            End = end;
        }

        public override string ToString()
        {
            return Start + " " + End;
        }
    }
}