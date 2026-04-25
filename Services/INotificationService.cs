using NudgeMcp.Models;

namespace NudgeMcp.Services;

public interface INotificationService
{
    Task ShowReminderAsync(Reminder reminder, CancellationToken cancellationToken);
}
