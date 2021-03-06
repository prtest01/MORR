﻿using System.Collections.Generic;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;

namespace MORR.Modules.Mouse
{
    public class MouseModuleConfiguration : IConfiguration
    {
        /// <summary>
        ///     The sampling rate of the mouse position capture, in Hz.
        /// </summary>
        public uint SamplingRateInHz { get; set; }

        /// <summary>
        ///     The minimal distance(computed with screen coordinates) a mouse move
        ///     must reach in a period to be recorded.
        ///     A mouse move with distance less than the Threshold will be ignored,
        ///     in other words, a new MouseMoveEvent will not be generated and
        ///     the mouse position will not be recorded.
        /// </summary>
        public int Threshold { get; set; }

        public void Parse(RawConfiguration configuration)
        {
            var element = JsonDocument.Parse(configuration.RawValue).RootElement;
            try
            {
                SamplingRateInHz = element.GetProperty(nameof(SamplingRateInHz)).GetUInt32();
                Threshold = element.GetProperty(nameof(Threshold)).GetInt32();
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidConfigurationException(
                    "Failed to parse the sampling rate and the threshold for mouse module.");
            }
        }
    }
}