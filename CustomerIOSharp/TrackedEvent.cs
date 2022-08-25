namespace CustomerIOSharp;

using System;

internal class TrackedEvent
{
    public string Name { get; set; }
    
    public object Data { get; set; }

    public DateTime? Timestamp { get; set; }
}