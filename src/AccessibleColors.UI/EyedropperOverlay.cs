using System.Runtime.InteropServices;
using System.ComponentModel;

namespace AccessibleColors.UI;

/// <summary>
/// The <see cref="EyedropperOverlay"/> class controls a set of transparent overlays spanning all available monitors.
/// These overlays allow the user to pick a color from any part of the desktop by pressing ENTER, or cancel by pressing ESC.
/// </summary>
public partial class EyedropperOverlay : Form
{
    /// <summary>
    /// Gets the color selected by the user.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Color SelectedColor { get; private set; } = Color.Black;

    // Native interop methods for capturing a pixel from the desktop
    [LibraryImport("user32.dll", EntryPoint = "GetDesktopWindow")]
    internal static partial IntPtr GetDesktopWindow();

    [LibraryImport("user32.dll", EntryPoint = "GetWindowDC")]
    internal static partial IntPtr GetWindowDC(IntPtr hWnd);

    [LibraryImport("gdi32.dll", EntryPoint = "GetPixel")]
    internal static partial uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

    [LibraryImport("user32.dll", EntryPoint = "ReleaseDC")]
    internal static partial int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    private readonly List<MonitorOverlay> overlays = [];
    private bool actionTaken = false; // To prevent multiple triggers of OK/Cancel

    /// <summary>
    /// Initializes a new instance of the <see cref="EyedropperOverlay"/> class.
    /// This main form acts as a controller and will not be visible.
    /// </summary>
    public EyedropperOverlay()
    {
        InitializeComponent();

        // Configure the controller form to be invisible and non-interactive
        ShowInTaskbar = false;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Opacity = 0; // Completely invisible
        Size = new Size(1, 1);
        TopMost = true;
    }

    /// <summary>
    /// Raises the <see cref="Form.Load"/> event. Creates and shows a <see cref="MonitorOverlay"/> per available screen.
    /// </summary>
    /// <param name="e">Event data.</param>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Create and display one overlay per screen
        foreach (var screen in Screen.AllScreens)
        {
            var overlay = new MonitorOverlay(this, screen.Bounds);
            overlays.Add(overlay);
            overlay.Show();
        }
    }

    /// <summary>
    /// Called by a <see cref="MonitorOverlay"/> when the user presses ENTER to select the color under the cursor.
    /// </summary>
    /// <param name="cursorPos">The position of the cursor at the time of selection.</param>
    public void OnColorSelected(Point cursorPos)
    {
        if (actionTaken)
            return;
        actionTaken = true;

        // Hide all overlays before capturing the pixel color
        foreach (var ov in overlays)
            ov.Visible = false;
        Application.DoEvents();

        SelectedColor = GetColorAtCursor(cursorPos.X, cursorPos.Y);
        CloseAllOverlays(DialogResult.OK);
    }

    /// <summary>
    /// Called by a <see cref="MonitorOverlay"/> when the user presses ESC to cancel the color selection.
    /// </summary>
    public void OnCancelled()
    {
        if (actionTaken)
            return;
        actionTaken = true;

        CloseAllOverlays(DialogResult.Cancel);
    }

    /// <summary>
    /// Closes all overlays and sets the <see cref="DialogResult"/> of this controller form.
    /// </summary>
    /// <param name="result">The dialog result to return.</param>
    private void CloseAllOverlays(DialogResult result)
    {
        foreach (var ov in overlays)
        {
            if (!ov.IsDisposed)
                ov.Close();
        }
        DialogResult = result;
        Close();
    }

    /// <summary>
    /// Captures the color of the desktop pixel at the specified coordinates.
    /// </summary>
    /// <param name="x">The X coordinate of the pixel.</param>
    /// <param name="y">The Y coordinate of the pixel.</param>
    /// <returns>The color at the specified pixel.</returns>
    private static Color GetColorAtCursor(int x, int y)
    {
        IntPtr desktopWnd = GetDesktopWindow();
        IntPtr hdc = GetWindowDC(desktopWnd);
        uint pixel = GetPixel(hdc, x, y);
        _ = ReleaseDC(desktopWnd, hdc);

        int r = (int)(pixel & 0x000000FF);
        int g = (int)((pixel & 0x0000FF00) >> 8);
        int b = (int)((pixel & 0x00FF0000) >> 16);
        return Color.FromArgb(r, g, b);
    }
}