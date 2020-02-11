﻿using System.Drawing;
using MORR.Modules.WindowManagement.Events;
using MORR.Modules.WindowManagement.Native;
using MORR.Shared.Events.Queue;
using MORR.Shared.Hook;
using Point = System.Windows.Point;

namespace MORR.Modules.WindowManagement.Producers
{
    /// <summary>
    ///     Provides a single-writer-multiple-reader queue for WindowMovementEvent
    /// </summary>
    public class WindowMovementEventProducer : DefaultEventQueue<WindowMovementEvent>
    {
        /// <summary>
        ///     The rectangle that holds the size and location of the window
        ///     after the change.
        /// </summary>
        private Rectangle windowRecAfterChange;

        /// <summary>
        ///     The rectangle that holds the size and location of the window
        ///     before the change.
        /// </summary>
        private Rectangle windowRecBeforeChange;

        /// <summary>
        ///     The hwnd of the windows being changed.
        ///     Change can mean move or resize.
        /// </summary>
        private int windowUnderChangeHwnd;

        private readonly GlobalHook.MessageType[] listenedMessageTypes =
            { GlobalHook.MessageType.WM_ENTERSIZEMOVE, GlobalHook.MessageType.WM_EXITSIZEMOVE };

        public void StartCapture()
        {
            GlobalHook.AddListener(WindowHookCallback, listenedMessageTypes);
            GlobalHook.IsActive = true;
        }

        public void StopCapture()
        {
            GlobalHook.RemoveListener(WindowHookCallback, listenedMessageTypes);
            Close();
        }

        /// <summary>
        ///     Everytime a WM_ENTERSIZEMOVE is received,
        ///     records the information of the window before the change
        ///     and wait for the WM_EXITSIZEMOVE.
        ///     Everytime a WM_EXITSIZEMOVE is received,
        ///     records the information of the window after the change
        ///     and see if the windows has been moved
        ///     (by if (Utility.IsRectSizeEqual(windowRecBeforeChange,windowRecAfterChange)))
        ///     If so, records the relevant information into a WindowMovementEvent.
        /// </summary>
        /// <param name="msg">the hook message</param>
        private void WindowHookCallback(GlobalHook.HookMessage msg)
        {
            if (msg.Type == (uint) GlobalHook.MessageType.WM_ENTERSIZEMOVE)
            {
                windowUnderChangeHwnd = (int) msg.Hwnd;
                windowRecBeforeChange = new Rectangle();
                WindowManagementNativeMethods.GetWindowRect(windowUnderChangeHwnd, ref windowRecBeforeChange);
            }

            if (msg.Type == (uint) GlobalHook.MessageType.WM_EXITSIZEMOVE)
            {
                windowRecAfterChange = new Rectangle();
                WindowManagementNativeMethods.GetWindowRect(windowUnderChangeHwnd, ref windowRecAfterChange);
                if (WindowManagementNativeMethods.IsRectSizeEqual(windowRecBeforeChange, windowRecAfterChange))
                {
                    var oldLocation = new Point { X = windowRecBeforeChange.X, Y = windowRecBeforeChange.Y };
                    var newLocation = new Point { X = windowRecAfterChange.X, Y = windowRecAfterChange.Y };
                    var @event = new WindowMovementEvent
                    {
                        IssuingModule = WindowManagementModule.Identifier,
                        OldLocation = oldLocation,
                        NewLocation = newLocation,
                        Title = WindowManagementNativeMethods.GetWindowTitleFromHwnd(msg.Hwnd),
                        ProcessName = WindowManagementNativeMethods.GetProcessNameFromHwnd(msg.Hwnd)
                    };
                    Enqueue(@event);
                }
            }
        }
    }
}