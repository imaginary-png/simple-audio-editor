using System.Collections.Generic;
using System.ComponentModel;

namespace simple_audio_editor
{
    public static class FFmpegArgsBuilder
    {
        static string Input = "C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\test1.mp3";
        static string Output = "C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\OUTPUT.mp3";
        static string FFmpegPath = "C:\\ffmpeg\\bin\\ffmpeg.exe";


        private static string _beginningArg = $"-y -i \"{Input}\" -filter_complex \"";
        private static string _trimMapOutArg = $"-map [outa]";
        private static string _endArg = $"\"{Output}\"";

        /// <summary>
        /// Returns an string that represents the FFmpeg args for the conversion options.
        /// </summary>
        /// <param name="options">FFmpegOptions to build args from.</param>
        /// <returns></returns>
        public static string Create(FFmpegOptions options)
        {
            //create arg string
            var trimArg = "";
            var bitRateArg = $"-b:a {options.BitRate}k";

            if (options.TrimFlag)
            {
                trimArg = CreateTrimArg(options);
                return $"-i \"{options.Input}\" -filter_complex \"{trimArg}\" {bitRateArg} {_trimMapOutArg} \"{options.Output}\"";
            }

            //else no trimming, so just adjust volume and/or bitrate;
            return $"-i \"{options.Input}\" -filter_complex \"volume={options.Volume}\" {bitRateArg} \"{options.Output}\"";
        }

        /// <summary>
        /// Loops through and passes the trim Start and End values to the TrimSection method to create the concatenated string of all trim actions.<br/>
        /// Finally, appends the concat arg needed to merge the streams. <br/>
        /// See: https://ffmpeg.org/ffmpeg-filters.html#atrim <br/>
        /// https://trac.ffmpeg.org/wiki/Concatenate
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        internal static string CreateTrimArg(FFmpegOptions options)
        {
            var trimArg = "";
            var concatArg = "";
            var count = 0;

            for (; count < options.TrimStart.Count; count++)
            {
                trimArg += TrimSection(count, options.TrimStart[count], options.TrimEnd[count], options.Volume);
                concatArg += $"[{count}a]";
            }

            concatArg += $"concat=n={count}:v=0:a=1[outa]"; //merge into stream outa

            return trimArg + concatArg;
        }

        /// <summary>
        /// Trim a section of the audio and keep that subsection. <br/>
        /// e.g. TrimSection(30,90) would save a one minute clip from 30s to 90s. <br/>
        /// Multiple subsections can be chosen for the output file. <br/>
        /// Leaving out the end param will save the section from the start param to end of file. <br/>
        /// See: https://ffmpeg.org/ffmpeg-filters.html#atrim <br/>
        /// Maybe this should be called Clip or SaveSection or something, but the ffmpeg audio filter is called atrim.
        /// </summary>
        /// <param name="count">The trim section number</param>
        /// <param name="startTimeInSeconds">Save section from: (seconds)</param>
        /// <param name="endTimeInSeconds">Save section to: (seconds)<br/>Default: 0</param>
        /// <param name="volume">Volume for the trimmed section<br/>Default: 0</param>
        internal static string TrimSection(int count, int startTimeInSeconds, int endTimeInSeconds = 0, double volume = 1.0)
        {
            if (startTimeInSeconds <= endTimeInSeconds)
            {
                return $"[0:a]atrim=start={startTimeInSeconds}:end={endTimeInSeconds},volume={volume},asetpts=PTS-STARTPTS[{count}a];";
            }

            return $"[0:a]atrim=start={startTimeInSeconds},volume={volume},asetpts=PTS-STARTPTS[{count}a];";


        }
    }
}