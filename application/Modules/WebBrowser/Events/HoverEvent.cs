﻿using System.Text.Json;

namespace MORR.Modules.WebBrowser.Events
{
    /// <summary>
    ///     A hover user interaction
    /// </summary>
    public class HoverEvent : WebBrowserEvent
    {
        private const string serializedHoveredElementField = "target";

        /// <summary>
        ///     The element on the website that has been hovered
        /// </summary>
        public string HoveredElement { get; private set; } = "";

        protected override void DeserializeSpecificAttributes(JsonElement parsed)
        {
            HoveredElement = parsed.GetProperty(serializedHoveredElementField).GetString();
        }
    }
}