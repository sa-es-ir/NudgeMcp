using NudgeMcp.Models;

namespace NudgeMcp.Services;

public interface IReminderStore
{
    Reminder Schedule(string message, TimeSpan delay);

    IReadOnlyList<Reminder> GetPending();

    bool TryCancel(string reminderId, out Reminder? reminder);

    IReadOnlyList<Reminder> DequeueDue(DateTimeOffset utcNow);
}
