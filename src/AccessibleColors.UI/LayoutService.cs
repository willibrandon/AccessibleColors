namespace AccessibleColors.UI;

/// <summary>
/// Provides layout logic separated into its own class for cleaner architecture.
/// Handles scaling and layout adjustments for both stacked (narrow) and wide modes.
/// </summary>
public static class LayoutService
{
    /// <summary>
    /// Adjusts the layout of controls depending on whether the form is in stacked or wide mode.
    /// Invokes the appropriate layout method and suspends/resumes layout for smoother transitions.
    /// </summary>
    /// <param name="form">The main form reference.</param>
    /// <param name="pnlContainer">The panel container holding all controls.</param>
    /// <param name="picLogo">The logo picture box control.</param>
    /// <param name="lblHeading">The heading label control.</param>
    /// <param name="lblExplanationHeading">The explanation heading label.</param>
    /// <param name="txtExplanation">The explanation text box control.</param>
    /// <param name="grpForeground">The foreground settings group box.</param>
    /// <param name="grpBackground">The background settings group box.</param>
    /// <param name="grpTextSettings">The text settings group box.</param>
    /// <param name="pnlPreview">The preview panel control.</param>
    /// <param name="lblContrastRatio">The contrast ratio label.</param>
    /// <param name="lblCompliance">The compliance label.</param>
    /// <param name="btnSavePalette">The save palette button.</param>
    /// <param name="btnLoadPalette">The load palette button.</param>
    public static void AdjustLayoutForMode(
        Form form,
        Panel pnlContainer,
        PictureBox picLogo,
        Label lblHeading,
        Label lblExplanationHeading,
        TextBox txtExplanation,
        GroupBox grpForeground,
        GroupBox grpBackground,
        GroupBox grpTextSettings,
        Panel pnlPreview,
        Label lblContrastRatio,
        Label lblCompliance,
        Button btnSavePalette,
        Button btnLoadPalette)
    {
        int topSectionSpacing = 10;
        int leftX = 20;
        int currentY = 20;
        int rightMargin = 20;
        int containerWidth = pnlContainer.ClientSize.Width;
        int availableWidth = containerWidth - leftX - rightMargin;
        availableWidth = Math.Max(availableWidth, 50);

        form.AutoScroll = false;
        form.HorizontalScroll.Enabled = false;
        form.HorizontalScroll.Visible = false;

        float lineHeight = lblHeading.Font.GetHeight();

        // Suspend layout to prevent intermediate states
        pnlContainer.SuspendLayout();
        form.SuspendLayout();

        // Determine if we should use stacked mode by checking a reference control
        // in one of the group boxes. For example, we can check the rightmost control
        // in grpForeground or grpBackground.
        bool useStackedMode = ShouldUseStackedMode(grpForeground, grpBackground);

        if (useStackedMode)
        {
            LayoutStackedMode(
                picLogo, lblHeading, lblExplanationHeading, txtExplanation,
                grpForeground, grpBackground, grpTextSettings,
                pnlPreview, lblContrastRatio, lblCompliance, btnSavePalette, btnLoadPalette,
                lineHeight, availableWidth, leftX, topSectionSpacing, ref currentY, pnlContainer);
        }
        else
        {
            LayoutWideMode(
                lblHeading, lblExplanationHeading, txtExplanation,
                grpForeground, grpBackground, grpTextSettings,
                pnlPreview, lblContrastRatio, lblCompliance, btnSavePalette, btnLoadPalette,
                picLogo, availableWidth, leftX, topSectionSpacing, ref currentY, pnlContainer);
        }

        // Resume layout after finishing adjustments
        pnlContainer.ResumeLayout(false);
        pnlContainer.PerformLayout();
        form.ResumeLayout(true);
    }

    /// <summary>
    /// Called after the user finishes resizing the form, to re-check layout and scrollbars.
    /// Attempts to remove unnecessary horizontal scrollbars if everything fits.
    /// </summary>
    /// <param name="form">The main form instance.</param>
    /// <param name="pnlContainer">The container panel holding all controls.</param>
    public static void AfterResizeEnd(Form form, Panel pnlContainer)
    {
        pnlContainer.SuspendLayout();
        form.SuspendLayout();

        pnlContainer.AutoScrollPosition = new Point(0, 0);
        pnlContainer.PerformLayout();
        form.PerformLayout();

        int maxRight = 0;
        foreach (Control c in pnlContainer.Controls)
        {
            if (c.Visible && c.Right > maxRight)
                maxRight = c.Right;
        }

        if (maxRight <= pnlContainer.ClientSize.Width)
        {
            pnlContainer.AutoScrollMinSize = new Size(0, pnlContainer.AutoScrollMinSize.Height);
            pnlContainer.HorizontalScroll.Visible = false;
            pnlContainer.HorizontalScroll.Enabled = false;
            pnlContainer.HorizontalScroll.Value = 0;
            pnlContainer.PerformLayout();
        }

        form.AutoScroll = false;
        form.HorizontalScroll.Visible = false;
        form.HorizontalScroll.Enabled = false;
        form.HorizontalScroll.Value = 0;
        form.PerformLayout();

        pnlContainer.ResumeLayout(false);
        pnlContainer.PerformLayout();
        form.ResumeLayout(true);

        pnlContainer.Invalidate();
        pnlContainer.Update();
        form.Invalidate();
        form.Update();
    }

    /// <summary>
    /// Finalizes the layout after adjustments have been made, ensuring scrollbars only appear if needed.
    /// Tries to remove horizontal scrollbars if content fits within the container.
    /// </summary>
    /// <param name="form">The main form instance.</param>
    /// <param name="pnlContainer">The panel container of all controls.</param>
    /// <param name="lblHeading">The heading label, used to ensure it remains visible and properly sized.</param>

    public static void FinalizeLayout(Form form, Panel pnlContainer, Label lblHeading)
    {
        // Suspend layout updates
        pnlContainer.SuspendLayout();
        form.SuspendLayout();

        int maxRight = 0;
        int maxBottom = 0;
        foreach (Control c in pnlContainer.Controls)
        {
            if (c.Visible)
            {
                if (c.Right > maxRight) maxRight = c.Right;
                if (c.Bottom > maxBottom) maxBottom = c.Bottom;
            }
        }

        maxBottom += 50;

        pnlContainer.AutoScrollPosition = new Point(0, 0);

        if (maxRight <= pnlContainer.ClientSize.Width)
        {
            pnlContainer.AutoScrollMinSize = new Size(0, maxBottom);
            pnlContainer.HorizontalScroll.Enabled = false;
            pnlContainer.HorizontalScroll.Visible = false;
            pnlContainer.HorizontalScroll.Value = 0;
        }
        else
        {
            pnlContainer.AutoScrollMinSize = new Size(maxRight, maxBottom);
            pnlContainer.HorizontalScroll.Enabled = true;
            pnlContainer.HorizontalScroll.Visible = true;
        }

        float lineHeight = lblHeading.Font.GetHeight();
        if (lblHeading.Width < 1 || lblHeading.Height < 1)
        {
            lblHeading.Width = 200;
            lblHeading.Height = (int)(lineHeight * 2);
        }
        lblHeading.Visible = true;

        form.AutoScroll = false;
        form.HorizontalScroll.Visible = false;
        form.HorizontalScroll.Enabled = false;
        form.HorizontalScroll.Value = 0;

        // Resume layout now that we have final sizes
        pnlContainer.ResumeLayout(false);
        pnlContainer.PerformLayout();
        form.ResumeLayout(true);
        form.PerformLayout();
        pnlContainer.PerformLayout();

        // If scrollbar is visible but not needed, remove it again
        if (pnlContainer.HorizontalScroll.Visible && maxRight <= pnlContainer.ClientSize.Width)
        {
            pnlContainer.AutoScrollMinSize = new Size(0, maxBottom);
            pnlContainer.HorizontalScroll.Visible = false;
            pnlContainer.HorizontalScroll.Enabled = false;
            pnlContainer.HorizontalScroll.Value = 0;
            pnlContainer.PerformLayout();
        }

        pnlContainer.Invalidate();
        pnlContainer.Update();
        form.Invalidate();
        form.Update();
    }

    /// <summary>
    /// Recursively scales all controls within a parent control based on the given scale factors.
    /// Avoids scaling inside specified group boxes to prevent layout complexity and overlapping issues.
    /// </summary>
    /// <param name="parent">The parent control whose children are to be scaled.</param>
    /// <param name="scaleX">The horizontal scale factor.</param>
    /// <param name="scaleY">The vertical scale factor.</param>
    /// <param name="originalMetrics">A dictionary mapping each control to its original location and size.</param>
    /// <param name="grpForeground">The foreground group box whose child controls should not be recursively scaled.</param>
    /// <param name="grpBackground">The background group box whose child controls should not be recursively scaled.</param>
    /// <param name="grpTextSettings">The text settings group box whose child controls should not be recursively scaled.</param>
    public static void ScaleAllControls(
        Control parent,
        float scaleX,
        float scaleY,
        Dictionary<Control, (Point originalLocation, Size originalSize)> originalMetrics,
        Control grpForeground,
        Control grpBackground,
        Control grpTextSettings)
    {
        foreach (Control ctrl in parent.Controls)
        {
            if (originalMetrics.TryGetValue(ctrl, out var metrics))
            {
                int newX = (int)(metrics.originalLocation.X * scaleX);
                int newY = (int)(metrics.originalLocation.Y * scaleY);
                int newWidth = (int)(metrics.originalSize.Width * scaleX);
                int newHeight = (int)(metrics.originalSize.Height * scaleY);

                ctrl.Location = new Point(newX, newY);
                ctrl.Size = new Size(newWidth, newHeight);
            }

            // Avoid scaling children of certain group boxes
            if (ctrl != grpForeground && ctrl != grpBackground && ctrl != grpTextSettings && ctrl.HasChildren)
            {
                ScaleAllControls(ctrl, scaleX, scaleY, originalMetrics, grpForeground, grpBackground, grpTextSettings);
            }
        }
    }

    /// <summary>
    /// Finds the widest (rightmost) visible control within a given parent control.
    /// This helps identify if any control is approaching the right boundary.
    /// </summary>
    /// <param name="parent">The parent control in which to search.</param>
    /// <returns>The widest control, or null if no controls found.</returns>
    private static Control? FindWidestControl(Control parent)
    {
        Control? widest = null;
        int maxRight = -1;

        foreach (Control c in parent.Controls)
        {
            if (c.Visible && c.Right > maxRight)
            {
                maxRight = c.Right;
                widest = c;
            }
        }

        return widest;
    }

    /// <summary>
    /// Ensures that no child controls within the given panel exceed its available width, 
    /// adjusting their size or position to prevent horizontal overflow. If a control extends 
    /// beyond the panel's width, its width is reduced or its position is shifted to fit within 
    /// the visible area.
    /// </summary>
    /// <param name="pnlContainer">The container panel whose child controls are checked for overflow.</param>
    private static void EnsureNoHorizontalOverflow(Panel pnlContainer)
    {
        foreach (Control c in pnlContainer.Controls)
        {
            if (c.Right > pnlContainer.ClientSize.Width)
            {
                int overflow = c.Right - pnlContainer.ClientSize.Width;
                if (overflow > 0)
                {
                    if (c.Width - overflow > 0)
                        c.Width -= overflow;
                    else
                        c.Left = Math.Max(0, pnlContainer.ClientSize.Width - c.Width - 20);
                }
            }
        }

        pnlContainer.PerformLayout();
        pnlContainer.Invalidate();
        pnlContainer.Update();
    }

    /// <summary>
    /// Lays out controls in stacked mode, positioning them in a single column.
    /// Ensures that no horizontal overflow occurs.
    /// </summary>
    /// <param name="picLogo">The picture box for the logo.</param>
    /// <param name="lblHeading">The heading label.</param>
    /// <param name="lblExplanationHeading">The explanation heading label.</param>
    /// <param name="txtExplanation">The explanation text box.</param>
    /// <param name="grpForeground">The foreground settings group box.</param>
    /// <param name="grpBackground">The background settings group box.</param>
    /// <param name="grpTextSettings">The text settings group box.</param>
    /// <param name="pnlPreview">The preview panel.</param>
    /// <param name="lblContrastRatio">The contrast ratio label.</param>
    /// <param name="lblCompliance">The compliance label.</param>
    /// <param name="btnSavePalette">The save palette button.</param>
    /// <param name="btnLoadPalette">The load palette button.</param>
    /// <param name="lineHeight">The line height of the heading font for scaling.</param>
    /// <param name="availableWidth">The available width for layout.</param>
    /// <param name="leftX">The left margin for controls.</param>
    /// <param name="topSectionSpacing">The vertical spacing between sections.</param>
    /// <param name="currentY">A reference to the current Y position for placing controls.</param>
    /// <param name="pnlContainer">The container panel.</param>
    private static void LayoutStackedMode(
        PictureBox picLogo,
        Label lblHeading,
        Label lblExplanationHeading,
        TextBox txtExplanation,
        GroupBox grpForeground,
        GroupBox grpBackground,
        GroupBox grpTextSettings,
        Panel pnlPreview,
        Label lblContrastRatio,
        Label lblCompliance,
        Button btnSavePalette,
        Button btnLoadPalette,
        float lineHeight,
        int availableWidth,
        int leftX,
        int topSectionSpacing,
        ref int currentY,
        Panel pnlContainer)
    {
        lblHeading.AutoSize = true;
        lblHeading.MaximumSize = new Size(availableWidth, 0);
        lblHeading.PerformLayout();

        Size pref = lblHeading.PreferredSize;
        if (pref.Width < 1 || pref.Height < 1)
        {
            lblHeading.AutoSize = false;
            lblHeading.MaximumSize = Size.Empty;
            lblHeading.Size = new Size(100, (int)(lineHeight * 2));
        }

        picLogo.Left = leftX;
        picLogo.Top = 20;

        lblHeading.Left = picLogo.Right + 10;
        lblHeading.Top = picLogo.Top + (picLogo.Height - lblHeading.Height) / 2;

        currentY = Math.Max(picLogo.Bottom, lblHeading.Bottom) + topSectionSpacing;

        lblExplanationHeading.Left = leftX;
        lblExplanationHeading.Top = currentY;
        currentY = lblExplanationHeading.Bottom + topSectionSpacing;

        txtExplanation.Left = leftX;
        txtExplanation.Top = currentY;
        txtExplanation.Width = availableWidth;
        txtExplanation.Height = 100;
        currentY = txtExplanation.Bottom + topSectionSpacing;

        grpForeground.Left = leftX;
        grpForeground.Top = currentY;
        grpForeground.Width = availableWidth;
        ResizeGroupBoxToFitContents(grpForeground);
        currentY = grpForeground.Bottom + topSectionSpacing;

        grpBackground.Left = leftX;
        grpBackground.Top = currentY;
        grpBackground.Width = availableWidth;
        ResizeGroupBoxToFitContents(grpBackground);
        currentY = grpBackground.Bottom + topSectionSpacing;

        grpTextSettings.Left = leftX;
        grpTextSettings.Top = currentY;
        grpTextSettings.Width = availableWidth;
        ResizeGroupBoxToFitContents(grpTextSettings);
        currentY = grpTextSettings.Bottom + topSectionSpacing;

        pnlPreview.Left = leftX;
        pnlPreview.Top = currentY;
        pnlPreview.Width = availableWidth;
        currentY = pnlPreview.Bottom + topSectionSpacing;

        lblContrastRatio.Left = leftX;
        lblContrastRatio.Top = currentY;
        currentY = lblContrastRatio.Bottom + topSectionSpacing;

        lblCompliance.Left = leftX;
        lblCompliance.Top = currentY;
        currentY = lblCompliance.Bottom + topSectionSpacing;

        btnSavePalette.Left = leftX;
        btnSavePalette.Top = currentY;
        btnLoadPalette.Left = leftX + availableWidth - btnLoadPalette.Width;
        btnLoadPalette.Top = currentY;

        EnsureNoHorizontalOverflow(pnlContainer);
    }

    /// <summary>
    /// Lays out controls in wide mode, positioning them in two columns where appropriate.
    /// Also ensures no horizontal overflow occurs.
    /// </summary>
    /// <param name="lblHeading">The heading label.</param>
    /// <param name="lblExplanationHeading">The explanation heading label.</param>
    /// <param name="txtExplanation">The explanation text box.</param>
    /// <param name="grpForeground">The foreground settings group box.</param>
    /// <param name="grpBackground">The background settings group box.</param>
    /// <param name="grpTextSettings">The text settings group box.</param>
    /// <param name="pnlPreview">The preview panel.</param>
    /// <param name="lblContrastRatio">The contrast ratio label.</param>
    /// <param name="lblCompliance">The compliance label.</param>
    /// <param name="btnSavePalette">The save palette button.</param>
    /// <param name="btnLoadPalette">The load palette button.</param>
    /// <param name="picLogo">The logo picture box.</param>
    /// <param name="availableWidth">The available width for layout.</param>
    /// <param name="leftX">The left margin for controls.</param>
    /// <param name="topSectionSpacing">The vertical spacing between sections.</param>
    /// <param name="currentY">A reference to the current Y position for placing controls.</param>
    /// <param name="pnlContainer">The container panel.</param>
    private static void LayoutWideMode(
        Label lblHeading,
        Label lblExplanationHeading,
        TextBox txtExplanation,
        GroupBox grpForeground,
        GroupBox grpBackground,
        GroupBox grpTextSettings,
        Panel pnlPreview,
        Label lblContrastRatio,
        Label lblCompliance,
        Button btnSavePalette,
        Button btnLoadPalette,
        PictureBox picLogo,
        int availableWidth,
        int leftX,
        int topSectionSpacing,
        ref int currentY,
        Panel pnlContainer)
    {
        picLogo.Left = leftX;
        picLogo.Top = currentY;

        lblHeading.AutoSize = false;
        lblHeading.MaximumSize = Size.Empty;
        int headingWidth = Math.Max(availableWidth, 100);
        lblHeading.Width = headingWidth;

        int verticalCenter = picLogo.Top + (picLogo.Height - lblHeading.Height) / 2;
        lblHeading.Left = picLogo.Right + 10;
        lblHeading.Top = verticalCenter;

        currentY = Math.Max(picLogo.Bottom, lblHeading.Bottom) + topSectionSpacing;

        lblExplanationHeading.AutoSize = true;
        lblExplanationHeading.MaximumSize = new Size(availableWidth, 0);
        lblExplanationHeading.PerformLayout();
        lblExplanationHeading.Left = leftX;
        lblExplanationHeading.Top = currentY;
        currentY = lblExplanationHeading.Bottom + topSectionSpacing;

        txtExplanation.Left = leftX;
        txtExplanation.Top = currentY;
        txtExplanation.Width = availableWidth;
        currentY = txtExplanation.Bottom + topSectionSpacing;

        int columnSpacing = 20;
        int halfWidth = (availableWidth - columnSpacing) / 2;
        halfWidth = Math.Max(halfWidth, 50);

        grpForeground.Left = leftX;
        grpForeground.Top = currentY;
        grpForeground.Width = halfWidth;
        ResizeGroupBoxToFitContents(grpForeground);

        grpBackground.Left = leftX + halfWidth + columnSpacing;
        grpBackground.Top = currentY;
        grpBackground.Width = halfWidth;
        ResizeGroupBoxToFitContents(grpBackground);

        int fgWidth = grpForeground.Width;
        int bgWidth = grpBackground.Width;
        int maxWidth = Math.Min(Math.Max(fgWidth, bgWidth), halfWidth);
        grpForeground.Width = maxWidth;
        grpBackground.Width = maxWidth;
        grpBackground.Left = leftX + maxWidth + columnSpacing;

        currentY = Math.Max(grpForeground.Bottom, grpBackground.Bottom) + topSectionSpacing;

        grpTextSettings.Left = leftX;
        grpTextSettings.Top = currentY;
        grpTextSettings.Width = availableWidth;
        ResizeGroupBoxToFitContents(grpTextSettings);
        currentY = grpTextSettings.Bottom + topSectionSpacing;

        pnlPreview.Left = leftX;
        pnlPreview.Top = currentY;
        pnlPreview.Width = availableWidth;
        currentY = pnlPreview.Bottom + topSectionSpacing;

        lblContrastRatio.Left = leftX;
        lblContrastRatio.Top = currentY;
        currentY = lblContrastRatio.Bottom + topSectionSpacing;

        lblCompliance.Left = leftX;
        lblCompliance.Top = currentY;
        currentY = lblCompliance.Bottom + topSectionSpacing;

        btnSavePalette.Left = leftX;
        btnSavePalette.Top = currentY;
        btnLoadPalette.Left = leftX + availableWidth - btnLoadPalette.Width;
        btnLoadPalette.Top = currentY;

        EnsureNoHorizontalOverflow(pnlContainer);
    }

    /// <summary>
    /// Resizes a given group box to fit its contained controls by increasing its height
    /// to match the tallest child control plus some padding.
    /// </summary>
    /// <param name="groupBox">The group box to resize.</param>
    private static void ResizeGroupBoxToFitContents(GroupBox groupBox)
    {
        int maxBottom = 0;
        foreach (Control c in groupBox.Controls)
        {
            if (c.Bottom > maxBottom) maxBottom = c.Bottom;
        }

        int padding = 20;
        groupBox.Height = maxBottom + padding;
    }

    /// <summary>
    /// Determines whether stacked mode should be used by checking if any key control within
    /// the specified group boxes touches or exceeds the right boundary of that group box.
    /// If the control appears too close or overlapping the edge, stacked mode is chosen.
    /// </summary>
    /// <param name="grpForeground">The foreground group box to check.</param>
    /// <param name="grpBackground">The background group box to check.</param>
    /// <returns>true if stacked mode should be used; otherwise, false for wide mode.</returns>
    private static bool ShouldUseStackedMode(GroupBox grpForeground, GroupBox grpBackground)
    {
        // Example logic: Identify a critical control in the group boxes
        // that should not collide with the right edge. This could be the
        // rightmost control in grpForeground or grpBackground.
        // Let's find the widest control in either group box and see if it touches the edge.

        Control? widestFgControl = FindWidestControl(grpForeground);
        Control? widestBgControl = FindWidestControl(grpBackground);

        // Decide which control to base decisions on. For simplicity, let's pick the "worst case":
        // the control that sticks out the furthest overall.
        Control? widestControl = null;
        if (widestFgControl != null && widestBgControl != null)
        {
            widestControl = (widestFgControl.Right > widestBgControl.Right) ? widestFgControl : widestBgControl;
        }
        else if (widestFgControl != null)
        {
            widestControl = widestFgControl;
        }
        else if (widestBgControl != null)
        {
            widestControl = widestBgControl;
        }

        if (widestControl == null)
        {
            // If we can't find a relevant control, default to wide mode.
            return false;
        }

        // Determine if the control touches the right boundary of its parent group box
        int margin = 10; // Allow some margin so it's not exactly at the edge
        int parentWidth = widestControl.Parent!.ClientSize.Width;
        return (widestControl.Right + margin >= parentWidth);
    }
}
