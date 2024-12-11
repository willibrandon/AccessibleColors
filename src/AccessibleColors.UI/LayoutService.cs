namespace AccessibleColors.UI;

public static class LayoutService
{
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
        int breakpoint = 915;
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

        if (form.Width < breakpoint)
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

    public static void ResizeGroupBoxToFitContents(GroupBox groupBox)
    {
        int maxBottom = 0;
        foreach (Control c in groupBox.Controls)
        {
            if (c.Bottom > maxBottom) maxBottom = c.Bottom;
        }

        int padding = 20;
        groupBox.Height = maxBottom + padding;
    }

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
}

///// <summary>
///// Provides layout logic separated into its own class for cleaner architecture.
///// Handles scaling and layout adjustments for stacked or wide mode.
///// </summary>
//public static class LayoutService
//{
//    /// <summary>
//    /// Recursively scales all controls based on the provided scale factors.
//    /// Avoids scaling inside certain containers to prevent complexity.
//    /// </summary>
//    public static void ScaleAllControls(
//        Control parent,
//        float scaleX,
//        float scaleY,
//        Dictionary<Control, (Point originalLocation, Size originalSize)> originalMetrics,
//        Control grpForeground,
//        Control grpBackground,
//        Control grpTextSettings)
//    {
//        foreach (Control ctrl in parent.Controls)
//        {
//            if (originalMetrics.TryGetValue(ctrl, out var metrics))
//            {
//                int newX = (int)(metrics.originalLocation.X * scaleX);
//                int newY = (int)(metrics.originalLocation.Y * scaleY);
//                int newWidth = (int)(metrics.originalSize.Width * scaleX);
//                int newHeight = (int)(metrics.originalSize.Height * scaleY);

//                ctrl.Location = new Point(newX, newY);
//                ctrl.Size = new Size(newWidth, newHeight);
//            }

//            // Avoid scaling children of certain group boxes
//            if (ctrl != grpForeground && ctrl != grpBackground && ctrl != grpTextSettings && ctrl.HasChildren)
//            {
//                ScaleAllControls(ctrl, scaleX, scaleY, originalMetrics, grpForeground, grpBackground, grpTextSettings);
//            }
//        }
//    }

//    /// <summary>
//    /// Adjusts layout for either stacked or wide mode.
//    /// </summary>
//    public static void AdjustLayoutForMode(
//        Form form,
//        Panel pnlContainer,
//        PictureBox picLogo,
//        Label lblHeading,
//        Label lblExplanationHeading,
//        TextBox txtExplanation,
//        GroupBox grpForeground,
//        GroupBox grpBackground,
//        GroupBox grpTextSettings,
//        Panel pnlPreview,
//        Label lblContrastRatio,
//        Label lblCompliance,
//        Button btnSavePalette,
//        Button btnLoadPalette)
//    {
//        int breakpoint = 915;
//        int topSectionSpacing = 10;
//        int leftX = 20;
//        int currentY = 20;
//        int rightMargin = 20;
//        int containerWidth = pnlContainer.ClientSize.Width;
//        int availableWidth = containerWidth - leftX - rightMargin;
//        availableWidth = Math.Max(availableWidth, 50);

//        form.AutoScroll = false;
//        form.HorizontalScroll.Enabled = false;
//        form.HorizontalScroll.Visible = false;

//        float lineHeight = lblHeading.Font.GetHeight();

//        if (form.Width < breakpoint)
//        {
//            LayoutStackedMode(
//                picLogo, lblHeading, lblExplanationHeading, txtExplanation,
//                grpForeground, grpBackground, grpTextSettings,
//                pnlPreview, lblContrastRatio, lblCompliance, btnSavePalette, btnLoadPalette,
//                lineHeight, availableWidth, leftX, topSectionSpacing, ref currentY, pnlContainer);
//        }
//        else
//        {
//            LayoutWideMode(
//                lblHeading, lblExplanationHeading, txtExplanation,
//                grpForeground, grpBackground, grpTextSettings,
//                pnlPreview, lblContrastRatio, lblCompliance, btnSavePalette, btnLoadPalette,
//                picLogo, availableWidth, leftX, topSectionSpacing, ref currentY, pnlContainer);
//        }
//    }

//    /// <summary>
//    /// Finalizes layout and ensures no unnecessary horizontal scrollbar.
//    /// </summary>
//    public static void FinalizeLayout(Form form, Panel pnlContainer, Label lblHeading)
//    {
//        int maxRight = 0;
//        int maxBottom = 0;
//        foreach (Control c in pnlContainer.Controls)
//        {
//            if (c.Visible)
//            {
//                if (c.Right > maxRight) maxRight = c.Right;
//                if (c.Bottom > maxBottom) maxBottom = c.Bottom;
//            }
//        }

//        // Add bottom padding only
//        maxBottom += 50;

//        pnlContainer.AutoScrollPosition = new Point(0, 0);

//        // If all fits horizontally, no horizontal scroll needed
//        if (maxRight <= pnlContainer.ClientSize.Width)
//        {
//            pnlContainer.AutoScrollMinSize = new Size(0, maxBottom);
//            pnlContainer.HorizontalScroll.Enabled = false;
//            pnlContainer.HorizontalScroll.Visible = false;
//            pnlContainer.HorizontalScroll.Value = 0;
//        }
//        else
//        {
//            // Content exceeds available width
//            pnlContainer.AutoScrollMinSize = new Size(maxRight, maxBottom);
//            pnlContainer.HorizontalScroll.Enabled = true;
//            pnlContainer.HorizontalScroll.Visible = true;
//        }

//        pnlContainer.AutoScroll = true;
//        pnlContainer.PerformLayout();
//        pnlContainer.Invalidate();
//        pnlContainer.Update();

//        float lineHeight = lblHeading.Font.GetHeight();
//        if (lblHeading.Width < 1 || lblHeading.Height < 1)
//        {
//            lblHeading.Width = 200;
//            lblHeading.Height = (int)(lineHeight * 2);
//        }
//        lblHeading.Visible = true;

//        form.AutoScroll = false;
//        form.HorizontalScroll.Visible = false;
//        form.HorizontalScroll.Enabled = false;
//        form.HorizontalScroll.Value = 0;
//        form.PerformLayout();
//        form.Invalidate();
//        form.Update();

//        // Double-check if we can remove horizontal scroll if it's currently visible and doesn't need to be
//        if (pnlContainer.HorizontalScroll.Visible && maxRight <= pnlContainer.ClientSize.Width)
//        {
//            pnlContainer.AutoScrollMinSize = new Size(0, maxBottom);
//            pnlContainer.HorizontalScroll.Visible = false;
//            pnlContainer.HorizontalScroll.Enabled = false;
//            pnlContainer.HorizontalScroll.Value = 0;
//            pnlContainer.PerformLayout();
//            pnlContainer.Invalidate();
//            pnlContainer.Update();
//        }
//    }

//    /// <summary>
//    /// Called after resizing ends to ensure no unnecessary scrollbar.
//    /// </summary>
//    public static void AfterResizeEnd(Form form, Panel pnlContainer)
//    {
//        pnlContainer.AutoScrollPosition = new Point(0, 0);
//        pnlContainer.PerformLayout();
//        form.PerformLayout();

//        int maxRight = 0;
//        foreach (Control c in pnlContainer.Controls)
//        {
//            if (c.Visible && c.Right > maxRight)
//                maxRight = c.Right;
//        }

//        if (maxRight <= pnlContainer.ClientSize.Width)
//        {
//            // Fits horizontally, no horizontal scroll needed
//            pnlContainer.AutoScrollMinSize = new Size(0, pnlContainer.AutoScrollMinSize.Height);
//            pnlContainer.HorizontalScroll.Visible = false;
//            pnlContainer.HorizontalScroll.Enabled = false;
//            pnlContainer.HorizontalScroll.Value = 0;
//            pnlContainer.PerformLayout();
//            pnlContainer.Invalidate();
//            pnlContainer.Update();
//        }

//        form.AutoScroll = false;
//        form.HorizontalScroll.Visible = false;
//        form.HorizontalScroll.Enabled = false;
//        form.HorizontalScroll.Value = 0;
//        form.PerformLayout();
//        form.Invalidate();
//        form.Update();
//    }

//    private static void LayoutStackedMode(
//        PictureBox picLogo,
//        Label lblHeading,
//        Label lblExplanationHeading,
//        TextBox txtExplanation,
//        GroupBox grpForeground,
//        GroupBox grpBackground,
//        GroupBox grpTextSettings,
//        Panel pnlPreview,
//        Label lblContrastRatio,
//        Label lblCompliance,
//        Button btnSavePalette,
//        Button btnLoadPalette,
//        float lineHeight,
//        int availableWidth,
//        int leftX,
//        int topSectionSpacing,
//        ref int currentY,
//        Panel pnlContainer)
//    {
//        lblHeading.AutoSize = true;
//        lblHeading.MaximumSize = new Size(availableWidth, 0);
//        lblHeading.PerformLayout();

//        Size pref = lblHeading.PreferredSize;
//        if (pref.Width < 1 || pref.Height < 1)
//        {
//            lblHeading.AutoSize = false;
//            lblHeading.MaximumSize = Size.Empty;
//            lblHeading.Size = new Size(100, (int)(lineHeight * 2));
//        }

//        picLogo.Left = leftX;
//        picLogo.Top = 20;

//        lblHeading.Left = picLogo.Right + 10;
//        lblHeading.Top = picLogo.Top + (picLogo.Height - lblHeading.Height) / 2;

//        currentY = Math.Max(picLogo.Bottom, lblHeading.Bottom) + topSectionSpacing;

//        lblExplanationHeading.Left = leftX;
//        lblExplanationHeading.Top = currentY;
//        currentY = lblExplanationHeading.Bottom + topSectionSpacing;

//        txtExplanation.Left = leftX;
//        txtExplanation.Top = currentY;
//        txtExplanation.Width = availableWidth;
//        txtExplanation.Height = 100;
//        currentY = txtExplanation.Bottom + topSectionSpacing;

//        grpForeground.Left = leftX;
//        grpForeground.Top = currentY;
//        grpForeground.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpForeground);
//        currentY = grpForeground.Bottom + topSectionSpacing;

//        grpBackground.Left = leftX;
//        grpBackground.Top = currentY;
//        grpBackground.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpBackground);
//        currentY = grpBackground.Bottom + topSectionSpacing;

//        grpTextSettings.Left = leftX;
//        grpTextSettings.Top = currentY;
//        grpTextSettings.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpTextSettings);
//        currentY = grpTextSettings.Bottom + topSectionSpacing;

//        pnlPreview.Left = leftX;
//        pnlPreview.Top = currentY;
//        pnlPreview.Width = availableWidth;
//        currentY = pnlPreview.Bottom + topSectionSpacing;

//        lblContrastRatio.Left = leftX;
//        lblContrastRatio.Top = currentY;
//        currentY = lblContrastRatio.Bottom + topSectionSpacing;

//        lblCompliance.Left = leftX;
//        lblCompliance.Top = currentY;
//        currentY = lblCompliance.Bottom + topSectionSpacing;

//        btnSavePalette.Left = leftX;
//        btnSavePalette.Top = currentY;
//        btnLoadPalette.Left = leftX + availableWidth - btnLoadPalette.Width;
//        btnLoadPalette.Top = currentY;

//        EnsureNoHorizontalOverflow(pnlContainer);
//    }

//    private static void LayoutWideMode(
//        Label lblHeading,
//        Label lblExplanationHeading,
//        TextBox txtExplanation,
//        GroupBox grpForeground,
//        GroupBox grpBackground,
//        GroupBox grpTextSettings,
//        Panel pnlPreview,
//        Label lblContrastRatio,
//        Label lblCompliance,
//        Button btnSavePalette,
//        Button btnLoadPalette,
//        PictureBox picLogo,
//        int availableWidth,
//        int leftX,
//        int topSectionSpacing,
//        ref int currentY,
//        Panel pnlContainer)
//    {
//        picLogo.Left = leftX;
//        picLogo.Top = currentY;
//        currentY = picLogo.Bottom + topSectionSpacing;

//        lblHeading.AutoSize = false;
//        lblHeading.MaximumSize = Size.Empty;
//        int headingWidth = Math.Max(availableWidth, 100);
//        lblHeading.Width = headingWidth;

//        int verticalCenter = picLogo.Top + (picLogo.Height - lblHeading.Height) / 2;
//        lblHeading.Left = picLogo.Right + 10;
//        lblHeading.Top = verticalCenter;

//        currentY = Math.Max(picLogo.Bottom, lblHeading.Bottom) + topSectionSpacing;

//        lblExplanationHeading.AutoSize = true;
//        lblExplanationHeading.MaximumSize = new Size(availableWidth, 0);
//        lblExplanationHeading.PerformLayout();
//        lblExplanationHeading.Left = leftX;
//        lblExplanationHeading.Top = currentY;
//        currentY = lblExplanationHeading.Bottom + topSectionSpacing;

//        txtExplanation.Left = leftX;
//        txtExplanation.Top = currentY;
//        txtExplanation.Width = availableWidth;
//        currentY = txtExplanation.Bottom + topSectionSpacing;

//        int columnSpacing = 20;
//        int halfWidth = (availableWidth - columnSpacing) / 2;
//        halfWidth = Math.Max(halfWidth, 50);

//        grpForeground.Left = leftX;
//        grpForeground.Top = currentY;
//        grpForeground.Width = halfWidth;
//        ResizeGroupBoxToFitContents(grpForeground);

//        grpBackground.Left = leftX + halfWidth + columnSpacing;
//        grpBackground.Top = currentY;
//        grpBackground.Width = halfWidth;
//        ResizeGroupBoxToFitContents(grpBackground);

//        int fgWidth = grpForeground.Width;
//        int bgWidth = grpBackground.Width;
//        int maxWidth = Math.Min(Math.Max(fgWidth, bgWidth), halfWidth);
//        grpForeground.Width = maxWidth;
//        grpBackground.Width = maxWidth;
//        grpBackground.Left = leftX + maxWidth + columnSpacing;

//        currentY = Math.Max(grpForeground.Bottom, grpBackground.Bottom) + topSectionSpacing;

//        grpTextSettings.Left = leftX;
//        grpTextSettings.Top = currentY;
//        grpTextSettings.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpTextSettings);
//        currentY = grpTextSettings.Bottom + topSectionSpacing;

//        pnlPreview.Left = leftX;
//        pnlPreview.Top = currentY;
//        pnlPreview.Width = availableWidth;
//        currentY = pnlPreview.Bottom + topSectionSpacing;

//        lblContrastRatio.Left = leftX;
//        lblContrastRatio.Top = currentY;
//        currentY = lblContrastRatio.Bottom + topSectionSpacing;

//        lblCompliance.Left = leftX;
//        lblCompliance.Top = currentY;
//        currentY = lblCompliance.Bottom + topSectionSpacing;

//        btnSavePalette.Left = leftX;
//        btnSavePalette.Top = currentY;
//        btnLoadPalette.Left = leftX + availableWidth - btnLoadPalette.Width;
//        btnLoadPalette.Top = currentY;

//        EnsureNoHorizontalOverflow(pnlContainer);
//    }

//    /// <summary>
//    /// Resizes a group box to fit its contained controls by increasing its height.
//    /// </summary>
//    public static void ResizeGroupBoxToFitContents(GroupBox groupBox)
//    {
//        int maxBottom = 0;
//        foreach (Control c in groupBox.Controls)
//        {
//            if (c.Bottom > maxBottom) maxBottom = c.Bottom;
//        }

//        int padding = 20;
//        groupBox.Height = maxBottom + padding;
//    }

//    /// <summary>
//    /// Ensures that no controls exceed the panel width and adjust if necessary.
//    /// </summary>
//    private static void EnsureNoHorizontalOverflow(Panel pnlContainer)
//    {
//        // If any control exceeds panel width, clamp it
//        foreach (Control c in pnlContainer.Controls)
//        {
//            if (c.Right > pnlContainer.ClientSize.Width)
//            {
//                int overflow = c.Right - pnlContainer.ClientSize.Width;
//                if (overflow > 0)
//                {
//                    // Reduce width or adjust left as needed
//                    if (c.Width - overflow > 0)
//                        c.Width -= overflow;
//                    else
//                        c.Left = Math.Max(0, pnlContainer.ClientSize.Width - c.Width - 20);
//                }
//            }
//        }

//        pnlContainer.PerformLayout();
//        pnlContainer.Invalidate();
//        pnlContainer.Update();
//    }
//}


///// <summary>
///// Provides layout logic separated into its own class for cleaner architecture.
///// Handles scaling and layout adjustments for stacked or wide mode.
///// </summary>
//public static class LayoutService
//{
//    /// <summary>
//    /// Recursively scales all controls based on the provided scale factors.
//    /// Avoids scaling inside certain containers to prevent complexity.
//    /// </summary>
//    public static void ScaleAllControls(
//        Control parent,
//        float scaleX,
//        float scaleY,
//        Dictionary<Control, (Point originalLocation, Size originalSize)> originalMetrics,
//        Control grpForeground,
//        Control grpBackground,
//        Control grpTextSettings)
//    {
//        foreach (Control ctrl in parent.Controls)
//        {
//            if (originalMetrics.TryGetValue(ctrl, out var metrics))
//            {
//                int newX = (int)(metrics.originalLocation.X * scaleX);
//                int newY = (int)(metrics.originalLocation.Y * scaleY);
//                int newWidth = (int)(metrics.originalSize.Width * scaleX);
//                int newHeight = (int)(metrics.originalSize.Height * scaleY);

//                ctrl.Location = new Point(newX, newY);
//                ctrl.Size = new Size(newWidth, newHeight);
//            }

//            // Avoid scaling children of certain group boxes
//            if (ctrl != grpForeground && ctrl != grpBackground && ctrl != grpTextSettings && ctrl.HasChildren)
//            {
//                ScaleAllControls(ctrl, scaleX, scaleY, originalMetrics, grpForeground, grpBackground, grpTextSettings);
//            }
//        }
//    }

//    /// <summary>
//    /// Adjusts layout for either stacked or wide mode.
//    /// </summary>
//    public static void AdjustLayoutForMode(
//        Form form,
//        Panel pnlContainer,
//        PictureBox picLogo,
//        Label lblHeading,
//        Label lblExplanationHeading,
//        TextBox txtExplanation,
//        GroupBox grpForeground,
//        GroupBox grpBackground,
//        GroupBox grpTextSettings,
//        Panel pnlPreview,
//        Label lblContrastRatio,
//        Label lblCompliance,
//        Button btnSavePalette,
//        Button btnLoadPalette)
//    {
//        int breakpoint = 800;
//        int topSectionSpacing = 10;
//        int leftX = 20;
//        int currentY = 20;
//        int rightMargin = 20;
//        int containerWidth = pnlContainer.ClientSize.Width;
//        int availableWidth = containerWidth - leftX - rightMargin;
//        availableWidth = Math.Max(availableWidth, 50);

//        form.AutoScroll = false;
//        form.HorizontalScroll.Enabled = false;
//        form.HorizontalScroll.Visible = false;

//        float lineHeight = lblHeading.Font.GetHeight();

//        if (form.Width < breakpoint)
//        {
//            LayoutStackedMode(
//                picLogo, lblHeading, lblExplanationHeading, txtExplanation,
//                grpForeground, grpBackground, grpTextSettings,
//                pnlPreview, lblContrastRatio, lblCompliance, btnSavePalette, btnLoadPalette,
//                lineHeight, availableWidth, leftX, topSectionSpacing, ref currentY);
//        }
//        else
//        {
//            LayoutWideMode(
//                lblHeading, lblExplanationHeading, txtExplanation,
//                grpForeground, grpBackground, grpTextSettings,
//                pnlPreview, lblContrastRatio, lblCompliance, btnSavePalette, btnLoadPalette,
//                picLogo, availableWidth, leftX, topSectionSpacing, ref currentY);
//        }
//    }

//    /// <summary>
//    /// Finalizes layout and ensures no unnecessary horizontal scrollbar.
//    /// </summary>
//    public static void FinalizeLayout(Form form, Panel pnlContainer, Label lblHeading)
//    {
//        int maxRight = 0;
//        int maxBottom = 0;
//        foreach (Control c in pnlContainer.Controls)
//        {
//            if (c.Visible)
//            {
//                if (c.Right > maxRight) maxRight = c.Right;
//                if (c.Bottom > maxBottom) maxBottom = c.Bottom;
//            }
//        }

//        // Do not add extra margin to maxRight here; just use controls' actual bounds
//        // maxRight += 20; // remove this extra margin

//        // Add some bottom padding only for vertical space
//        maxBottom += 50;

//        pnlContainer.AutoScrollPosition = new Point(0, 0);

//        // Determine if horizontal scrollbar is needed
//        if (maxRight <= pnlContainer.ClientSize.Width)
//        {
//            // All content fits horizontally
//            pnlContainer.AutoScrollMinSize = new Size(0, maxBottom);
//            pnlContainer.HorizontalScroll.Enabled = false;
//            pnlContainer.HorizontalScroll.Visible = false;
//            pnlContainer.HorizontalScroll.Value = 0;
//        }
//        else
//        {
//            // Content exceeds available width
//            pnlContainer.AutoScrollMinSize = new Size(maxRight, maxBottom);
//            pnlContainer.HorizontalScroll.Enabled = true;
//            pnlContainer.HorizontalScroll.Visible = true;
//        }

//        pnlContainer.AutoScroll = true;
//        pnlContainer.PerformLayout();
//        pnlContainer.Invalidate();
//        pnlContainer.Update();

//        float lineHeight = lblHeading.Font.GetHeight();
//        if (lblHeading.Width < 1 || lblHeading.Height < 1)
//        {
//            lblHeading.Width = 200;
//            lblHeading.Height = (int)(lineHeight * 2);
//        }
//        lblHeading.Visible = true;

//        form.AutoScroll = false;
//        form.HorizontalScroll.Visible = false;
//        form.HorizontalScroll.Enabled = false;
//        form.HorizontalScroll.Value = 0;
//        form.PerformLayout();
//        form.Invalidate();
//        form.Update();
//    }

//    /// <summary>
//    /// Called after resizing ends to ensure no unnecessary scrollbar.
//    /// </summary>
//    public static void AfterResizeEnd(Form form, Panel pnlContainer)
//    {
//        pnlContainer.AutoScrollPosition = new Point(0, 0);
//        pnlContainer.PerformLayout();
//        form.PerformLayout();

//        int maxRight = 0;
//        foreach (Control c in pnlContainer.Controls)
//        {
//            if (c.Visible && c.Right > maxRight)
//                maxRight = c.Right;
//        }

//        if (maxRight <= pnlContainer.ClientSize.Width)
//        {
//            // Fits horizontally, no horizontal scroll needed
//            pnlContainer.AutoScrollMinSize = new Size(0, pnlContainer.AutoScrollMinSize.Height);
//            pnlContainer.HorizontalScroll.Visible = false;
//            pnlContainer.HorizontalScroll.Enabled = false;
//            pnlContainer.HorizontalScroll.Value = 0;
//            pnlContainer.PerformLayout();
//            pnlContainer.Invalidate();
//            pnlContainer.Update();
//        }

//        form.AutoScroll = false;
//        form.HorizontalScroll.Visible = false;
//        form.HorizontalScroll.Enabled = false;
//        form.HorizontalScroll.Value = 0;
//        form.PerformLayout();
//        form.Invalidate();
//        form.Update();
//    }

//    private static void LayoutStackedMode(
//        PictureBox picLogo,
//        Label lblHeading,
//        Label lblExplanationHeading,
//        TextBox txtExplanation,
//        GroupBox grpForeground,
//        GroupBox grpBackground,
//        GroupBox grpTextSettings,
//        Panel pnlPreview,
//        Label lblContrastRatio,
//        Label lblCompliance,
//        Button btnSavePalette,
//        Button btnLoadPalette,
//        float lineHeight,
//        int availableWidth,
//        int leftX,
//        int topSectionSpacing,
//        ref int currentY)
//    {
//        lblHeading.AutoSize = true;
//        lblHeading.MaximumSize = new Size(availableWidth, 0);
//        lblHeading.PerformLayout();

//        Size pref = lblHeading.PreferredSize;
//        if (pref.Width < 1 || pref.Height < 1)
//        {
//            lblHeading.AutoSize = false;
//            lblHeading.MaximumSize = Size.Empty;
//            lblHeading.Size = new Size(100, (int)(lineHeight * 2));
//        }

//        picLogo.Left = leftX;
//        picLogo.Top = 20;

//        lblHeading.Left = picLogo.Right + 10;
//        lblHeading.Top = picLogo.Top + (picLogo.Height - lblHeading.Height) / 2;

//        currentY = Math.Max(picLogo.Bottom, lblHeading.Bottom) + topSectionSpacing;

//        lblExplanationHeading.Left = leftX;
//        lblExplanationHeading.Top = currentY;
//        currentY = lblExplanationHeading.Bottom + topSectionSpacing;

//        txtExplanation.Left = leftX;
//        txtExplanation.Top = currentY;
//        txtExplanation.Width = availableWidth;
//        txtExplanation.Height = 100;
//        currentY = txtExplanation.Bottom + topSectionSpacing;

//        grpForeground.Left = leftX;
//        grpForeground.Top = currentY;
//        grpForeground.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpForeground);
//        currentY = grpForeground.Bottom + topSectionSpacing;

//        grpBackground.Left = leftX;
//        grpBackground.Top = currentY;
//        grpBackground.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpBackground);
//        currentY = grpBackground.Bottom + topSectionSpacing;

//        grpTextSettings.Left = leftX;
//        grpTextSettings.Top = currentY;
//        grpTextSettings.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpTextSettings);
//        currentY = grpTextSettings.Bottom + topSectionSpacing;

//        pnlPreview.Left = leftX;
//        pnlPreview.Top = currentY;
//        pnlPreview.Width = availableWidth;
//        currentY = pnlPreview.Bottom + topSectionSpacing;

//        lblContrastRatio.Left = leftX;
//        lblContrastRatio.Top = currentY;
//        currentY = lblContrastRatio.Bottom + topSectionSpacing;

//        lblCompliance.Left = leftX;
//        lblCompliance.Top = currentY;
//        currentY = lblCompliance.Bottom + topSectionSpacing;

//        btnSavePalette.Left = leftX;
//        btnSavePalette.Top = currentY;
//        btnLoadPalette.Left = leftX + availableWidth - btnLoadPalette.Width;
//        btnLoadPalette.Top = currentY;
//    }

//    private static void LayoutWideMode(
//        Label lblHeading,
//        Label lblExplanationHeading,
//        TextBox txtExplanation,
//        GroupBox grpForeground,
//        GroupBox grpBackground,
//        GroupBox grpTextSettings,
//        Panel pnlPreview,
//        Label lblContrastRatio,
//        Label lblCompliance,
//        Button btnSavePalette,
//        Button btnLoadPalette,
//        PictureBox picLogo,
//        int availableWidth,
//        int leftX,
//        int topSectionSpacing,
//        ref int currentY)
//    {
//        picLogo.Left = leftX;
//        picLogo.Top = currentY;
//        currentY = picLogo.Bottom + topSectionSpacing;

//        lblHeading.AutoSize = false;
//        lblHeading.MaximumSize = Size.Empty;
//        int headingWidth = Math.Max(availableWidth, 100);
//        lblHeading.Width = headingWidth;

//        int verticalCenter = picLogo.Top + (picLogo.Height - lblHeading.Height) / 2;
//        lblHeading.Left = picLogo.Right + 10;
//        lblHeading.Top = verticalCenter;

//        currentY = Math.Max(picLogo.Bottom, lblHeading.Bottom) + topSectionSpacing;

//        lblExplanationHeading.AutoSize = true;
//        lblExplanationHeading.MaximumSize = new Size(availableWidth, 0);
//        lblExplanationHeading.PerformLayout();
//        lblExplanationHeading.Left = leftX;
//        lblExplanationHeading.Top = currentY;
//        currentY = lblExplanationHeading.Bottom + topSectionSpacing;

//        txtExplanation.Left = leftX;
//        txtExplanation.Top = currentY;
//        txtExplanation.Width = availableWidth;
//        currentY = txtExplanation.Bottom + topSectionSpacing;

//        int columnSpacing = 20;
//        int halfWidth = (availableWidth - columnSpacing) / 2;
//        halfWidth = Math.Max(halfWidth, 50);

//        grpForeground.Left = leftX;
//        grpForeground.Top = currentY;
//        grpForeground.Width = halfWidth;
//        ResizeGroupBoxToFitContents(grpForeground);

//        grpBackground.Left = leftX + halfWidth + columnSpacing;
//        grpBackground.Top = currentY;
//        grpBackground.Width = halfWidth;
//        ResizeGroupBoxToFitContents(grpBackground);

//        int fgWidth = grpForeground.Width;
//        int bgWidth = grpBackground.Width;
//        int maxWidth = Math.Min(Math.Max(fgWidth, bgWidth), halfWidth);
//        grpForeground.Width = maxWidth;
//        grpBackground.Width = maxWidth;
//        grpBackground.Left = leftX + maxWidth + columnSpacing;

//        currentY = Math.Max(grpForeground.Bottom, grpBackground.Bottom) + topSectionSpacing;

//        grpTextSettings.Left = leftX;
//        grpTextSettings.Top = currentY;
//        grpTextSettings.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpTextSettings);
//        currentY = grpTextSettings.Bottom + topSectionSpacing;

//        pnlPreview.Left = leftX;
//        pnlPreview.Top = currentY;
//        pnlPreview.Width = availableWidth;
//        currentY = pnlPreview.Bottom + topSectionSpacing;

//        lblContrastRatio.Left = leftX;
//        lblContrastRatio.Top = currentY;
//        currentY = lblContrastRatio.Bottom + topSectionSpacing;

//        lblCompliance.Left = leftX;
//        lblCompliance.Top = currentY;
//        currentY = lblCompliance.Bottom + topSectionSpacing;

//        btnSavePalette.Left = leftX;
//        btnSavePalette.Top = currentY;
//        btnLoadPalette.Left = leftX + availableWidth - btnLoadPalette.Width;
//        btnLoadPalette.Top = currentY;
//    }

//    /// <summary>
//    /// Resizes a group box to fit its contained controls by increasing its height.
//    /// </summary>
//    public static void ResizeGroupBoxToFitContents(GroupBox groupBox)
//    {
//        int maxBottom = 0;
//        foreach (Control c in groupBox.Controls)
//        {
//            if (c.Bottom > maxBottom) maxBottom = c.Bottom;
//        }

//        int padding = 20;
//        groupBox.Height = maxBottom + padding;
//    }
//}


///// <summary>
///// Provides layout logic separated into its own class for cleaner architecture.
///// Handles scaling and layout adjustments for stacked or wide mode.
///// </summary>
//public static class LayoutService
//{
//    /// <summary>
//    /// Recursively scales all controls based on the provided scale factors.
//    /// Avoids scaling inside certain containers to prevent complexity.
//    /// </summary>
//    /// <param name="parent">The parent control to scale.</param>
//    /// <param name="scaleX">The horizontal scale factor.</param>
//    /// <param name="scaleY">The vertical scale factor.</param>
//    /// <param name="originalMetrics">A dictionary mapping each control to its original location and size.</param>
//    /// <param name="grpForeground">The foreground group box, whose children are not scaled.</param>
//    /// <param name="grpBackground">The background group box, whose children are not scaled.</param>
//    public static void ScaleAllControls(
//        Control parent,
//        float scaleX,
//        float scaleY,
//        Dictionary<Control, (Point originalLocation, Size originalSize)> originalMetrics,
//        Control grpForeground,
//        Control grpBackground,
//        Control grpTextSettings)
//    {
//        foreach (Control ctrl in parent.Controls)
//        {
//            if (originalMetrics.TryGetValue(ctrl, out var metrics))
//            {
//                int newX = (int)(metrics.originalLocation.X * scaleX);
//                int newY = (int)(metrics.originalLocation.Y * scaleY);
//                int newWidth = (int)(metrics.originalSize.Width * scaleX);
//                int newHeight = (int)(metrics.originalSize.Height * scaleY);

//                ctrl.Location = new Point(newX, newY);
//                ctrl.Size = new Size(newWidth, newHeight);
//            }

//            // Avoid scaling the children of certain complex group boxes to prevent layout issues.
//            // Now also exclude grpTextSettings to prevent overlap of labels and numeric/text controls.
//            if (ctrl != grpForeground && ctrl != grpBackground && ctrl != grpTextSettings && ctrl.HasChildren)
//            {
//                ScaleAllControls(ctrl, scaleX, scaleY, originalMetrics, grpForeground, grpBackground, grpTextSettings);
//            }
//        }
//    }

//    /// <summary>
//    /// Adjusts layout for either stacked or wide mode, positioning controls appropriately.
//    /// </summary>
//    /// <param name="form">The main form reference.</param>
//    /// <param name="pnlContainer">The container panel for all controls.</param>
//    /// <param name="picLogo">The picture box control for the logo.</param>
//    /// <param name="lblHeading">The heading label control.</param>
//    /// <param name="lblExplanationHeading">The explanation heading label.</param>
//    /// <param name="txtExplanation">The explanation text box control.</param>
//    /// <param name="grpForeground">The foreground settings group box.</param>
//    /// <param name="grpBackground">The background settings group box.</param>
//    /// <param name="grpTextSettings">The text settings group box.</param>
//    /// <param name="pnlPreview">The preview panel control.</param>
//    /// <param name="lblContrastRatio">The contrast ratio label.</param>
//    /// <param name="lblCompliance">The compliance label.</param>
//    /// <param name="btnSavePalette">The button to save the palette.</param>
//    /// <param name="btnLoadPalette">The button to load a palette.</param>
//    public static void AdjustLayoutForMode(
//        Form form,
//        Panel pnlContainer,
//        PictureBox picLogo,
//        Label lblHeading,
//        Label lblExplanationHeading,
//        TextBox txtExplanation,
//        GroupBox grpForeground,
//        GroupBox grpBackground,
//        GroupBox grpTextSettings,
//        Panel pnlPreview,
//        Label lblContrastRatio,
//        Label lblCompliance,
//        Button btnSavePalette,
//        Button btnLoadPalette)
//    {
//        int breakpoint = 800;
//        int topSectionSpacing = 10;
//        int leftX = 20;
//        int currentY = 20;
//        int rightMargin = 20;
//        int containerWidth = pnlContainer.ClientSize.Width;
//        int availableWidth = containerWidth - leftX - rightMargin;
//        availableWidth = Math.Max(availableWidth, 50);

//        form.AutoScroll = false;
//        form.HorizontalScroll.Enabled = false;
//        form.HorizontalScroll.Visible = false;

//        float lineHeight = lblHeading.Font.GetHeight();

//        if (form.Width < breakpoint)
//        {
//            // Stacked mode layout
//            LayoutStackedMode(
//                picLogo, lblHeading, lblExplanationHeading, txtExplanation,
//                grpForeground, grpBackground, grpTextSettings,
//                pnlPreview, lblContrastRatio, lblCompliance, btnSavePalette, btnLoadPalette,
//                lineHeight, availableWidth, leftX, topSectionSpacing, ref currentY);
//        }
//        else
//        {
//            // Wide mode layout
//            LayoutWideMode(
//                lblHeading, lblExplanationHeading, txtExplanation,
//                grpForeground, grpBackground, grpTextSettings,
//                pnlPreview, lblContrastRatio, lblCompliance, btnSavePalette, btnLoadPalette,
//                picLogo, availableWidth, leftX, topSectionSpacing, ref currentY);
//        }

//        // After layout, ensure controls do not exceed available width
//        EnsureNoHorizontalOverflow(pnlContainer, availableWidth);
//    }

//    /// <summary>
//    /// Finalizes layout after adjusting controls, setting scrollbars and ensuring the heading is visible.
//    /// </summary>
//    /// <param name="form">The main form instance.</param>
//    /// <param name="pnlContainer">The container panel of all controls.</param>
//    /// <param name="lblHeading">The heading label control to ensure visibility.</param>
//    public static void FinalizeLayout(Form form, Panel pnlContainer, Label lblHeading)
//    {
//        int maxRight = 0;
//        int maxBottom = 0;
//        foreach (Control c in pnlContainer.Controls)
//        {
//            if (c.Visible)
//            {
//                if (c.Right > maxRight) maxRight = c.Right;
//                if (c.Bottom > maxBottom) maxBottom = c.Bottom;
//            }
//        }

//        maxRight += 20;
//        maxBottom += 50;

//        pnlContainer.AutoScrollPosition = new Point(0, 0);

//        if (maxRight <= pnlContainer.ClientSize.Width)
//        {
//            pnlContainer.AutoScrollMinSize = new Size(0, maxBottom);
//            pnlContainer.HorizontalScroll.Enabled = false;
//            pnlContainer.HorizontalScroll.Visible = false;
//            pnlContainer.HorizontalScroll.Value = 0;
//        }
//        else
//        {
//            pnlContainer.AutoScrollMinSize = new Size(maxRight, maxBottom);
//            pnlContainer.HorizontalScroll.Enabled = true;
//            pnlContainer.HorizontalScroll.Visible = true;
//        }

//        pnlContainer.AutoScroll = true;
//        pnlContainer.PerformLayout();
//        pnlContainer.Invalidate();
//        pnlContainer.Update();

//        if (pnlContainer.HorizontalScroll.Visible && maxRight <= pnlContainer.ClientSize.Width)
//        {
//            pnlContainer.HorizontalScroll.Visible = false;
//            pnlContainer.HorizontalScroll.Enabled = false;
//            pnlContainer.HorizontalScroll.Value = 0;
//            pnlContainer.AutoScrollMinSize = new Size(0, maxBottom);
//            pnlContainer.PerformLayout();
//            pnlContainer.Invalidate();
//            pnlContainer.Update();
//        }

//        float lineHeight = lblHeading.Font.GetHeight();
//        if (lblHeading.Width < 1 || lblHeading.Height < 1)
//        {
//            lblHeading.Width = 200;
//            lblHeading.Height = (int)(lineHeight * 2);
//        }
//        lblHeading.Visible = true;

//        form.AutoScroll = false;
//        form.HorizontalScroll.Visible = false;
//        form.HorizontalScroll.Enabled = false;
//        form.HorizontalScroll.Value = 0;
//        form.PerformLayout();
//        form.Invalidate();
//        form.Update();

//        // Ensure no horizontal overflow after final layout
//        EnsureNoHorizontalOverflow(pnlContainer, pnlContainer.ClientSize.Width - 20);
//    }

//    /// <summary>
//    /// Ensures that no control exceeds the given available width, adjusting their widths if necessary.
//    /// This helps prevent horizontal overflow and unwanted scrollbars.
//    /// </summary>
//    /// <param name="pnlContainer">The container panel.</param>
//    /// <param name="availableWidth">The maximum allowed width.</param>
//    private static void EnsureNoHorizontalOverflow(Panel pnlContainer, int availableWidth)
//    {
//        foreach (Control c in pnlContainer.Controls)
//        {
//            if (c.Right > pnlContainer.ClientSize.Width)
//            {
//                // If any control exceeds the width, trim it
//                int overflow = c.Right - pnlContainer.ClientSize.Width;
//                if (overflow > 0 && c.Width - overflow > 0)
//                {
//                    c.Width = c.Width - overflow;
//                }

//                if (c.Width > availableWidth)
//                {
//                    c.Width = availableWidth;
//                }
//            }
//        }
//        pnlContainer.PerformLayout();
//        pnlContainer.Invalidate();
//        pnlContainer.Update();
//    }

//    /// <summary>
//    /// Called after the user finishes resizing the form, to re-check layout and scrollbars.
//    /// </summary>
//    /// <param name="form">The main form instance.</param>
//    /// <param name="pnlContainer">The panel container of all controls.</param>
//    public static void AfterResizeEnd(Form form, Panel pnlContainer)
//    {
//        pnlContainer.AutoScrollPosition = new Point(0, 0);
//        pnlContainer.PerformLayout();
//        form.PerformLayout();

//        int maxRight = 0;
//        foreach (Control c in pnlContainer.Controls)
//        {
//            if (c.Visible && c.Right > maxRight)
//                maxRight = c.Right;
//        }

//        if (maxRight <= pnlContainer.ClientSize.Width)
//        {
//            pnlContainer.AutoScrollMinSize = new Size(0, pnlContainer.AutoScrollMinSize.Height);
//            pnlContainer.HorizontalScroll.Visible = false;
//            pnlContainer.HorizontalScroll.Enabled = false;
//            pnlContainer.HorizontalScroll.Value = 0;
//            pnlContainer.PerformLayout();
//            pnlContainer.Invalidate();
//            pnlContainer.Update();
//        }

//        form.AutoScroll = false;
//        form.HorizontalScroll.Visible = false;
//        form.HorizontalScroll.Enabled = false;
//        form.HorizontalScroll.Value = 0;
//        form.PerformLayout();
//        form.Invalidate();
//        form.Update();
//    }

//    /// <summary>
//    /// Lays out controls in stacked mode, positioning logo, heading, explanation, groups, and preview in a single column.
//    /// </summary>
//    /// <param name="picLogo">The logo picture box.</param>
//    /// <param name="lblHeading">The heading label.</param>
//    /// <param name="lblExplanationHeading">The explanation heading label.</param>
//    /// <param name="txtExplanation">The explanation text box.</param>
//    /// <param name="grpForeground">The foreground settings group box.</param>
//    /// <param name="grpBackground">The background settings group box.</param>
//    /// <param name="grpTextSettings">The text settings group box.</param>
//    /// <param name="pnlPreview">The preview panel.</param>
//    /// <param name="lblContrastRatio">The contrast ratio label.</param>
//    /// <param name="lblCompliance">The compliance label.</param>
//    /// <param name="btnSavePalette">The save palette button.</param>
//    /// <param name="btnLoadPalette">The load palette button.</param>
//    /// <param name="lineHeight">The line height of the heading font for scaling.</param>
//    /// <param name="availableWidth">The available width for layout.</param>
//    /// <param name="leftX">The left X coordinate start for placing controls.</param>
//    /// <param name="topSectionSpacing">The vertical spacing between sections.</param>
//    /// <param name="currentY">A reference to the current Y position to place controls.</param>
//    private static void LayoutStackedMode(
//        PictureBox picLogo,
//        Label lblHeading,
//        Label lblExplanationHeading,
//        TextBox txtExplanation,
//        GroupBox grpForeground,
//        GroupBox grpBackground,
//        GroupBox grpTextSettings,
//        Panel pnlPreview,
//        Label lblContrastRatio,
//        Label lblCompliance,
//        Button btnSavePalette,
//        Button btnLoadPalette,
//        float lineHeight,
//        int availableWidth,
//        int leftX,
//        int topSectionSpacing,
//        ref int currentY)
//    {
//        lblHeading.AutoSize = true;
//        lblHeading.MaximumSize = new Size(availableWidth, 0);
//        lblHeading.PerformLayout();

//        Size pref = lblHeading.PreferredSize;
//        if (pref.Width < 1 || pref.Height < 1)
//        {
//            lblHeading.AutoSize = false;
//            lblHeading.MaximumSize = Size.Empty;
//            lblHeading.Size = new Size(100, (int)(lineHeight * 2));
//        }

//        // Position the logo
//        picLogo.Left = leftX;
//        picLogo.Top = 20;

//        // Position heading to the right of the logo, vertically centered
//        lblHeading.Left = picLogo.Right + 10;
//        lblHeading.Top = picLogo.Top + (picLogo.Height - lblHeading.Height) / 2;

//        // Advance currentY below both logo and heading
//        currentY = Math.Max(picLogo.Bottom, lblHeading.Bottom) + topSectionSpacing;

//        lblExplanationHeading.Left = leftX;
//        lblExplanationHeading.Top = currentY;
//        currentY = lblExplanationHeading.Bottom + topSectionSpacing;

//        txtExplanation.Left = leftX;
//        txtExplanation.Top = currentY;
//        txtExplanation.Width = availableWidth;
//        txtExplanation.Height = 100;
//        currentY = txtExplanation.Bottom + topSectionSpacing;

//        grpForeground.Left = leftX;
//        grpForeground.Top = currentY;
//        grpForeground.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpForeground);
//        currentY = grpForeground.Bottom + topSectionSpacing;

//        grpBackground.Left = leftX;
//        grpBackground.Top = currentY;
//        grpBackground.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpBackground);
//        currentY = grpBackground.Bottom + topSectionSpacing;

//        grpTextSettings.Left = leftX;
//        grpTextSettings.Top = currentY;
//        grpTextSettings.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpTextSettings);
//        currentY = grpTextSettings.Bottom + topSectionSpacing;

//        pnlPreview.Left = leftX;
//        pnlPreview.Top = currentY;
//        pnlPreview.Width = availableWidth;
//        currentY = pnlPreview.Bottom + topSectionSpacing;

//        lblContrastRatio.Left = leftX;
//        lblContrastRatio.Top = currentY;
//        currentY = lblContrastRatio.Bottom + topSectionSpacing;

//        lblCompliance.Left = leftX;
//        lblCompliance.Top = currentY;
//        currentY = lblCompliance.Bottom + topSectionSpacing;

//        btnSavePalette.Left = leftX;
//        btnSavePalette.Top = currentY;
//        btnLoadPalette.Left = leftX + availableWidth - btnLoadPalette.Width;
//        btnLoadPalette.Top = currentY;
//    }

//    /// <summary>
//    /// Lays out controls in wide mode, positioning logo, heading side-by-side, and other controls in two columns.
//    /// </summary>
//    /// <param name="lblHeading">The heading label.</param>
//    /// <param name="lblExplanationHeading">The explanation heading label.</param>
//    /// <param name="txtExplanation">The explanation text box.</param>
//    /// <param name="grpForeground">The foreground settings group box.</param>
//    /// <param name="grpBackground">The background settings group box.</param>
//    /// <param name="grpTextSettings">The text settings group box.</param>
//    /// <param name="pnlPreview">The preview panel.</param>
//    /// <param name="lblContrastRatio">The contrast ratio label.</param>
//    /// <param name="lblCompliance">The compliance label.</param>
//    /// <param name="btnSavePalette">The save palette button.</param>
//    /// <param name="btnLoadPalette">The load palette button.</param>
//    /// <param name="picLogo">The logo picture box.</param>
//    /// <param name="availableWidth">The available width for layout.</param>
//    /// <param name="leftX">The left X coordinate start for placing controls.</param>
//    /// <param name="topSectionSpacing">The vertical spacing between sections.</param>
//    /// <param name="currentY">A reference to the current Y position to place controls.</param>
//    private static void LayoutWideMode(
//        Label lblHeading,
//        Label lblExplanationHeading,
//        TextBox txtExplanation,
//        GroupBox grpForeground,
//        GroupBox grpBackground,
//        GroupBox grpTextSettings,
//        Panel pnlPreview,
//        Label lblContrastRatio,
//        Label lblCompliance,
//        Button btnSavePalette,
//        Button btnLoadPalette,
//        PictureBox picLogo,
//        int availableWidth,
//        int leftX,
//        int topSectionSpacing,
//        ref int currentY)
//    {
//        // Position the logo first
//        picLogo.Left = leftX;
//        picLogo.Top = currentY;
//        currentY = picLogo.Bottom + topSectionSpacing;

//        // Configure heading in wide mode
//        lblHeading.AutoSize = false;
//        lblHeading.MaximumSize = Size.Empty;
//        int headingWidth = Math.Max(availableWidth, 100);
//        lblHeading.Width = headingWidth;

//        // Center lblHeading vertically relative to picLogo
//        int verticalCenter = picLogo.Top + (picLogo.Height - lblHeading.Height) / 2;
//        lblHeading.Left = picLogo.Right + 10;
//        lblHeading.Top = verticalCenter;

//        // Ensure currentY accommodates both picLogo and lblHeading
//        currentY = Math.Max(picLogo.Bottom, lblHeading.Bottom) + topSectionSpacing;

//        lblExplanationHeading.AutoSize = true;
//        lblExplanationHeading.MaximumSize = new Size(availableWidth, 0);
//        lblExplanationHeading.PerformLayout();
//        lblExplanationHeading.Left = leftX;
//        lblExplanationHeading.Top = currentY;
//        currentY = lblExplanationHeading.Bottom + topSectionSpacing;

//        txtExplanation.Left = leftX;
//        txtExplanation.Top = currentY;
//        txtExplanation.Width = availableWidth;
//        currentY = txtExplanation.Bottom + topSectionSpacing;

//        int columnSpacing = 20;
//        int halfWidth = (availableWidth - columnSpacing) / 2;
//        halfWidth = Math.Max(halfWidth, 50);

//        grpForeground.Left = leftX;
//        grpForeground.Top = currentY;
//        grpForeground.Width = halfWidth;
//        ResizeGroupBoxToFitContents(grpForeground);

//        grpBackground.Left = leftX + halfWidth + columnSpacing;
//        grpBackground.Top = currentY;
//        grpBackground.Width = halfWidth;
//        ResizeGroupBoxToFitContents(grpBackground);

//        // Make widths symmetrical if needed
//        int fgWidth = grpForeground.Width;
//        int bgWidth = grpBackground.Width;
//        int maxWidth = Math.Min(Math.Max(fgWidth, bgWidth), halfWidth);
//        grpForeground.Width = maxWidth;
//        grpBackground.Width = maxWidth;
//        grpBackground.Left = leftX + maxWidth + columnSpacing;

//        currentY = Math.Max(grpForeground.Bottom, grpBackground.Bottom) + topSectionSpacing;

//        grpTextSettings.Left = leftX;
//        grpTextSettings.Top = currentY;
//        grpTextSettings.Width = availableWidth;
//        ResizeGroupBoxToFitContents(grpTextSettings);
//        currentY = grpTextSettings.Bottom + topSectionSpacing;

//        pnlPreview.Left = leftX;
//        pnlPreview.Top = currentY;
//        pnlPreview.Width = availableWidth;
//        currentY = pnlPreview.Bottom + topSectionSpacing;

//        lblContrastRatio.Left = leftX;
//        lblContrastRatio.Top = currentY;
//        currentY = lblContrastRatio.Bottom + topSectionSpacing;

//        lblCompliance.Left = leftX;
//        lblCompliance.Top = currentY;
//        currentY = lblCompliance.Bottom + topSectionSpacing;

//        btnSavePalette.Left = leftX;
//        btnSavePalette.Top = currentY;
//        btnLoadPalette.Left = leftX + availableWidth - btnLoadPalette.Width;
//        btnLoadPalette.Top = currentY;
//    }

//    /// <summary>
//    /// Resizes a group box to fit its contained controls by increasing its height to accommodate the tallest child control.
//    /// </summary>
//    /// <param name="groupBox">The group box to resize.</param>
//    public static void ResizeGroupBoxToFitContents(GroupBox groupBox)
//    {
//        int maxBottom = 0;
//        foreach (Control c in groupBox.Controls)
//        {
//            int bottom = c.Bottom;
//            if (bottom > maxBottom)
//                maxBottom = bottom;
//        }

//        int padding = 20;
//        groupBox.Height = maxBottom + padding;
//    }
//}
