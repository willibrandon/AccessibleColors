namespace AccessibleColors.UI;

/// <summary>
/// A label control that enables double buffering to reduce flicker and improve rendering performance.
/// </summary>
public class DoubleBufferedLabel : Label
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleBufferedLabel"/> class.
    /// This constructor sets the necessary control styles to allow double buffered painting,
    /// resulting in smoother visuals and reduced flickering.
    /// </summary>
    public DoubleBufferedLabel()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer,
            true);

        UpdateStyles();
    }
}
