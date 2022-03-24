using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace simple_audio_editor
{
    public class FFmpegProcess
    {
        //
        private IList<FFmpegOptions> _queue;


        //begin conversion of each item in the queue.
        //call the  argsbuilder first? or do that elsewhere? A list of args from addtoqueue?
        public bool Start()
        {


            return false;
        }

        private bool Execute(string parameters)
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

                p.StartInfo.FileName = "ffmpeg.exe";
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
                return true;
            }
            else
            {
                return false;
            }
        }

        //helpers

        //add a item to queue
        public void AddToQueue()
        {
            //add to queue list

            //pass through to argsbuilder.create and add to a list of strings?
        }

        bool CheckFFmpegPath(string path)
        {
            if (File.Exists(path)) return true;


            return false;
        }


    }
}