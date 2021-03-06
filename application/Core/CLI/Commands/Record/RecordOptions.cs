﻿using CommandLine;

namespace MORR.Core.CLI.Commands.Record
{
    [Verb("record", HelpText = "Starts a new recording with the give configuration")]
    public class RecordOptions : CommandOptions
    {
        [Option('c', "config", Required = true, HelpText = "Path to configuration file")]
        public string ConfigPath { get; set; } = "";
    }
}
