namespace sibber.WindowMessageMonitor;

public delegate void RefEventHandler<TEventArgs>(object sender, ref TEventArgs e) where TEventArgs : struct;
