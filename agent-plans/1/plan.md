# Plan for Issue #1 — Add Troubleshooting section to README

## Goal

Append a **Troubleshooting** section to the end of `README.md` (after the existing "Notes" section). No code changes — docs only.

## What to add

Add a horizontal rule (`---`) followed by a `## Troubleshooting` heading, then two subsections:

### 1. Notifications not appearing

Explain that Windows **Focus Assist** (called "Do not disturb" on Windows 11) can silently suppress toast notifications. Tell the user to:

- Open **Settings → System → Focus Assist** (Win 10) or **Settings → System → Notifications → Do not disturb** (Win 11) and turn it off, or add an exception.
- Also check **Settings → System → Notifications** and make sure notifications are enabled globally and for the app.

### 2. MCP client cannot find the executable

Explain that the most common cause is an incorrect path in the client config. Tell the user to:

- Verify the path in `claude_desktop_config.json` points to the actual `NudgeMcp.exe` produced by `dotnet publish`.
- Use double backslashes (`\\`) in JSON strings.
- Make sure they ran `dotnet publish -c Release -o publish` and the file exists at the specified location.

## Where to edit

File: `README.md` (project root, currently 101 lines).  
Insert after line 100 (the last line of the "Notes" section). Keep the existing trailing newline.

## Style

Match the existing README style: `##` for the section heading, short paragraphs, backticks for paths/commands, and a leading `---` separator consistent with the other sections.
