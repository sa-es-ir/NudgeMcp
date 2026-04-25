using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NudgeMcp.Models;

namespace NudgeMcp.Services;

public sealed class ReminderDispatchWorker(
    IReminderStore reminderStore,
    INotificationService notificationService,
    ILogger<ReminderDispatchWorker> logger) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(PollInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            var dueReminders = reminderStore.DequeueDue(DateTimeOffset.UtcNow);

            foreach (var reminder in dueReminders)
            {
                await DispatchReminderAsync(reminder, stoppingToken);
            }
        }
    }

    private async Task DispatchReminderAsync(Reminder reminder, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Dispatching reminder {ReminderId} scheduled for {ScheduledForUtc}.",
            reminder.Id,
            reminder.ScheduledForUtc);

        await notificationService.ShowReminderAsync(reminder, cancellationToken);
    }
}
