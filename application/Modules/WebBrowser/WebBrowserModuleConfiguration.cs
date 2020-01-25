﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text.Json;
using MORR.Core.Configuration;
using MORR.Shared.Configuration;

namespace MORR.Modules.WebBrowser
{
    [Export(typeof(IConfiguration))]
    [Export(typeof(WebBrowserModuleConfiguration))]
    public class WebBrowserModuleConfiguration : IConfiguration
    {
        /// <summary>
        ///     A combination of port number and optionally directory.
        ///     Will be appended to the localhost-prefix defined in <see cref="WebExtensionListener" />.
        ///     Must end in a slash ('/') character.
        ///     Examples for valid values are "60024/" or "60024/johndoe/".
        /// </summary>
        public string UrlSuffix;

        public void Parse(RawConfiguration configuration)
        {
            var element = JsonDocument.Parse(configuration.RawValue).RootElement;
            try
            {
                UrlSuffix = element.GetProperty("UrlSuffix").GetString();
            }
            catch (KeyNotFoundException)
            {
                throw new InvalidConfigurationException("Failed to parse UrlSuffix.");
            }
        }
    }
}