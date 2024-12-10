namespace AccessibleColors.UI;

/// <summary>
/// Represents a saved color palette configuration.
/// </summary>
public class Palette
{
    /// <summary>
    /// Gets or sets the alpha transparency value (0-255).
    /// </summary>
    public int Alpha { get; set; }

    /// <summary>
    /// Gets or sets the background color in #RRGGBB format.
    /// </summary>
    public string BackgroundColor { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the foreground color in #RRGGBB format.
    /// </summary>
    public string ForegroundColor { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the text is bold.
    /// </summary>
    public bool IsBold { get; set; }

    /// <summary>
    /// Gets or sets the sample text displayed in the preview.
    /// </summary>
    public string SampleText { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text size in points.
    /// </summary>
    public double TextSizePt { get; set; }
}