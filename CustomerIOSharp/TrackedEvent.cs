namespace CustomerIOSharp;

using System;

internal readonly record struct TrackedEvent(string Name, object Data, DateTime? Timestamp);