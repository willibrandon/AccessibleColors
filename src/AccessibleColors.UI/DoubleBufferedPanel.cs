namespace AccessibleColors.UI;

/// <summary>
/// A panel control that enables double buffering to reduce flicker and improve rendering performance.
/// </summary>
public class DoubleBufferedPanel : Panel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleBufferedPanel"/> class.
    /// This constructor sets the necessary control styles to allow double buffered painting,
    /// resulting in smoother visuals and reduced flickering.
    /// </summary>
    public DoubleBufferedPanel()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);

        UpdateStyles();
    }
}