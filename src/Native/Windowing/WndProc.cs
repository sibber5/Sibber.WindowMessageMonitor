using System.Runtime.InteropServices;
using Sibber.Common.Native.Windows.Windowing;

namespace Sibber.WindowMessageMonitor.Native.Windowing;

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate nint WndProc(HWnd hWnd, uint uMsg, nuint wParam, nint lParam);
