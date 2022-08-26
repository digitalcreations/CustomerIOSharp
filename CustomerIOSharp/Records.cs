using System;

namespace CustomerIOSharp;

internal readonly record struct TriggerBroadcast(object? Data, object? Recipients);

internal readonly record struct TrackedEvent(string Name, object? Data, DateTime? Timestamp);

public record CustomerDetails(string Id, string Email) : ICustomerDetails;