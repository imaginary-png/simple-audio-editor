using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace simple_audio_editor
{
    /// <summary>
    /// Used to set arguments for the FFmpeg command-line. You need to set a valid input and output path.
    /// Currently handles audio - volume, bit rate, and trimming;
    /// </summary>
    public class FFmpegOptions : IEquatable<FFmpegOptions>
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
        public bool InputFlag { get; set; }
        public bool OutputFlag { get; set; }
        public bool VolumeFlag { get; set; }
        public bool BitRateFlag { get; set; }
        public bool TrimFlag { get; set; } //trim flag is used in argsbuilder -- but not really needed. could remove/refactor but cbf.

        /// <summary> 
        /// Used to set arguments for the FFmpeg command-line. Input Output paths and empty.<br/>
        /// Defaults: Volume = 1.0. Bit Rate = 128.
        /// </summary>
        /// <param name="input">Input file path</param>
        /// <param name="output">Output path, creates new file and folders as necessary</param>
        /// <param name="volume">Output Volume, 1 = 100%, 0.5 = 50% of current volume. Default: 1.0</param>
        /// <param name="bitRate">Output bitrate. Default: 128</param>
        public FFmpegOptions()
        {
            Input = "";
            Output = "";
            Volume = 1;
            BitRate = 128;
            TrimTimes = new List<TrimTime>();
        }

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

        #region Trim Functions

        /// <summary>
        /// Specifies the start and end time, in seconds, of a subsection to save. Negative numbers result in start as 0 seconds.
        /// </summary>
        /// <param name="start">Start Time in seconds</param>
        /// <param name="end">End Time in seconds</param>
        public void AddTrimSection(int start, int end = 0)
        {
            //bother verifying start is positive? negative values are valid args for ffmpeg and result as if start was 0 seconds.
            TrimTimes.Add(new TrimTime(start,end));
            TrimFlag = true;
        }

        public bool RemoveTrimSection(int start, int end)
        {
            return RemoveTrimSection(new TrimTime(start, end));
        }

        
        public bool RemoveTrimSection(TrimTime trimTime)
        {
            return TrimTimes.Remove(trimTime);
        }
        #endregion

        #region Equality 

        public override bool Equals(object? obj) 
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FFmpegOptions)obj);
        }

        public bool Equals(FFmpegOptions other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _bitRate == other._bitRate && Input == other.Input && Output == other.Output && Volume.Equals(other.Volume) && Equals(TrimTimes, other.TrimTimes);
        }

        public static bool operator ==(FFmpegOptions obj1, FFmpegOptions obj2)
        {
            if (ReferenceEquals(obj1, obj2)) return true;
            if (ReferenceEquals(obj1, null)) return false;
            if (ReferenceEquals(obj2, null)) return false;
            return obj1.Equals(obj2);
        }

        public static bool operator !=(FFmpegOptions obj1, FFmpegOptions obj2) => !(obj1 == obj2);
        
        public override int GetHashCode()
        {
            return HashCode.Combine(_bitRate, Input, Output, Volume, TrimTimes);
        }

        #endregion
    }

    public readonly struct TrimTime
    {
        public readonly int Start { get; }
        public readonly int End { get; }

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