﻿using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace MORR.Core.CLI.Commands.Process
{
    [Verb("process", HelpText = "Starts the processing from a container file to a new output file using the configuration.")]
    internal class ProcessOptions : ICommandOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to configuration file")]
        public string ConfigPath { get; set; }

        [Option('i', "inputFile", Required = true, HelpText = "Path to input file, which should be processed")]
        public string InputFile { get; set; }
    }
}