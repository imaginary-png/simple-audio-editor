using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace simple_audio_editor
{
    public class FFmpegProcess
    {
        public IList<FFmpegOptions> OptionsQueue { get; private set; }

        public IList<string>
            FinishedFiles { get; private set; } //put paths to successful conversions here? for use in GUI?
        public IList<string>
            FailedFiles { get; private set; } //put paths for failed conversion inputs here? for use in GUI?
        public ObservableCollection<ConversionResult>
            Results { get; private set; } //put paths for failed conversion inputs here? for use in GUI?

        private IList<Task<ConversionResult>> _taskQueue;


        public FFmpegProcess(List<FFmpegOptions> optionsQueue)
        {
            OptionsQueue = optionsQueue;
            _taskQueue = new List<Task<ConversionResult>>();
            Results = new ObservableCollection<ConversionResult>();
            Results.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs args) =>
            {
#if DEBUG
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                //Console.WriteLine($"{input} {result}");
                if (args?.NewItems != null)
                {
                    foreach (var r in args.NewItems)
                    {
                        Console.WriteLine(r);
                    }
                }

                Console.ResetColor();
#endif
            };

        }

        /// <summary>
        /// Begin asynchronous conversion of FFmpegOptions stored in the queue
        /// </summary>
        public async Task Start()
        {
            foreach (var option in OptionsQueue)
            {
                _taskQueue.Add(Task.Run(() => Execute(FFmpegArgsBuilder.Create(option))));
            }

            var total = 0;
            while (_taskQueue.Any())
            {
                Task<ConversionResult> finishedTask = await Task.WhenAny(_taskQueue);
                _taskQueue.Remove(finishedTask);

                Results.Add(await finishedTask);
                total += 1;
            }

            Console.WriteLine($"finished converting {total} files.");
        }

        /// <summary>
        /// Starts the FFmpeg process with the passed arguments. <br/>
        /// Requires FFmpeg path to be set as an environment variable.
        /// </summary>
        /// <param name="parameters">String containing the args to be passed to the FFmpeg process, generate from FFmpegArgsBuilder.Create()</param>
        /// <returns>Returns a tuple containing the input path, and a bool based on success/failure</returns>
        private ConversionResult Execute(string parameters)
        {
            var startTime = DateTime.UtcNow;
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

            var inputStartIndex = CultureInfo.InvariantCulture.CompareInfo.IndexOf(parameters, "-i ") + 3;
            var input = parameters.Substring(inputStartIndex, CultureInfo.InvariantCulture.CompareInfo.IndexOf(parameters, "-filter") - inputStartIndex);

            var outputStartIndex = CultureInfo.InvariantCulture.CompareInfo.IndexOf(parameters, "[outa] ") + 7;
            var output = parameters.Substring(outputStartIndex, CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(parameters,"\"") - outputStartIndex + 1);
            

            if (exitCode == 0) //apparently ffmpeg will sometimes return 0 even if there is an error. 
            {
                return new ConversionResult() { Input = input, Output = output, Succeeded = true, StartTime = startTime, EndTime = DateTime.UtcNow };
               // return (input, true);
            }
            else
            {
                return new ConversionResult() { Input = input, Output = output, Succeeded = false, StartTime = startTime, EndTime = DateTime.UtcNow };
               // return (input, false);
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


    public record ConversionResult()
    {
        public string Input;
        public string Output;
        public bool Succeeded;
        public DateTime StartTime;
        public DateTime EndTime;

        public override string ToString()
        {
            var result = Succeeded ? "Finished" : "Failed";

            return $"Status: {result}\n" +
                   $"Input Path: {Input}\n" +
                   $"Output Path: {Output}\n" +
                   $"Start Time: {StartTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss.fff tt \"GMT\"zzz", CultureInfo.InvariantCulture)}\n" +
                   $"End Time: {EndTime.ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss.fff tt \"GMT\"zzz", CultureInfo.InvariantCulture)}\n";
        }
    }
}