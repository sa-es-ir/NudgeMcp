using ModelContextProtocol.Server;
using System.ComponentModel;
using NudgeMcp.Models;
using NudgeMcp.Services;

namespace NudgeMcp.Tools;

[McpServerToolType]
public static class ReminderTools
{
    [McpServerTool, Description("Schedule a Windows reminder notification after a delay in minutes. Example: if the user says \"tell me to stretch 10 minutes later\", use message=\"stretch\" and delayMinutes=10.")]
    public static ScheduleReminderResult ScheduleReminder(
        IReminderStore reminderStore,
        [Description("The message to show in the Windows notification.")] string message,
        [Description("How many minutes to wait before the reminder fires. Fractional values are allowed for testing.")] double delayMinutes)
    {
        if (delayMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(delayMinutes), "Delay must be greater than zero minutes.");
        }

        var reminder = reminderStore.Schedule(message, TimeSpan.FromMinutes(delayMinutes));

        return new ScheduleReminderResult(
            reminder.Id,
            reminder.Message,
            reminder.ScheduledForUtc,
            delayMinutes);
    }

    [McpServerTool, Description("List all pending reminders that are still waiting to fire.")]
    public static IReadOnlyList<PendingReminderResult> ListReminders(IReminderStore reminderStore)
    {
        return reminderStore.GetPending()
            .Select(reminder => new PendingReminderResult(
                reminder.Id,
                reminder.Message,
                reminder.ScheduledForUtc,
                reminder.CreatedAtUtc))
            .ToArray();
    }

    [McpServerTool, Description("Cancel a pending reminder by its reminder ID.")]
    public static CancelReminderResult CancelReminder(
        IReminderStore reminderStore,
        [Description("The reminder ID returned from schedule_reminder.")] string reminderId)
    {
        var cancelled = reminderStore.TryCancel(reminderId, out var reminder);

        return new CancelReminderResult(
            reminderId,
            cancelled,
            reminder?.Message);
    }

    public sealed record ScheduleReminderResult(
        string ReminderId,
        string Message,
        DateTimeOffset ScheduledForUtc,
        double DelayMinutes);

    public sealed record PendingReminderResult(
        string ReminderId,
        string Message,
        DateTimeOffset ScheduledForUtc,
        DateTimeOffset CreatedAtUtc);

    public sealed record CancelReminderResult(
        string ReminderId,
        bool Cancelled,
        string? Message);
}
