// SPDX-License-Identifier: MIT
// Copyright (c) 2025 sibber (GitHub: sibber5)

using System;
using System.Runtime.InteropServices;
using Sibber.Common.Native.Windows.Windowing;

namespace Sibber.WindowMessageMonitor.Native.Windowing;

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate IntPtr WndProc(HWnd hWnd, uint uMsg, UIntPtr wParam, IntPtr lParam);
