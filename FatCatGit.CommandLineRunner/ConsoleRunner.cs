﻿using System;
using System.Diagnostics;

namespace FatCatGit.CommandLineRunner
{
    public class ConsoleRunner : Runner
    {
        public ConsoleRunner(ConsoleCommand consoleCommand)
        {
            ConsoleCommand = consoleCommand;
            Output = string.Empty;
            ErrorOutput = string.Empty;
        }

        public ConsoleCommand ConsoleCommand { get; set; }
        public string Output { get; set; }

        private ProcessStartInfo StartInfo { get; set; }
        private Process Process { get; set; }

        public string ErrorOutput { get; set; }
        private CommandAsyncResult Result { get; set; }

        public event Action<DataReceivedEventArgs> OutputReceived;

        public void Execute()
        {
            IAsyncResult result = BeginExecute();

            result.AsyncWaitHandle.WaitOne();
        }

        private void ExecuteProcess()
        {
            CreateProcessStartInfo();

            FindCommandOutput();
        }

        private void FindCommandOutput()
        {
            CreateProcess();

            RegisterForOutputEvents();

            StartProcess();

            Process.WaitForExit();
        }

        private void StartProcess()
        {
            Process.Start();

            Process.BeginOutputReadLine();
            Process.BeginErrorReadLine();
        }

        private void RegisterForOutputEvents()
        {
            Process.OutputDataReceived += OutputDataRecieved;
            Process.ErrorDataReceived += ErrorDataRecieved;
        }

        private void ErrorDataRecieved(object sender, DataReceivedEventArgs e)
        {
            ErrorOutput += e.Data;

            TriggerErrorReceivedEvent(e);
        }

        private void TriggerErrorReceivedEvent(DataReceivedEventArgs e)
        {
            if (ErrorOutputReceived != null)
            {
                ErrorOutputReceived(e);
            }
        }

        private void OutputDataRecieved(object sender, DataReceivedEventArgs e)
        {
            Output += e.Data;

            TriggerOutputReceivedEvent(e);
        }

        private void TriggerOutputReceivedEvent(DataReceivedEventArgs e)
        {
            if (OutputReceived != null)
            {
                OutputReceived(e);
            }
        }

        private void CreateProcess()
        {
            Process = new Process
                          {
                              StartInfo = StartInfo
                          };
        }

        private void CreateProcessStartInfo()
        {
            StartInfo = new ProcessStartInfo(ConsoleCommand.CommandFullLocation)
                            {
                                RedirectStandardOutput = true,
                                RedirectStandardInput = true,
                                RedirectStandardError = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true,
                                UseShellExecute = false,
                                Arguments = ConsoleCommand.Arguments,
                                WorkingDirectory = ConsoleCommand.WorkingDirectory
                            };
        }

        public IAsyncResult BeginExecute()
        {
            StartAsyncExecute();

            return Result;
        }

        private void StartAsyncExecute()
        {
            var lockObj = new object();

            lock (lockObj)
            {
                CreateCommandResult();

                StartExecutionProcessThread();
            }
        }

        private void StartExecutionProcessThread()
        {
            Action executationProcess = RunExecuteProcess;

            executationProcess.BeginInvoke(null, null);
        }

        private void CreateCommandResult()
        {
            Result = new CommandAsyncResult();
        }

        private void RunExecuteProcess()
        {
            try
            {
                ExecuteProcess();

                Result.ProcessComplete(Output, ErrorOutput);
            }
            catch (Exception ex)
            {
                Result.ProcessComplete(null, string.Format("{0}|{1}", ex.Message, ex.StackTrace));
            }
        }

        public event Action<DataReceivedEventArgs> ErrorOutputReceived;
    }
}