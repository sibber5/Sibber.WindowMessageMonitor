using System.Runtime.InteropServices;

namespace sibber.WindowMessageMonitor.Native.Windowing;

[UnmanagedFunctionPointer(CallingConvention.StdCall)]
internal unsafe delegate nint WndProc(HWnd hWnd, uint uMsg, nuint wParam, nint lParam);
