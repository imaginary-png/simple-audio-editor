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
            var ffmpegPath = "C:\\ffmpeg\\bin\\ffmpeg.exe";

            var volume = 0.6;

            var trimStart = new List<int>() { 600, 1800, 3180 };
            var trimEnd = new List<int>() { 660, 1860, 0 };

            var beginningArg = $"-y -i \"{input}\" -filter_complex \"";
            var trimArg = TrimMultiple(trimStart, trimEnd, volume);
            var endArg = $"\" -b:a 128k -map [outa] \"{output}\"";

            var arguments = beginningArg + trimArg + endArg;

            Console.WriteLine($"-------------------------------\n{arguments}\n-------------------------------");

            // Start ffmpeg process with actual arguments
            if (File.Exists(input))
            {
                var result = Execute(ffmpegPath, arguments);
                Console.WriteLine(result);
            }
            else
            {
                Console.WriteLine("File not found.");
            }
            
            var opt = new FFmpegOptions();
            opt.Input = "Fdfs";

        }


        private static string Execute(string exePath, string parameters)
        {
            string result = String.Empty;
            var exitCode = 1;

            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                /*
                p.StartInfo.RedirectStandardOutput = true;
                //ffmpeg outputs lines as standarderror
                p.StartInfo.RedirectStandardError = true;
                p.ErrorDataReceived += (sender, args) => Console.WriteLine("received output: {0}", args.Data);
                p.OutputDataReceived += (sender, args) => Console.WriteLine("received output: {0}", args.Data);*/

                p.StartInfo.FileName = exePath;
                p.StartInfo.Arguments = parameters;
                p.Start();

                /*p.BeginErrorReadLine();
                p.BeginOutputReadLine();*/
                
                p.WaitForExit();

                //1 = fail 0 = success
                exitCode = p.ExitCode;
            }

            if (exitCode == 0)
            {
                return "done";
            }
            else
            {
                return "failed";
            }
        }



        private static string TrimMultiple(List<int> trimStart, List<int> trimEnd, double volume = 1)
        {
            var concatArg = string.Empty;
            var trimArg = string.Empty;
            var count = 0;

            for (int i = 0; i < trimStart.Count; i++, count++)
            {
                trimArg += Trim(trimStart[i], trimEnd[i], count, volume);
                concatArg += $"[{count}a]";
            }
            concatArg += $"concat=n={count}:v=0:a=1[outa]";

            return trimArg + concatArg;
        }

        private static string Trim(int start, int end, int count = 0, double volume = 1)
        {
            var trimArg = $"[0:a]atrim=start={start}";

            if (end != 0)
            {
                trimArg += $":end={end}";
            }
            trimArg += $",volume ={ volume},asetpts=PTS-STARTPTS[{count}a];";

            return trimArg;
        }
    }
}
