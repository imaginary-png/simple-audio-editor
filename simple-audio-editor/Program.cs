using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace simple_audio_editor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // test inputs
            var input = "C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\test.mp3";
            var output = "C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\OUTPUT.mp3";

            var volume = 0.6;

            var trimStart = new List<int>() {600, 1800, 3180};
            var trimEnd = new List<int>() {660, 1860, 0};

            //var opt = new FFmpegOptions(input, output, trimStart, trimEnd, volume);
            var opt = new FFmpegOptions(input, "C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\OUTPUT1.mp3", trimStart, trimEnd, volume);
            var opt1 = new FFmpegOptions(input, "C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\OUTPUT2.mp3", trimStart, trimEnd, volume);
            var opt2 = new FFmpegOptions("C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\test1.mp3", "C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\OUTPUT3.mp3", trimStart, trimEnd, volume);

            var optList = new List<FFmpegOptions>() {opt, opt1, opt2};
            
            var ffmpeg = new FFmpegProcess(optList);

            await ffmpeg.Start();

            Console.WriteLine("done");

        }
    }
}
