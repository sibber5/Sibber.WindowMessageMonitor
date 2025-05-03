<div align="center">
<h1>sibber.WindowMessageMonitor</h1>
<a href="https://www.nuget.org/packages/sibber.WindowMessageMonitor">
    <img alt="NuGet Version" src="https://img.shields.io/nuget/v/sibber.WindowMessageMonitor" alt="nuget" />
</a> 
 <a href="https://github.com/sibber5/sibber.WindowMessageMonitor/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/sibber5/sibber.WindowMessageMonitor?color=lightgrey" alt="license" />
</a>
</div>

A class that lets you listen to window messages for a given window, forked from [dotMorten/WinUIEx.Messaging.WindowMessageMonitor](https://github.com/dotMorten/WinUIEx/blob/c363a6d25b586701a7996dfa8622b42a3c3b5740/src/WinUIEx/Messaging/WindowMessageMonitor.cs).

# Usage

If you have an existing window:
```cs
var monitor = new WindowMessageMonitor(windowHandle);
monitor += (object sender, ref WindowMessageEventArgs e) =>
{
    Debug.WriteLine($"Recieved message: {e.MessageType} with wParam: {e.Message.WParam} and LParam: {e.Message.LParam}");

    // set e.Handled to true to return e.Result from the window procedure
    // (keep in mind that `WindowMessageEventArgs` is a readonly struct that is passed by ref,
    // so we set the parameter `e` to a new value to modify Handled and Result)
    e = e with { Handled = true, Result = 0 };
}
```

Or you can create a [message-only window](https://learn.microsoft.com/en-us/windows/win32/winmsg/window-features#message-only-windows):
```cs
var monitor = WindowMessageMonitor.CreateWithMessageOnlyWindow();
```

Make sure to dispose when you're done using the instance:
```cs
monitor.Dispose();
```
***Important: If the monitor was created with `CreateWithMessageOnlyWindow()` then it must be disposed on the same thread it was created on and in the same executing assembly.***

The interface `IWindowMessageMonitor` is also provided in case you want to provide custom implementations, useful for things like other libraries that use this library.

# License

The file [src/WindowMessageMonitor.cs](https://github.com/sibber5/sibber.WindowMessageMonitor/blob/main/src/WindowMessageMonitor.cs) was taken from [dotMorten/WinUIEx](https://github.com/dotMorten/WinUIEx/blob/c363a6d25b586701a7996dfa8622b42a3c3b5740/src/WinUIEx/Messaging/WindowMessageMonitor.cs), MIT License - Copyright (c) 2021 Morten Nielsen. See the license notice at the top of the file for more info.

The rest of the library/repository is licensed under the MIT License - see [LICENSE](https://github.com/sibber5/sibber.WindowMessageMonitor/blob/main/LICENSE) - unless otherwise stated in specific files or sections. See individual files for exceptions.
