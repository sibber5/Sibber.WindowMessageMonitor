// SPDX-License-Identifier: MIT
// Copyright (c) 2025 sibber (GitHub: sibber5)

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Sibber.Common.Native.Windows;
using Sibber.Common.Native.Windows.Windowing;

namespace Sibber.WindowMessageMonitor.Native;

internal static partial class Helpers
{
    internal static class Windowing
    {
        public static unsafe HWnd CreateMessageOnlyWindow(string className, string? windowName, Handle instance)
        {
            HWnd HWND_MESSAGE = unchecked((HWnd)(IntPtr)(-3));
            return PInvoke.Windowing.CreateWindowEx(0, className, windowName, 0, 0, 0, 0, 0, HWND_MESSAGE, Handle.Null, instance);
        }

        /// <returns>The HINSTANCE for the module that contains the type <see cref="WindowMessageMonitor"/>.</returns>
#if NETSTANDARD
        /// <exception cref="Win32Exception">Thrown if the HINSTANCE could not be retrieved.</exception>
#else
        /// <exception cref="InvalidOperationException">Thrown if the module does not have an HINSTANCE.</exception>
#endif
        public static unsafe Handle GetCurrentHINSTANCE()
        {
#if NETSTANDARD
            const uint GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT = 0x00000002u;

            string modulePath = typeof(WindowMessageMonitor).Assembly.Location;
            IntPtr hModule = IntPtr.Zero;
            bool success = PInvoke.Misc.GetModuleHandleEx(GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT, modulePath, &hModule);

            if (!success) throw new Win32Exception();

            return (Handle)hModule;
#else
            var hInstance = (Handle)Marshal.GetHINSTANCE(typeof(WindowMessageMonitor).Module);
            if (hInstance == -1) throw new InvalidOperationException("Module does not have an HINSTANCE.");
            return hInstance;
#endif
        }
    }
}
