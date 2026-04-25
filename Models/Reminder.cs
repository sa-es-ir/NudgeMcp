namespace NudgeMcp.Models;

public sealed record Reminder(
    string Id,
    string Message,
    DateTimeOffset ScheduledForUtc,
    DateTimeOffset CreatedAtUtc);
