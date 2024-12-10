namespace AccessibleColors.UI;

/// <summary>
/// A full-screen transparent overlay that covers a single monitor, allowing the user
/// to select a color via the eyedropper functionality. This overlay displays instructions
/// and captures keyboard input (ENTER or ESC) to coordinate actions with the main
/// <see cref="EyedropperOverlay"/>.
/// </summary>
public class MonitorOverlay : Form
{
    private readonly EyedropperOverlay controller;
    private readonly Label lblInstructions;

    /// <summary>
    /// Initializes a new instance of the <see cref="MonitorOverlay"/> class.
    /// </summary>
    /// <param name="controller">The main <see cref="EyedropperOverlay"/> controller instance.</param>
    /// <param name="screenBounds">The bounds of the monitor this overlay should cover.</param>
    public MonitorOverlay(EyedropperOverlay controller, Rectangle screenBounds)
    {
        this.controller = controller;

        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        Location = new Point(screenBounds.X, screenBounds.Y);
        Size = new Size(screenBounds.Width, screenBounds.Height);
        ShowInTaskbar = false;
        TopMost = true;
        KeyPreview = true;
        Cursor = Cursors.Cross;

        BackColor = Color.Black;
        Opacity = 0.3;

        lblInstructions = new Label
        {
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = true,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            Text = "Press ENTER to select the color under the cursor.\nPress ESC to cancel.",
            TextAlign = ContentAlignment.MiddleCenter
        };

        Controls.Add(lblInstructions);
    }

    /// <summary>
    /// Called when the overlay is first shown. Centers the instructions and activates the overlay.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        // Center instructions within the overlay
        lblInstructions.Left = (ClientSize.Width - lblInstructions.Width) / 2;
        lblInstructions.Top = (ClientSize.Height - lblInstructions.Height) / 2;
        Activate();
    }

    /// <summary>
    /// Handles key presses. Pressing ENTER selects the color under the cursor, pressing ESC cancels.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Enter)
        {
            var pos = Cursor.Position;
            controller.OnColorSelected(pos);
        }
        else if (e.KeyCode == Keys.Escape)
        {
            controller.OnCancelled();
        }
    }
}
