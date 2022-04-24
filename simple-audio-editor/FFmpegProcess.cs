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
        public IList<FFmpegOptions> OptionsQueue { get; set; }

        public IList<string> FinishedFiles { get; private set; } //put paths to successful conversions here? for use in GUI?
        public IList<string> FailedFiles { get; private set; } //put paths for failed conversion inputs here? for use in GUI?
        public ObservableCollection<ConversionResult> Results { get; private set; } //put paths for failed conversion inputs here? for use in GUI?

        private IList<Task<ConversionResult>> _taskQueue;

        public FFmpegProcess()
        {
            OptionsQueue = new List<FFmpegOptions>();
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
                        Trace.WriteLine(r);
                    }
                }

                Console.ResetColor();
#endif
            };
        }

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
                        Trace.WriteLine(r);
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
            //reset results list before new batch of jobs
            Results.Clear();

            Trace.WriteLine("FFmpeg Starting...");
            foreach (var option in OptionsQueue)
            {
                Trace.WriteLine("Using Args: " + FFmpegArgsBuilder.Create(option));

                MakeOutputDir(option.Output);
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
            Trace.WriteLine($"finished converting {total} files.");
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
                p.OutputDataReceived += (sender, args) =>
                {
                    Console.WriteLine($"received output: {args.Data}");
                    Trace.WriteLine($"received output: {args.Data}");
                };

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

            return GetConversionResult(exitCode, startTime, parameters);
        }
        
        #region Helpers

        private void MakeOutputDir(string output)
        {
            var split = output.Split("\\");

            var outputDir = "";
            for (var i = 0; i < split.Length-1; i++)
            {
                outputDir += split[i] + "\\";
            }

            Trace.WriteLine("Making Output Folder... " + outputDir);
            Directory.CreateDirectory(outputDir);
        }

        private ConversionResult GetConversionResult(int exitCode, DateTime startTime, string parameters)
        {
            var inputStartIndex = CultureInfo.InvariantCulture.CompareInfo.IndexOf(parameters, "-i ") + 4;
            var input = parameters.Substring(inputStartIndex, CultureInfo.InvariantCulture.CompareInfo.IndexOf(parameters, "\" -filter") - inputStartIndex);

            var outputStartIndex = 0;
            var output = "";
            if (parameters.Contains("[outa]"))
            {
                outputStartIndex = CultureInfo.InvariantCulture.CompareInfo.IndexOf(parameters, "[outa] \"") + 8;
                output = parameters.Substring(outputStartIndex,
                    CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(parameters, "\"") - outputStartIndex);
                output += "||><>o<><";
            }
            else
            {
                outputStartIndex = CultureInfo.InvariantCulture.CompareInfo.IndexOf(parameters, "k \"") + 3;
                output = parameters.Substring(outputStartIndex,
                    CultureInfo.InvariantCulture.CompareInfo.LastIndexOf(parameters, "\"") - outputStartIndex);
                
            }


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
        

        #region Add and remove to OptionsQueue methods

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


        public bool RemoveFromQueue(IList<FFmpegOptions> options)
        {
            var optionsQueueAsList = OptionsQueue.ToList();
            var removedCount = optionsQueueAsList.RemoveAll(o => options.Any(opt => opt == o));
            OptionsQueue = optionsQueueAsList;

            return (removedCount == options.Count);
        }

        public void RemoveFromQueue(IList<ConversionResult> finishedTasks)
        {
            foreach (var conversionResult in finishedTasks)
            {
                if (conversionResult.Succeeded)
                {
                    var toRemove =
                        OptionsQueue.FirstOrDefault(o =>
                            o.Input == conversionResult.Input &&
                            o.Output == conversionResult.Output);
                    RemoveFromQueue(toRemove);
                }
            }
        }

        public bool RemoveFromQueue(FFmpegOptions option)
        {
            return OptionsQueue.Remove(option);
        }

        public bool ClearQueue()
        {
           OptionsQueue.Clear();

           if (OptionsQueue.Count == 0) return true;

           return false;
        }
        #endregion

        #endregion
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