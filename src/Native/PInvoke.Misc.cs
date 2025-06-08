// SPDX-License-Identifier: MIT
// Copyright (c) 2025 sibber (GitHub: sibber5)

using System;
using System.Runtime.InteropServices;

namespace Sibber.WindowMessageMonitor.Native;

internal static unsafe partial class PInvoke
{
    public static unsafe partial class Misc
    {
#if NETSTANDARD
        [DllImport("KERNEL32.dll", EntryPoint = "GetModuleHandleExW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static unsafe extern bool GetModuleHandleEx(uint dwFlags, [Optional][MarshalAs(UnmanagedType.LPWStr)] string lpModuleName, IntPtr* phModule);
#endif
    }
}
