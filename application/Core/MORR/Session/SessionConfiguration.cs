﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;
using MORR.Shared.Utility;

namespace MORR.Core.Session
{
    public class SessionConfiguration : IConfiguration
    {
        /// <summary>
        ///     The types of the encoders to use.
        /// </summary>
        public IEnumerable<Type> Encoders { get; private set; }

        /// <summary>
        ///     The types of the decoders to use.
        /// </summary>
        public IEnumerable<Type>? Decoders { get; private set; }

        /// <summary>
        ///     The directory in which to store new recordings.
        /// </summary>
        public DirectoryPath RecordingDirectory { get; private set; }

        public void Parse(RawConfiguration configuration)
        {
            var element = JsonDocument.Parse(configuration.RawValue).RootElement;

            var encoders = new List<Type>();

            if (!element.TryGetProperty(nameof(Encoders), out var encodersElement))
            {
                throw new InvalidConfigurationException("Failed to parse encoders property.");
            }

            foreach (var encoderElement in encodersElement.EnumerateArray())
            {
                if (!TryGetType(encoderElement, out var encoder))
                {
                    throw new InvalidConfigurationException("Failed to parse encoder type.");
                }

                encoders.Add(encoder);
            }

            Encoders = encoders;

            // Specifying a decoder is optional; do not throw an error if the property does not exist
            if (element.TryGetProperty(nameof(Decoders), out var decodersElement))
            {
                var decoders = new List<Type>();

                foreach (var decoderElement in decodersElement.EnumerateArray())
                {
                    if (!TryGetType(decoderElement, out var decoder))
                    {
                        throw new InvalidConfigurationException("Failed to parse decoder type.");
                    }

                    decoders.Add(decoder);
                }

                Decoders = decoders;
            }

            if (!element.TryGetProperty(nameof(RecordingDirectory), out var directoryElement))
            {
                throw new InvalidConfigurationException("Failed to parse directory path.");
            }

            var directoryPath = directoryElement.GetString();
            directoryPath = Environment.ExpandEnvironmentVariables(directoryPath);
            RecordingDirectory = new DirectoryPath(directoryPath);
        }

        private static bool TryGetType(JsonElement element, [NotNullWhen(true)] out Type? value)
        {
            value = Utility.GetTypeFromAnyAssembly(element.ToString());
            return value != null;
        }
    }
}