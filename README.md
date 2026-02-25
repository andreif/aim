# Aim

Transparent, click-through crosshair overlay for Windows. Draws a red dot with a black outline at the center of the primary screen. Does not read or inject into any process.

## Controls

| Key | Action |
|---|---|
| `Ctrl+Shift+X` | Toggle overlay visibility |
| `Escape` | Exit (when focused) |

## Customization

Edit the fields in `aim.cs` to change dot size, color, and outline:

```csharp
private readonly int dotRadius = 6;
private readonly Color dotColor = Color.Red;
private readonly bool drawOutline = true;
private readonly int outlineThickness = 2;
private readonly Color outlineColor = Color.Black;
```

## Building

Requires .NET 10 SDK on Windows (uses Windows Forms).

```
dotnet publish -r win-x64 -c Release --self-contained true -p:PublishSingleFile=true -p:PublishReadyToRun=true
```

Builds are also produced automatically via GitHub Actions on push to `main`. Download the artifact from the Actions tab.
