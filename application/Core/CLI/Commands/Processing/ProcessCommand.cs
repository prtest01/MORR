﻿using MORR.Core.CLI.Output;
using MORR.Core.Session;
using MORR.Shared.Utility;
using System;
using System.IO;

namespace MORR.Core.CLI.Commands.Processing
{
    internal class ProcessCommand : ICommand<ProcessOptions>
    {
        private const string loadedFileMessage = "Load configuration file.";
        private const string loadInputMessage = "Load input file.";
        private const string sessionManagerMessage = "Load session manager with configuration file.";
        private const string startProcessingMessage = "Start processing session:";
        private const string completeProcessingMessage = "Processing did complete!";

        public int Execute(ProcessOptions options)
        {
            try
            {
                OutputFormatter.IsVerbose = options.IsVerbose;

                // Load configuration file
                OutputFormatter.PrintDebug(loadedFileMessage);
                var configPath = new FilePath(Path.GetFullPath(options.ConfigPath));

                // Load input file
                OutputFormatter.PrintDebug(loadInputMessage);
                var inputPath = new DirectoryPath(Path.GetFullPath(options.InputFile));

                // Start session manager
                OutputFormatter.PrintDebug(sessionManagerMessage);
                ISessionManager sessionManager = new SessionManager(configPath);

                // Start processing
                OutputFormatter.PrintDebug(startProcessingMessage);
                sessionManager.Process(new[] { inputPath });
                OutputFormatter.PrintDebug(completeProcessingMessage);
                return 0;
            }
            catch (Exception exception) // I know this is not a recommend way to deal with exception, however this method receives a arbitrary amount of exception types.
            {
                OutputFormatter.PrintError(exception);
                return -1;
            }
        }
    }
}
