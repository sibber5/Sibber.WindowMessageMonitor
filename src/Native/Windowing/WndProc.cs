using System;
using System.Runtime.InteropServices;
using Sibber.Common.Native.Windows.Windowing;

namespace Sibber.WindowMessageMonitor.Native.Windowing;

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate IntPtr WndProc(HWnd hWnd, uint uMsg, UIntPtr wParam, IntPtr lParam);
