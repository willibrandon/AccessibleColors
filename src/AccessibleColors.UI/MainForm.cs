using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.Json;
using System.Windows.Forms;

namespace AccessibleColors.UI;

/// <summary>
/// The main application form that allows users to select foreground and background colors,
/// adjust alpha transparency, set text properties, and view contrast compliance with WCAG guidelines.
/// </summary>
public partial class MainForm : Form
{
    private bool pickingForeground = false;
    private bool pickingBackground = false;
    private readonly ToolTip toolTip;

    private bool isAdjustingForeground = false;
    private Color originalForegroundColor;

    private bool isAdjustingBackground = false;
    private Color originalBackgroundColor;

    private readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

    // Store original form size and control positions
    private Size originalFormSize;
    private readonly Dictionary<Control, (Point originalLocation, Size originalSize)> originalMetrics = [];

    // Introduce resource manager for localization
    private readonly ResourceManager resourceManager;
    private readonly CultureInfo currentCulture = CultureInfo.CurrentUICulture;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    public MainForm()
    {
        InitializeComponent();
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        toolTip = new ToolTip();

        // Initialize resource manager for localization
        resourceManager = new ResourceManager("AccessibleColors.UI.Properties.Resources",
            Assembly.GetExecutingAssembly());

        Load += MainForm_Load;
        Resize += MainForm_Resize;
        ResizeEnd += MainForm_ResizeEnd;

        // Paint event for true alpha blending in the preview
        pnlPreview.Paint += PnlPreview_Paint;

        // We don't need to paint lblPreviewText directly since we handle it in pnlPreview
        lblPreviewText.Visible = false;

        ApplyLocalization(currentCulture);

        // Enable drag-and-drop functionality
        AllowDrop = true;
        DragEnter += MainForm_DragEnter;
        DragDrop += MainForm_DragDrop;
    }

    // <summary>
    /// Applies localization based on the provided culture.
    /// </summary>
    /// <param name="culture">The culture to apply.</param>
    private void ApplyLocalization(CultureInfo culture)
    {
        // Update UI text based on resource strings.
        Text = resourceManager.GetString("MainForm_Title", culture) ?? "Accessible Colors Tool";
        lblHeading.Text = resourceManager.GetString("Heading_Text", culture) ?? "AccessibleColors: WCAG-Compliant Contrast Colors";
        lblExplanationHeading.Text = resourceManager.GetString("Explanation_Heading", culture) ?? "Explanation";
        txtExplanation.Text = resourceManager.GetString("Explanation_Body", culture) ??
            "Enter foreground and background colors and adjust settings.";

        lblForegroundColor.Text = resourceManager.GetString("ForegroundColor_Label", culture) ?? "Color (#RRGGBB):";
        lblBackgroundColor.Text = resourceManager.GetString("BackgroundColor_Label", culture) ?? "Color (#RRGGBB):";

        lblForegroundAlpha.Text = resourceManager.GetString("Alpha_Label", culture) ?? "Alpha (0-255):";
        lblBackgroundAlpha.Text = resourceManager.GetString("Alpha_Label", culture) ?? "Alpha (0-255):";

        lblForegroundLightness.Text = resourceManager.GetString("Lightness_Label", culture) ?? "Lightness:";
        lblBackgroundLightness.Text = resourceManager.GetString("Lightness_Label", culture) ?? "Lightness:";

        lblTextSize.Text = resourceManager.GetString("TextSize_Label", culture) ?? "Text Size (pt):";
        lblSampleText.Text = resourceManager.GetString("SampleText_Label", culture) ?? "Sample Text:";
        chkBold.Text = resourceManager.GetString("Bold_Checkbox", culture) ?? "Bold";

        btnForegroundPicker.Text = resourceManager.GetString("ColorPicker_Button", culture) ?? "Color Picker";
        btnForegroundEyedropper.Text = resourceManager.GetString("EyeDropper_Button", culture) ?? "Eye Dropper";
        btnBackgroundPicker.Text = resourceManager.GetString("ColorPicker_Button", culture) ?? "Color Picker";
        btnBackgroundEyedropper.Text = resourceManager.GetString("EyeDropper_Button", culture) ?? "Eye Dropper";

        btnSavePalette.Text = resourceManager.GetString("SavePalette_Button", culture) ?? "Save Palette";
        btnLoadPalette.Text = resourceManager.GetString("LoadPalette_Button", culture) ?? "Load Palette";

        // Update tooltips after localization
        SetupTooltips();
    }

    private void SetupTooltips()
    {
        toolTip.SetToolTip(txtForegroundColor, resourceManager.GetString("Tooltip_ForegroundColor", currentCulture) ?? "Enter the foreground color in #RRGGBB format.");
        toolTip.SetToolTip(btnForegroundPicker, resourceManager.GetString("Tooltip_ForegroundPicker", currentCulture) ?? "Open a color picker dialog to select the foreground color.");
        toolTip.SetToolTip(btnForegroundEyedropper, resourceManager.GetString("Tooltip_ForegroundEyedropper", currentCulture) ?? "Use the eyedropper to pick a color from anywhere on your screen.");
        toolTip.SetToolTip(numForegroundAlpha, resourceManager.GetString("Tooltip_Alpha", currentCulture) ?? "Adjust the transparency (0-255) of the foreground color.");
        toolTip.SetToolTip(sliderForeground, resourceManager.GetString("Tooltip_ForegroundLightness", currentCulture) ?? "Adjust the perceived lightness of the foreground color.");

        toolTip.SetToolTip(txtBackgroundColor, resourceManager.GetString("Tooltip_BackgroundColor", currentCulture) ?? "Enter the background color in #RRGGBB format.");
        toolTip.SetToolTip(btnBackgroundPicker, resourceManager.GetString("Tooltip_BackgroundPicker", currentCulture) ?? "Open a color picker dialog to select the background color.");
        toolTip.SetToolTip(btnBackgroundEyedropper, resourceManager.GetString("Tooltip_BackgroundEyedropper", currentCulture) ?? "Use the eyedropper to pick a color from anywhere on your screen.");
        toolTip.SetToolTip(sliderBackground, resourceManager.GetString("Tooltip_BackgroundLightness", currentCulture) ?? "Adjust the perceived lightness of the background color.");

        toolTip.SetToolTip(numTextSize, resourceManager.GetString("Tooltip_TextSize", currentCulture) ?? "Set the text size in points. Large text has looser contrast requirements.");
        toolTip.SetToolTip(chkBold, resourceManager.GetString("Tooltip_Bold", currentCulture) ?? "Check if the text is bold. Bold >= 14pt is considered large text.");
        toolTip.SetToolTip(txtSampleText, resourceManager.GetString("Tooltip_SampleText", currentCulture) ?? "Enter your own sample text to preview.");

        toolTip.SetToolTip(btnSavePalette, resourceManager.GetString("Tooltip_SavePalette", currentCulture) ?? "Save the current colors, text settings, and sample text as a JSON file.");
        toolTip.SetToolTip(btnLoadPalette, resourceManager.GetString("Tooltip_LoadPalette", currentCulture) ?? "Load previously saved palette settings from a JSON file.");
    }

    /// <summary>
    /// Handles the click event for the Background Eyedropper button.
    /// </summary>
    private void BtnBackgroundEyedropper_Click(object sender, EventArgs e)
    {
        pickingForeground = false;
        pickingBackground = true;
        StartEyedropper();
    }

    /// <summary>
    /// Handles the click event for the Background Picker button.
    /// Opens a color dialog to select a new background color.
    /// </summary>
    private void BtnBackgroundPicker_Click(object sender, EventArgs e)
    {
        using var cd = new ColorDialog();
        if (cd.ShowDialog() == DialogResult.OK)
        {
            isAdjustingBackground = false;
            txtBackgroundColor.Text = ColorTranslator.ToHtml(cd.Color);

            // Reset the slider to midpoint
            sliderBackground.Value = 100;

            // Update original color
            originalBackgroundColor = cd.Color;

            UpdateContrastInfo();
        }
    }

    /// <summary>
    /// Handles the click event for the Foreground Picker button.
    /// Opens a color dialog to select a new foreground color.
    /// </summary>
    private void BtnForegroundPicker_Click(object sender, EventArgs e)
    {
        using var cd = new ColorDialog();
        if (cd.ShowDialog() == DialogResult.OK)
        {
            isAdjustingForeground = false;
            txtForegroundColor.Text = ColorTranslator.ToHtml(cd.Color);

            // Reset the slider to midpoint
            sliderForeground.Value = 100;

            // Update original color
            originalForegroundColor = cd.Color;

            UpdateContrastInfo();
        }
    }

    /// <summary>
    /// Handles the click event for the Foreground Eyedropper button.
    /// Starts the eyedropper to pick a foreground color from the screen.
    /// </summary>
    private void BtnForegroundEyedropper_Click(object sender, EventArgs e)
    {
        pickingForeground = true;
        pickingBackground = false;
        StartEyedropper();
    }

    /// <summary>
    /// Handles the click event for the Load Palette button.
    /// Loads a previously saved palette from a JSON file.
    /// </summary>
    private void BtnLoadPalette_Click(object sender, EventArgs e)
    {
        using var ofd = new OpenFileDialog
        {
            Filter = "JSON files|*.json",
            Title = resourceManager.GetString("LoadPalette_DialogTitle", currentCulture) ?? "Load Palette"
        };
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            LoadPalette(ofd.FileName);
        }
    }

    /// <summary>
    /// Handles the click event for the Save Palette button.
    /// Saves the current palette settings to a JSON file.
    /// </summary>
    private void BtnSavePalette_Click(object sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog
        {
            Filter = "JSON files|*.json",
            Title = resourceManager.GetString("SavePalette_DialogTitle", currentCulture) ?? "Save Palette"
        };
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            var palette = new
            {
                ForegroundColor = txtForegroundColor.Text,
                BackgroundColor = txtBackgroundColor.Text,
                Alpha = (int)numForegroundAlpha.Value,
                TextSizePt = (double)numTextSize.Value,
                IsBold = chkBold.Checked,
                SampleText = txtSampleText.Text
            };
            string json = JsonSerializer.Serialize(palette, jsonOptions);
            File.WriteAllText(sfd.FileName, json);
        }
    }

    /// <summary>
    /// Handles the CheckedChanged event of chkBold checkbox.
    /// Updates the preview contrast info when boldness changes.
    /// </summary>
    private void ChkBold_CheckedChanged(object sender, EventArgs e) => UpdateContrastInfo();

    /// <summary>
    /// Handles the Load event of the MainForm.
    /// Sets up tooltips, initializes original colors, updates contrast info, and records metrics.
    /// </summary>
    private void MainForm_Load(object? sender, EventArgs e)
    {
        SetupTooltips();

        // Initialize original colors
        originalForegroundColor = ColorTranslator.FromHtml(txtForegroundColor.Text);
        originalBackgroundColor = ColorTranslator.FromHtml(txtBackgroundColor.Text);

        UpdateContrastInfo();
        txtForegroundColor.Focus();

        originalFormSize = Size;
        RecordMetrics(this);
    }

    /// <summary>
    /// Handles the Resize event of the MainForm.
    /// Adjusts layout and scaling of controls for stacked or wide layout modes.
    /// </summary>
    private void MainForm_Resize(object? sender, EventArgs e)
    {
        if (originalFormSize.Width == 0 || originalFormSize.Height == 0)
            return;

        AutoScroll = false;
        HorizontalScroll.Enabled = false;
        HorizontalScroll.Visible = false;

        SuspendLayout();
        try
        {
            float scaleX = (float)Width / originalFormSize.Width;
            float scaleY = (float)Height / originalFormSize.Height;

            // Scale all controls except we haven't handled logo separately yet
            LayoutService.ScaleAllControls(
                this,
                scaleX,
                scaleY,
                originalMetrics,
                grpForeground,
                grpBackground,
                grpTextSettings);

            // Enforce a minimum size for the picLogo so it doesn't become tiny
            const int minLogoSize = 64; // Adjust as needed
            if (picLogo.Width < minLogoSize || picLogo.Height < minLogoSize)
            {
                picLogo.Width = Math.Max(picLogo.Width, minLogoSize);
                picLogo.Height = Math.Max(picLogo.Height, minLogoSize);
            }

            // Now adjust layout for either stacked or wide mode
            LayoutService.AdjustLayoutForMode(
                this, pnlContainer, picLogo, lblHeading, lblExplanationHeading, txtExplanation,
                grpForeground, grpBackground, grpTextSettings, pnlPreview,
                lblContrastRatio, lblCompliance, btnSavePalette, btnLoadPalette);

            // Finalize layout and scroll settings
            LayoutService.FinalizeLayout(this, pnlContainer, lblHeading);
        }
        finally
        {
            ResumeLayout(true);
        }
    }

    /// <summary>
    /// Handles the ResizeEnd event of the MainForm.
    /// Re-checks layout and adjusts scrollbars accordingly.
    /// </summary>
    private void MainForm_ResizeEnd(object? sender, EventArgs e)
    {
        LayoutService.AfterResizeEnd(this, pnlContainer);
    }

    /// <summary>
    /// Handles ValueChanged events for numeric up/down controls that affect contrast.
    /// </summary>
    private void NumAlpha_ValueChanged(object sender, EventArgs e) => UpdateContrastInfo();
    private void NumBackgroundAlpha_ValueChanged(object sender, EventArgs e) => UpdateContrastInfo();
    private void NumTextSize_ValueChanged(object sender, EventArgs e) => UpdateContrastInfo();

    /// <summary>
    /// Records the initial metrics (location and size) of all controls for scaling purposes.
    /// </summary>
    /// <param name="parent">The parent control to record metrics from.</param>
    private void RecordMetrics(Control parent)
    {
        foreach (Control ctrl in parent.Controls)
        {
            originalMetrics[ctrl] = (ctrl.Location, ctrl.Size);
            if (ctrl.HasChildren)
            {
                RecordMetrics(ctrl);
            }
        }
    }

    /// <summary>
    /// Handles the Scroll event for the background brightness slider.
    /// Adjusts background color brightness.
    /// </summary>
    private void SliderBackground_Scroll(object sender, EventArgs e)
    {
        isAdjustingBackground = true;
        try
        {
            float factor = sliderBackground.Value / 100f;
            Color adjustedColor = ChangeBrightness(originalBackgroundColor, factor);
            txtBackgroundColor.Text = ColorTranslator.ToHtml(adjustedColor);
            UpdateContrastInfo();
        }
        finally
        {
            isAdjustingBackground = false;
        }
    }

    /// <summary>
    /// Handles the Scroll event for the foreground brightness slider.
    /// Adjusts foreground color brightness.
    /// </summary>
    private void SliderForeground_Scroll(object sender, EventArgs e)
    {
        isAdjustingForeground = true;
        try
        {
            float factor = sliderForeground.Value / 100f;
            Color adjustedColor = ChangeBrightness(originalForegroundColor, factor);
            txtForegroundColor.Text = ColorTranslator.ToHtml(adjustedColor);
            UpdateContrastInfo();
        }
        finally
        {
            isAdjustingForeground = false;
        }
    }

    /// <summary>
    /// Handles the TextChanged event for the background color textbox.
    /// Updates the background color if valid and updates contrast.
    /// </summary>
    private void TxtBackgroundColor_TextChanged(object sender, EventArgs e)
    {
        if (!isAdjustingBackground)
        {
            try
            {
                originalBackgroundColor = ColorTranslator.FromHtml(txtBackgroundColor.Text);
                UpdateContrastInfo();
            }
            catch
            {
                // Ignore invalid input
            }
        }
    }

    /// <summary>
    /// Handles the TextChanged event for the foreground color textbox.
    /// Updates the foreground color if valid and updates contrast.
    /// </summary>
    private void TxtForegroundColor_TextChanged(object sender, EventArgs e)
    {
        if (!isAdjustingForeground)
        {
            try
            {
                originalForegroundColor = ColorTranslator.FromHtml(txtForegroundColor.Text);
                UpdateContrastInfo();
            }
            catch
            {
                // Ignore invalid input
            }
        }
    }

    /// <summary>
    /// Handles the TextChanged event for the sample text textbox.
    /// Updates the preview text and contrast info.
    /// </summary>
    private void TxtSampleText_TextChanged(object sender, EventArgs e)
    {
        lblPreviewText.Text = txtSampleText.Text;
        UpdateContrastInfo();
    }

    /// <summary>
    /// Updates the contrast ratio, compliance, and preview based on the current colors and settings.
    /// </summary>
    private void UpdateContrastInfo()
    {
        try
        {
            var baseFg = ColorTranslator.FromHtml(txtForegroundColor.Text);
            var baseBg = ColorTranslator.FromHtml(txtBackgroundColor.Text);

            int fgAlpha = (int)numForegroundAlpha.Value;
            int bgAlpha = (int)numBackgroundAlpha.Value;

            Color fgWithAlpha = Color.FromArgb(fgAlpha, baseFg.R, baseFg.G, baseFg.B);
            Color bgWithAlpha = Color.FromArgb(bgAlpha, baseBg.R, baseBg.G, baseBg.B);

            Color panelColor = Color.LightGray;

            Color effectiveBg = ColorService.BlendWithBackground(bgWithAlpha, panelColor);
            Color effectiveFg = ColorService.BlendWithBackground(fgWithAlpha, effectiveBg);

            pnlForegroundSwatch.BackColor = fgWithAlpha;
            pnlBackgroundSwatch.BackColor = bgWithAlpha;

            bool isBold = chkBold.Checked;
            FontStyle style = isBold ? FontStyle.Bold : FontStyle.Regular;
            lblPreviewText.Font = new Font(lblPreviewText.Font.FontFamily, (float)numTextSize.Value, style);

            double textSizePt = (double)numTextSize.Value;

            double contrastRatio = WcagContrastColor.GetContrastRatio(effectiveBg, effectiveFg);
            lblContrastRatio.Text = string.Format(resourceManager.GetString("ContrastRatio_Format", currentCulture) ?? "Contrast Ratio: {0:F2}:1", contrastRatio);

            bool isCompliant = WcagContrastColor.IsTextCompliant(effectiveBg, effectiveFg, textSizePt, isBold);

            lblCompliance.Text = isCompliant
                ? resourceManager.GetString("Compliance_Compliant", currentCulture) ?? "WCAG AA Compliant"
                : resourceManager.GetString("Compliance_NotCompliant", currentCulture) ?? "Not Compliant";

            lblCompliance.ForeColor = isCompliant ? Color.Green : Color.Red;

            pnlPreview.Invalidate();
        }
        catch
        {
            lblContrastRatio.Text = resourceManager.GetString("ContrastRatio_InvalidInput", currentCulture) ?? "Invalid Input";
            lblCompliance.Text = "";
        }
    }

    /// <summary>
    /// Changes the brightness of a color by a given factor.
    /// </summary>
    /// <param name="originalColor">The original color.</param>
    /// <param name="factor">The brightness factor. &lt;1 darkens, &gt;1 lightens.</param>
    /// <returns>The adjusted color.</returns>
    private static Color ChangeBrightness(Color originalColor, float factor)
    {
        RgbToHsl(originalColor, out float h, out float s, out float l);

        if (factor < 1.0f)
        {
            l *= factor;
        }
        else
        {
            l += (1 - l) * (factor - 1f);
        }

        l = Math.Min(Math.Max(l, 0), 1);
        return HslToRgb(h, s, l);
    }

    /// <summary>
    /// Converts an HSL color to an RGB color.
    /// </summary>
    /// <param name="h">Hue [0..1]</param>
    /// <param name="s">Saturation [0..1]</param>
    /// <param name="l">Lightness [0..1]</param>
    /// <returns>The resulting RGB color.</returns>
    private static Color HslToRgb(float h, float s, float l)
    {
        float r, g, b;
        if (Math.Abs(s) < 0.00001f)
        {
            r = g = b = l;
        }
        else
        {
            float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
            float p = 2f * l - q;

            r = HueToRgb(p, q, h + 1f / 3f);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1f / 3f);
        }

        return Color.FromArgb(
            255,
            ((int)(r * 255f)).Clamp(0, 255),
            ((int)(g * 255f)).Clamp(0, 255),
            ((int)(b * 255f)).Clamp(0, 255)
        );
    }

    /// <summary>
    /// Helper method for HSL to RGB conversion.
    /// </summary>
    private static float HueToRgb(float p, float q, float t)
    {
        if (t < 0f) t += 1f;
        if (t > 1f) t -= 1f;

        if (t < 1f / 6f) return p + (q - p) * 6f * t;
        if (t < 1f / 2f) return q;
        if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
        return p;
    }

    /// <summary>
    /// Converts an RGB color to HSL representation.
    /// </summary>
    /// <param name="color">The RGB color.</param>
    /// <param name="h">Hue [0..1]</param>
    /// <param name="s">Saturation [0..1]</param>
    /// <param name="l">Lightness [0..1]</param>
    private static void RgbToHsl(Color color, out float h, out float s, out float l)
    {
        float r = color.R / 255f;
        float g = color.G / 255f;
        float b = color.B / 255f;

        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));
        float delta = max - min;

        l = (max + min) / 2f;

        if (Math.Abs(delta) < 0.00001f)
        {
            s = 0f;
            h = 0f;
        }
        else
        {
            s = l < 0.5f ? delta / (max + min) : delta / (2f - max - min);

            float hue;
            if (Math.Abs(max - r) < 0.00001f)
                hue = (g - b) / delta + (g < b ? 6f : 0f);
            else if (Math.Abs(max - g) < 0.00001f)
                hue = (b - r) / delta + 2f;
            else
                hue = (r - g) / delta + 4f;

            h = hue / 6f;
            if (h < 0f) h += 1f;
            if (h > 1f) h -= 1f;
        }
    }

    /// <summary>
    /// Initiates the eyedropper overlay to pick a color from anywhere on the screen.
    /// </summary>
    private void StartEyedropper()
    {
        Hide();
        using (var overlay = new EyedropperOverlay())
        {
            if (overlay.ShowDialog() == DialogResult.OK)
            {
                Color chosenColor = overlay.SelectedColor;

                if (pickingForeground)
                {
                    isAdjustingForeground = false;
                    txtForegroundColor.Text = ColorTranslator.ToHtml(chosenColor);

                    sliderForeground.Value = 100;
                    originalForegroundColor = chosenColor;

                    UpdateContrastInfo();
                }
                else if (pickingBackground)
                {
                    isAdjustingBackground = false;
                    txtBackgroundColor.Text = ColorTranslator.ToHtml(chosenColor);

                    sliderBackground.Value = 100;
                    originalBackgroundColor = chosenColor;

                    UpdateContrastInfo();
                }
            }
        }
        Show();
        pickingForeground = false;
        pickingBackground = false;
    }

    /// <summary>
    /// Handles the Paint event of pnlPreview to custom draw the preview panel with alpha blending.
    /// </summary>
    private void PnlPreview_Paint(object? sender, PaintEventArgs e)
    {
        e.Graphics.Clear(Color.LightGray);

        // Draw background with alpha
        Color baseBgColor = ColorTranslator.FromHtml(txtBackgroundColor.Text);
        int bgAlpha = (int)numBackgroundAlpha.Value;
        Color alphaBackground = Color.FromArgb(bgAlpha, baseBgColor.R, baseBgColor.G, baseBgColor.B);
        using (var bgBrush = new SolidBrush(alphaBackground))
        {
            e.Graphics.FillRectangle(bgBrush, pnlPreview.ClientRectangle);
        }

        // Draw foreground text with alpha
        var fgBase = ColorTranslator.FromHtml(txtForegroundColor.Text);
        int fgAlpha = (int)numForegroundAlpha.Value;
        Color fgColor = Color.FromArgb(fgAlpha, fgBase.R, fgBase.G, fgBase.B);

        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        using var fgBrush = new SolidBrush(fgColor);
        var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
        e.Graphics.DrawString(lblPreviewText.Text, lblPreviewText.Font, fgBrush, pnlPreview.ClientRectangle, sf);
    }

    /// <summary>
    /// Handles the DragEnter event to provide visual feedback when dragging a .json file over the form.
    /// </summary>
    private void MainForm_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 1 && Path.GetExtension(files[0]).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                e.Effect = DragDropEffects.Copy;
                e.DropImageType = DropImageType.Copy;
                e.Message = "Load Palette";
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
    }

    /// <summary>
    /// Handles the DragDrop event to load the palette from the dropped .json file.
    /// </summary>
    private void MainForm_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 1 && Path.GetExtension(files[0]).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                LoadPalette(files[0]);
            }
        }
    }

    /// <summary>
    /// Loads a palette from a JSON file.
    /// </summary>
    /// <param name="filePath">The path to the JSON file.</param>
    private void LoadPalette(string filePath)
    {
        string json = File.ReadAllText(filePath);
        var palette = JsonSerializer.Deserialize<Palette>(json);
        if (palette != null)
        {
            txtForegroundColor.Text = palette.ForegroundColor;
            txtBackgroundColor.Text = palette.BackgroundColor;
            numForegroundAlpha.Value = palette.Alpha;
            numTextSize.Value = (decimal)palette.TextSizePt;
            chkBold.Checked = palette.IsBold;
            txtSampleText.Text = palette.SampleText;

            sliderForeground.Value = 100;
            sliderBackground.Value = 100;

            originalForegroundColor = ColorTranslator.FromHtml(palette.ForegroundColor);
            originalBackgroundColor = ColorTranslator.FromHtml(palette.BackgroundColor);

            UpdateContrastInfo();
        }
    }
}
