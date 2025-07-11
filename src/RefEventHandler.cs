﻿// SPDX-License-Identifier: MIT
// Copyright (c) 2025 sibber (GitHub: sibber5)

namespace Sibber.WindowMessageMonitor;

#pragma warning disable CA1003 // Use generic event handler instances
public delegate void RefEventHandler<TEventArgs>(object sender, ref TEventArgs e) where TEventArgs : struct;
