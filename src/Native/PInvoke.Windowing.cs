using System.Runtime.InteropServices;
using sibber.Common.Native.Windows;
using sibber.Common.Native.Windows.Windowing;
using sibber.WindowMessageMonitor.Native.Windowing;

namespace sibber.WindowMessageMonitor.Native;

internal static unsafe partial class PInvoke
{
    public static unsafe partial class Windowing
    {
        [DllImport("USER32.dll", EntryPoint = "DefWindowProcW", ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern nint DefWindowProc(HWnd hWnd, uint Msg, nuint wParam, nint lParam);

        [DllImport("COMCTL32.dll", ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern nint DefSubclassProc(HWnd hWnd, uint uMsg, nuint wParam, nint lParam);

#if NET7_0_OR_GREATER
        public static unsafe bool SetWindowSubclass(HWnd hWnd, delegate* unmanaged[Stdcall]<HWnd, uint, nuint, nint, nuint, nuint, nint> pfnSubclass, nuint uIdSubclass, nuint dwRefData)
            => SetWindowSubclass(hWnd.Value, pfnSubclass, uIdSubclass, dwRefData);

        [LibraryImport("COMCTL32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static unsafe partial bool SetWindowSubclass(nint hWnd, delegate* unmanaged[Stdcall]<HWnd, uint, nuint, nint, nuint, nuint, nint> pfnSubclass, nuint uIdSubclass, nuint dwRefData);

        public static unsafe bool RemoveWindowSubclass(HWnd hWnd, delegate* unmanaged[Stdcall]<HWnd, uint, nuint, nint, nuint, nuint, nint> pfnSubclass, nuint uIdSubclass)
            => RemoveWindowSubclass(hWnd.Value, pfnSubclass, uIdSubclass);

        [LibraryImport("COMCTL32.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static unsafe partial bool RemoveWindowSubclass(nint hWnd, delegate* unmanaged[Stdcall]<HWnd, uint, nuint, nint, nuint, nuint, nint> pfnSubclass, nuint uIdSubclass);
#else
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public unsafe delegate nint SUBCLASSPROC(HWnd hWnd, uint uMsg, nuint wParam, nint lParam, nuint uIdSubclass, nuint dwRefData);

        [DllImport("COMCTL32.dll", ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern unsafe bool SetWindowSubclass(HWnd hWnd, SUBCLASSPROC pfnSubclass, nuint uIdSubclass, nuint dwRefData);

        [DllImport("COMCTL32.dll", ExactSpelling = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern unsafe bool RemoveWindowSubclass(HWnd hWnd, SUBCLASSPROC pfnSubclass, nuint uIdSubclass);
#endif

        [DllImport("USER32.dll", EntryPoint = "RegisterClassW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern unsafe ushort RegisterClass(ref WNDCLASSW lpWndClass);

        [DllImport("USER32.dll", EntryPoint = "UnregisterClassW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnregisterClass([MarshalAs(UnmanagedType.LPWStr)] string lpClassName, Handle hInstance);

#if NET7_0_OR_GREATER
        public static bool DestroyWindow(HWnd hWnd) => DestroyWindow(hWnd.Value);

        [LibraryImport("USER32.dll", EntryPoint = "DestroyWindow", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool DestroyWindow(nint hWnd);
#else
        [DllImport("USER32.dll", EntryPoint = "DestroyWindow", ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyWindow(HWnd hWnd);
#endif

#if NET7_0_OR_GREATER
        public static HWnd CreateWindowEx(uint dwExStyle, string lpClassName, string? lpWindowName, uint dwStyle, int X, int Y, int nWidth, int nHeight, HWnd hWndParent, Handle hMenu, Handle hInstance, [Optional] void* lpParam)
            => (HWnd)CreateWindowEx(dwExStyle, lpClassName, lpWindowName, dwStyle, X, Y, nWidth, nHeight, hWndParent.Value, hMenu.Value, hInstance.Value, lpParam);

        [LibraryImport("USER32.dll", EntryPoint = "CreateWindowExW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        private static partial nint CreateWindowEx(uint dwExStyle, string lpClassName, string? lpWindowName, uint dwStyle, int X, int Y, int nWidth, int nHeight, nint hWndParent, nint hMenu, nint hInstance, [Optional] void* lpParam);
#else
        [DllImport("USER32.dll", EntryPoint = "CreateWindowExW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern HWnd CreateWindowEx(uint dwExStyle, string lpClassName, string? lpWindowName, uint dwStyle, int X, int Y, int nWidth, int nHeight, HWnd hWndParent, Handle hMenu, Handle hInstance, [Optional] void* lpParam);
#endif
    }
}
