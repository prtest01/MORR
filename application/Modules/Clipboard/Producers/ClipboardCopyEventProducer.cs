﻿using System;
using MORR.Modules.Clipboard.Events;
using MORR.Modules.Clipboard.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;

namespace MORR.Modules.Clipboard.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for ClipboardCopyEvent
    /// </summary>
    public class ClipboardCopyEventProducer : DefaultEventQueue<ClipboardCopyEvent>
    {
        private const int wparamcut = 14;

        private static IClipboardWindowMessageSink? clipboardWindowMessageSink;

        private static INativeClipboard? nativeClipboard;

        #region Private methods

        private void OnClipboardUpdate(IntPtr hWnd, uint messageId, IntPtr wParam, IntPtr lParam)
        {
            if (messageId != (int) GlobalHook.MessageType.WM_CLIPBOARDUPDATE)
            {
                return;
            }

            string text;
            try
            {
                if (nativeClipboard == null)
                {
                    return;
                }

                text = nativeClipboard.GetClipboardText();
            }
            catch (Exception)
            {
                return;
            }

            if (wParam.ToInt64() != wparamcut)
            {
                var clipboardCopyEvent = new ClipboardCopyEvent
                    { ClipboardText = text, IssuingModule = ClipboardModule.Identifier };
                Enqueue(clipboardCopyEvent);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Sets the hook for the clipboard copy event.
        /// </summary>
        public void StartCapture(IClipboardWindowMessageSink windowMessageSink, INativeClipboard nativeClip)
        {
            clipboardWindowMessageSink = windowMessageSink;
            nativeClipboard = nativeClip;

            if (clipboardWindowMessageSink == null)
            {
                return;
            }

            clipboardWindowMessageSink.ClipboardUpdated += OnClipboardUpdate;
        }

        /// <summary>
        ///     Releases the hook for the clipboard copy event.
        /// </summary>
        public void StopCapture()
        {
            clipboardWindowMessageSink?.Dispose();
            Close();
        }

        #endregion
    }
}