using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace simple_audio_editor
{
    class Program
    {
        static void Main(string[] args)
        {
            // test inputs
            var input = "C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\test1.mp3";
            var output = "C:\\PROGRAMMING STUFF\\C#\\simple-audio-editor\\OUTPUT.mp3";

            var volume = 0.6;

            var trimStart = new List<int>() {600, 1800, 3180};
            var trimEnd = new List<int>() {660, 1860, 0};

            var opt = new FFmpegOptions(input, output, trimStart, trimEnd, volume);
        }
    }
}
