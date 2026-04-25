using Microsoft.Toolkit.Uwp.Notifications;
using NudgeMcp.Models;

namespace NudgeMcp.Services;

public sealed class WindowsNotificationService : INotificationService
{
    public Task ShowReminderAsync(Reminder reminder, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        new ToastContentBuilder()
            .AddText("TaskScheduler reminder!")
            .AddText(reminder.Message)
            .Show(toast =>
            {
                toast.Tag = reminder.Id;
                toast.Group = "reminders";
                toast.ExpirationTime = reminder.ScheduledForUtc.LocalDateTime.AddDays(1);
            });

        return Task.CompletedTask;
    }
}
