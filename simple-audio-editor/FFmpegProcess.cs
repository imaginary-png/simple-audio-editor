using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace simple_audio_editor
{
    public class FFmpegProcess
    {
        //
        public IList<FFmpegOptions> OptionsQueue { get; private set; }

        private IList<Task<(string, bool)>> _taskQueue;

        public FFmpegProcess(List<FFmpegOptions> optionsQueue)
        {
            OptionsQueue = optionsQueue;
            _taskQueue = new List<Task<(string, bool)>>();
        }
        
        /// <summary>
        /// Begin asynchronous conversion of FFmpegOptions stored in the queue
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Start()
        {
            foreach (var option in OptionsQueue)
            {
                _taskQueue.Add(Task.Run(() => Execute(FFmpegArgsBuilder.Create(option))));
            }

            var total = 0;
            while (_taskQueue.Any())
            {
                Task<(string, bool)> finishedTask = await Task.WhenAny(_taskQueue);
                _taskQueue.Remove(finishedTask);

                var (input, result) = await finishedTask;

                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine($"{input} {result}");
                Console.ResetColor();

                total += 1;
            }

            Console.WriteLine($"finished converting {total} files.");
            return false;
        }

        private (string, bool) Execute(string parameters)
        {
            string result = String.Empty;
            var exitCode = 1;

            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;

                p.StartInfo.RedirectStandardOutput = true;
                p.OutputDataReceived += (sender, args) => Console.WriteLine("received output: {0}", args.Data);

                //ffmpeg outputs lines as standarderror
                p.StartInfo.RedirectStandardError = true;
                //p.ErrorDataReceived += (sender, args) => Console.WriteLine("received Error: {0}", args.Data);

                p.StartInfo.FileName = "ffmpeg.exe";
                p.StartInfo.Arguments = parameters;
                p.Start();

                //p.BeginOutputReadLine();
                p.BeginErrorReadLine();


                p.WaitForExit();

                //1 = fail 0 = success
                exitCode = p.ExitCode;
            }

            var start = parameters.IndexOf("-i ") + 3;
            var input = parameters.Substring(start, parameters.IndexOf("-filter") - start);

            if (exitCode == 0)
            {
                return (input,true);
            }
            else
            {
                return (input, false);
            }
        }

        //helpers

        /// <summary>
        /// Queue a FFmpegOptions object for processing
        /// </summary>
        /// <param name="option"></param>
        public void AddToQueue(FFmpegOptions option)
        {
            OptionsQueue.Add(option);
            //add to queue list

            //pass through to argsbuilder.create and add to a list of strings?
        }
    }
}