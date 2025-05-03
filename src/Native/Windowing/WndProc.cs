using System.Runtime.InteropServices;
using sibber.Common.Native.Windows.Windowing;

namespace sibber.WindowMessageMonitor.Native.Windowing;

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate nint WndProc(HWnd hWnd, uint uMsg, nuint wParam, nint lParam);
