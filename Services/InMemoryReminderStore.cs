using NudgeMcp.Models;

namespace NudgeMcp.Services;

public sealed class InMemoryReminderStore : IReminderStore
{
    private readonly Lock _gate = new();
    private readonly List<Reminder> _reminders = [];

    public Reminder Schedule(string message, TimeSpan delay)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        if (delay <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay), "Delay must be greater than zero.");
        }

        var now = DateTimeOffset.UtcNow;
        var reminder = new Reminder(
            Guid.NewGuid().ToString("N"),
            message.Trim(),
            now.Add(delay),
            now);

        lock (_gate)
        {
            _reminders.Add(reminder);
        }

        return reminder;
    }

    public IReadOnlyList<Reminder> GetPending()
    {
        lock (_gate)
        {
            return _reminders
                .OrderBy(reminder => reminder.ScheduledForUtc)
                .ThenBy(reminder => reminder.CreatedAtUtc)
                .ToArray();
        }
    }

    public bool TryCancel(string reminderId, out Reminder? reminder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reminderId);

        lock (_gate)
        {
            var index = _reminders.FindIndex(reminder => reminder.Id.Equals(reminderId, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                reminder = null;
                return false;
            }

            reminder = _reminders[index];
            _reminders.RemoveAt(index);
            return true;
        }
    }

    public IReadOnlyList<Reminder> DequeueDue(DateTimeOffset utcNow)
    {
        lock (_gate)
        {
            var dueReminders = _reminders
                .Where(reminder => reminder.ScheduledForUtc <= utcNow)
                .OrderBy(reminder => reminder.ScheduledForUtc)
                .ThenBy(reminder => reminder.CreatedAtUtc)
                .ToArray();

            if (dueReminders.Length == 0)
            {
                return dueReminders;
            }

            var dueIds = dueReminders
                .Select(reminder => reminder.Id)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            _reminders.RemoveAll(reminder => dueIds.Contains(reminder.Id));

            return dueReminders;
        }
    }
}
