namespace CustomerIOSharp
{
    using System;

    //[SerializeAs(NameStyle = NameStyle.CamelCase)]
    public class TrackedEvent
    {
        public string Name { get; set; }
        public object Data { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}