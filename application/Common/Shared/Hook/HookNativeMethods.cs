﻿using System;
using System.Runtime.InteropServices;

namespace MORR.Shared.Hook
{
    internal class HookNativeMethods: IHookNativeMethods
    {
        private const string hookLibName = @"HookLibrary64";

        #region Interface Methods

        string IHookNativeMethods.HookLibraryName=> hookLibName;

        bool IHookNativeMethods.FreeLibrary(IntPtr hModule) => FreeLibrary(hModule);

        void IHookNativeMethods.SetHook(GlobalHook.CppGetMessageCallback callbackPointer, bool blocking) => SetHook(callbackPointer, blocking);

        IntPtr IHookNativeMethods.LoadLibrary() => LoadLibrary(hookLibName);

        bool IHookNativeMethods.Capture(uint type) => Capture(type);

        void IHookNativeMethods.StopCapture(uint type) => StopCapture(type);

        void IHookNativeMethods.RemoveHook() => RemoveHook();

        #endregion

        #region Static Imports
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport(hookLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void SetHook([MarshalAs(UnmanagedType.FunctionPtr)] GlobalHook.CppGetMessageCallback callbackPointer, [MarshalAs(UnmanagedType.Bool)] bool blocking);

        [DllImport(hookLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void RemoveHook();

        [DllImport(hookLibName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        private static extern bool Capture([MarshalAs(UnmanagedType.U4)] uint type);

        [DllImport(hookLibName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void StopCapture([MarshalAs(UnmanagedType.U4)] uint type);

        #endregion
    }
}
