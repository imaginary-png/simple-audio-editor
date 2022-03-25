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
        /// Used to set arguments for the FFmpeg command-line. <br/>
        /// Defaults: Volume = 1.0. Bit Rate = 128.
        /// </summary>
        /// <param name="input">Input file path</param>
        /// <param name="output">Output path, creates new file and folders as necessary</param>
        /// <param name="volume">Output Volume, 1 = 100%, 0.5 = 50% of current volume. Default: 1.0</param>
        /// <param name="bitRate">Output bitrate. Default: 128</param>
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

        /// <summary>
        /// Used to set arguments for the FFmpeg command-line. <br/>
        /// Defaults: Volume = 1.0. Bit Rate = 128.
        /// </summary>
        /// <param name="input">Input file path</param>
        /// <param name="output">Output path, creates new file and folders as necessary</param>
        /// <param name="trimTimes">List of trim times for getting subsections of audio to stitch together into the output file</param>
        /// <param name="volume">Output Volume, 1 = 100%, 0.5 = 50% of current volume. Default: 1.0</param>
        /// <param name="bitRate">Output bitrate. Default: 128</param>
        public FFmpegOptions(string input, string output, IList<TrimTime> trimTimes, double volume = 1.0, int bitRate = DefaultBitRate) :
            this(input, output, volume, bitRate)
        {
            TrimTimes = trimTimes;
            TrimFlag = true;
        }

        /// <summary>
        /// Specifies the start and end time of a subsection to save. Negative numbers result in start as 0 seconds.
        /// </summary>
        /// <param name="start">Start Time</param>
        /// <param name="end">End Time</param>
        public void AddTrimSection(int start, int end = 0)
        {
            //bother verifying start is positive? negative values are valid args for ffmpeg and result as if start was 0 seconds.
            TrimTimes.Add(new TrimTime() { Start = start, End = end });
            TrimFlag = true;
        }

        public bool RemoveTrimSection(int start, int end)
        {
            //todo
            return false;
        }

        public bool RemoveTrimSection(TrimTime trimTime)
        {
            //todo
            return true;
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