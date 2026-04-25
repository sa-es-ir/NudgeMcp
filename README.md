# NudgeMcp

A Windows MCP (Model Context Protocol) server that lets AI assistants schedule, list, and cancel Windows toast notifications on your behalf.

## Prerequisites

- Windows 10 (build 17763) or later
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- An MCP-compatible client (e.g. [Claude Desktop](https://claude.ai/download))

---

## Step 1 — Clone and build

```bash
git clone https://github.com/sa-es-ir/NudgeMcp.git
cd NudgeMcp
dotnet publish -c Release -o publish
```

The self-contained executable will be at `publish\NudgeMcp.exe`.

---

## Step 2 — Configure your MCP client

### Claude Desktop

Open (or create) the Claude Desktop configuration file:

```
%APPDATA%\Claude\claude_desktop_config.json
```

Add NudgeMcp to the `mcpServers` section:

```json
{
  "mcpServers": {
    "nudgemcp": {
      "command": "C:\\path\\to\\publish\\NudgeMcp.exe"
    }
  }
}
```

Replace `C:\\path\\to\\publish\\NudgeMcp.exe` with the actual path where you published the executable.

**Restart Claude Desktop** after saving the file.

### Other MCP clients

NudgeMcp uses the standard stdio transport, so any MCP-compatible client works. Point the client at the `NudgeMcp.exe` executable as the server command with no additional arguments.

---

## Step 3 — Verify the connection

After restarting your client, open a new conversation. The client should report NudgeMcp as a connected tool server. You can verify by asking:

> "List my pending reminders."

You should get back an empty list (no error), confirming the server is running.

---

## Step 4 — Use it

Just talk to your AI assistant naturally. Examples:

| What you say | What happens |
|---|---|
| "Remind me to drink water in 30 minutes." | Schedules a Windows notification for 30 minutes from now. |
| "Tell me to review the PR in 2 hours." | Schedules a notification in 120 minutes. |
| "What reminders do I have?" | Lists all pending reminders with their scheduled times. |
| "Cancel my water reminder." | Cancels the reminder by ID. |

When a reminder fires, a Windows toast notification pops up in the bottom-right corner of your screen.

---

## Available MCP tools

| Tool | Description |
|---|---|
| `schedule_reminder` | Schedule a notification after a delay in minutes. Fractional values are allowed (e.g. `0.5` for 30 seconds). |
| `list_reminders` | List all pending reminders that have not fired yet. |
| `cancel_reminder` | Cancel a pending reminder by its ID. |

---

## Notes

- Reminders are stored **in memory**. If the server process restarts, all pending reminders are lost.
- The server process runs in the background as long as your MCP client is open. It polls every second to check for due reminders.
- Notifications appear as standard Windows toast notifications and expire after 24 hours if not dismissed.
