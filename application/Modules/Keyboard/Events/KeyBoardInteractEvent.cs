﻿namespace MORR.Modules.Keyboard.Events
{
    /// <summary>
    ///     A keyboard user interaction
    /// </summary>
    public class KeyBoardInteractEvent : KeyboardEvent
    {
        /// <summary>
        ///     The key pressed
        /// </summary>
        public System.Windows.Input.Key PressedKey { get; set; }
    }
}
