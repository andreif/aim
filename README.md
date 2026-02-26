# Aim

Transparent, click-through crosshair overlay for Windows. Draws a red/white dot at the center of the primary screen with a drop scale below. Does not read or inject into any process.

## Controls

| Key | Action |
|---|---|
| `Ctrl+Shift+X` | Toggle overlay visibility |
| `Escape` | Exit (when focused) |

## Building

Requires .NET 10 SDK on Windows (uses Windows Forms).

```
dotnet publish -r win-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true
```

Builds are also produced automatically via GitHub Actions. Tag a version (e.g. `git tag v1.0.0 && git push origin v1.0.0`) to create a release with the `.exe` attached.
