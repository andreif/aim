// Program.cs
// .NET 6+ Windows Forms single-file app.
// Builds a transparent, click-through fullscreen overlay that draws a central "aim dot".
// Toggle visibility with Ctrl+Shift+X. Exit app with Escape.
// NOTE: This is a simple visual overlay only — it does NOT read or inject into games.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new AimDotOverlay());
    }
}

public class AimDotOverlay : Form
{
    // P/Invoke constants and signatures
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_LAYERED = 0x80000;
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int WS_EX_NOACTIVATE = 0x8000000;
    private const int SW_SHOW = 5;

    private const int MOD_CONTROL = 0x0002;
    private const int MOD_SHIFT = 0x0004;
    private const int HOTKEY_ID = 0xB001; // arbitrary id
    private const int VK_X = 0x58;

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll", SetLastError = true)]
    static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public AimDotOverlay()
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;

        var bounds = Screen.PrimaryScreen?.Bounds ?? Screen.AllScreens[0].Bounds;
        Bounds = bounds;

        // BackColor used as the transparent key
        BackColor = Color.Lime;
        TransparencyKey = Color.Lime;

        DoubleBuffered = true;
        // Make the window non-activating and click-through after shown
        Load += (s, e) => {
            var ex = GetWindowLong(Handle, GWL_EXSTYLE);
            ex |= WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_NOACTIVATE;
            SetWindowLong(Handle, GWL_EXSTYLE, ex);

            // Register Ctrl+Shift+X as toggle hotkey
            RegisterHotKey(Handle, HOTKEY_ID, MOD_CONTROL | MOD_SHIFT, VK_X);
        };

        FormClosing += (s, e) => {
            UnregisterHotKey(Handle, HOTKEY_ID);
        };

        // Minimal key handling in case user focuses the window accidentally
        KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        };

        // Make sure we can receive key messages even if not focused
        SetStyle(ControlStyles.Selectable, false);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        int cx = Width / 2;
        int cy = Height / 2;

        e.Graphics.FillRectangle(Brushes.White, cx, cy + 1, 1, 1);
        e.Graphics.FillRectangle(Brushes.Red, cx, cy, 1, 1);
    }

    // Listen for the registered hotkey message
    protected override void WndProc(ref Message m)
    {
        const int WM_HOTKEY = 0x0312;
        if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
        {
            // Toggle visibility of the overlay when hotkey pressed
            Visible = !Visible;
            // If becoming visible, force a repaint
            if (Visible) Invalidate();
            return;
        }

        base.WndProc(ref m);
    }

    // Ensure the overlay does not steal focus when shown programmatically
    protected override CreateParams CreateParams
    {
        get
        {
            var cp = base.CreateParams;
            // WS_EX_NOACTIVATE flag would normally be used here but we already set it via SetWindowLong on load.
            return cp;
        }
    }
}
