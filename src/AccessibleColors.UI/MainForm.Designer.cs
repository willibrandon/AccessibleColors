namespace AccessibleColors.UI
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            pnlContainer = new Panel();
            picLogo = new PictureBox();
            lblHeading = new Label();
            lblExplanationHeading = new Label();
            txtExplanation = new TextBox();
            grpForeground = new GroupBox();
            lblForegroundColor = new Label();
            txtForegroundColor = new TextBox();
            btnForegroundPicker = new Button();
            btnForegroundEyedropper = new Button();
            lblForegroundAlpha = new Label();
            numForegroundAlpha = new NumericUpDown();
            lblForegroundLightness = new Label();
            sliderForeground = new TrackBar();
            pnlForegroundSwatch = new Panel();
            grpBackground = new GroupBox();
            lblBackgroundAlpha = new Label();
            numBackgroundAlpha = new NumericUpDown();
            lblBackgroundColor = new Label();
            txtBackgroundColor = new TextBox();
            btnBackgroundPicker = new Button();
            btnBackgroundEyedropper = new Button();
            lblBackgroundLightness = new Label();
            sliderBackground = new TrackBar();
            pnlBackgroundSwatch = new Panel();
            grpTextSettings = new GroupBox();
            lblTextSize = new Label();
            numTextSize = new NumericUpDown();
            chkBold = new CheckBox();
            lblSampleText = new Label();
            txtSampleText = new TextBox();
            lblContrastRatio = new Label();
            lblCompliance = new Label();
            btnLoadPalette = new Button();
            btnSavePalette = new Button();
            pnlPreview = new DoubleBufferedPanel();
            lblPreviewText = new DoubleBufferedLabel();
            pnlContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            grpForeground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numForegroundAlpha).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sliderForeground).BeginInit();
            grpBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numBackgroundAlpha).BeginInit();
            ((System.ComponentModel.ISupportInitialize)sliderBackground).BeginInit();
            grpTextSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numTextSize).BeginInit();
            pnlPreview.SuspendLayout();
            SuspendLayout();
            // 
            // pnlContainer
            // 
            pnlContainer.AutoScroll = true;
            pnlContainer.Controls.Add(pnlPreview);
            pnlContainer.Controls.Add(picLogo);
            pnlContainer.Controls.Add(lblHeading);
            pnlContainer.Controls.Add(lblExplanationHeading);
            pnlContainer.Controls.Add(txtExplanation);
            pnlContainer.Controls.Add(grpForeground);
            pnlContainer.Controls.Add(grpBackground);
            pnlContainer.Controls.Add(grpTextSettings);
            pnlContainer.Controls.Add(lblContrastRatio);
            pnlContainer.Controls.Add(lblCompliance);
            pnlContainer.Controls.Add(btnLoadPalette);
            pnlContainer.Controls.Add(btnSavePalette);
            pnlContainer.Dock = DockStyle.Fill;
            pnlContainer.Location = new Point(0, 0);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.Size = new Size(984, 744);
            pnlContainer.TabIndex = 0;
            // 
            // picLogo
            // 
            picLogo.Image = Properties.Resources.icon_128x128;
            picLogo.InitialImage = (Image)resources.GetObject("picLogo.InitialImage");
            picLogo.Location = new Point(20, 15);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(64, 64);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            // 
            // lblHeading
            // 
            lblHeading.AutoSize = true;
            lblHeading.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblHeading.Location = new Point(100, 35);
            lblHeading.Name = "lblHeading";
            lblHeading.Size = new Size(674, 25);
            lblHeading.TabIndex = 1;
            lblHeading.Text = "AccessibleColors: WCAG-Compliant Contrast Colors for Inclusive UI Design";
            // 
            // lblExplanationHeading
            // 
            lblExplanationHeading.AutoSize = true;
            lblExplanationHeading.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblExplanationHeading.Location = new Point(20, 82);
            lblExplanationHeading.Name = "lblExplanationHeading";
            lblExplanationHeading.Size = new Size(102, 21);
            lblExplanationHeading.TabIndex = 2;
            lblExplanationHeading.Text = "Explanation";
            // 
            // txtExplanation
            // 
            txtExplanation.BorderStyle = BorderStyle.FixedSingle;
            txtExplanation.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtExplanation.Location = new Point(20, 112);
            txtExplanation.Multiline = true;
            txtExplanation.Name = "txtExplanation";
            txtExplanation.ReadOnly = true;
            txtExplanation.ScrollBars = ScrollBars.Vertical;
            txtExplanation.Size = new Size(950, 140);
            txtExplanation.TabIndex = 3;
            txtExplanation.TabStop = false;
            txtExplanation.Text = resources.GetString("txtExplanation.Text");
            // 
            // grpForeground
            // 
            grpForeground.Controls.Add(lblForegroundColor);
            grpForeground.Controls.Add(txtForegroundColor);
            grpForeground.Controls.Add(btnForegroundPicker);
            grpForeground.Controls.Add(btnForegroundEyedropper);
            grpForeground.Controls.Add(lblForegroundAlpha);
            grpForeground.Controls.Add(numForegroundAlpha);
            grpForeground.Controls.Add(lblForegroundLightness);
            grpForeground.Controls.Add(sliderForeground);
            grpForeground.Controls.Add(pnlForegroundSwatch);
            grpForeground.Location = new Point(20, 258);
            grpForeground.Name = "grpForeground";
            grpForeground.Size = new Size(480, 180);
            grpForeground.TabIndex = 4;
            grpForeground.TabStop = false;
            grpForeground.Text = "Foreground Settings";
            // 
            // lblForegroundColor
            // 
            lblForegroundColor.AutoSize = true;
            lblForegroundColor.Location = new Point(20, 30);
            lblForegroundColor.Name = "lblForegroundColor";
            lblForegroundColor.Size = new Size(101, 15);
            lblForegroundColor.TabIndex = 0;
            lblForegroundColor.Text = "Color (#RRGGBB):";
            // 
            // txtForegroundColor
            // 
            txtForegroundColor.Location = new Point(130, 28);
            txtForegroundColor.Name = "txtForegroundColor";
            txtForegroundColor.Size = new Size(80, 23);
            txtForegroundColor.TabIndex = 1;
            txtForegroundColor.Text = "#000000";
            txtForegroundColor.TextChanged += TxtForegroundColor_TextChanged;
            // 
            // btnForegroundPicker
            // 
            btnForegroundPicker.Location = new Point(220, 26);
            btnForegroundPicker.Name = "btnForegroundPicker";
            btnForegroundPicker.Size = new Size(90, 23);
            btnForegroundPicker.TabIndex = 2;
            btnForegroundPicker.Text = "Color Picker";
            btnForegroundPicker.UseVisualStyleBackColor = true;
            btnForegroundPicker.Click += BtnForegroundPicker_Click;
            // 
            // btnForegroundEyedropper
            // 
            btnForegroundEyedropper.Location = new Point(320, 26);
            btnForegroundEyedropper.Name = "btnForegroundEyedropper";
            btnForegroundEyedropper.Size = new Size(90, 23);
            btnForegroundEyedropper.TabIndex = 3;
            btnForegroundEyedropper.Text = "Eye Dropper";
            btnForegroundEyedropper.UseVisualStyleBackColor = true;
            btnForegroundEyedropper.Click += BtnForegroundEyedropper_Click;
            // 
            // lblForegroundAlpha
            // 
            lblForegroundAlpha.AutoSize = true;
            lblForegroundAlpha.Location = new Point(20, 70);
            lblForegroundAlpha.Name = "lblForegroundAlpha";
            lblForegroundAlpha.Size = new Size(81, 15);
            lblForegroundAlpha.TabIndex = 4;
            lblForegroundAlpha.Text = "Alpha (0-255):";
            // 
            // numForegroundAlpha
            // 
            numForegroundAlpha.Location = new Point(130, 68);
            numForegroundAlpha.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numForegroundAlpha.Name = "numForegroundAlpha";
            numForegroundAlpha.Size = new Size(80, 23);
            numForegroundAlpha.TabIndex = 5;
            numForegroundAlpha.Value = new decimal(new int[] { 255, 0, 0, 0 });
            numForegroundAlpha.ValueChanged += NumAlpha_ValueChanged;
            // 
            // lblForegroundLightness
            // 
            lblForegroundLightness.AutoSize = true;
            lblForegroundLightness.Location = new Point(20, 110);
            lblForegroundLightness.Name = "lblForegroundLightness";
            lblForegroundLightness.Size = new Size(60, 15);
            lblForegroundLightness.TabIndex = 6;
            lblForegroundLightness.Text = "Lightness:";
            // 
            // sliderForeground
            // 
            sliderForeground.Location = new Point(130, 100);
            sliderForeground.Maximum = 200;
            sliderForeground.Name = "sliderForeground";
            sliderForeground.Size = new Size(200, 45);
            sliderForeground.TabIndex = 7;
            sliderForeground.TickFrequency = 10;
            sliderForeground.Value = 100;
            sliderForeground.Scroll += SliderForeground_Scroll;
            // 
            // pnlForegroundSwatch
            // 
            pnlForegroundSwatch.BorderStyle = BorderStyle.FixedSingle;
            pnlForegroundSwatch.Location = new Point(350, 100);
            pnlForegroundSwatch.Name = "pnlForegroundSwatch";
            pnlForegroundSwatch.Size = new Size(40, 40);
            pnlForegroundSwatch.TabIndex = 8;
            // 
            // grpBackground
            // 
            grpBackground.Controls.Add(lblBackgroundAlpha);
            grpBackground.Controls.Add(numBackgroundAlpha);
            grpBackground.Controls.Add(lblBackgroundColor);
            grpBackground.Controls.Add(txtBackgroundColor);
            grpBackground.Controls.Add(btnBackgroundPicker);
            grpBackground.Controls.Add(btnBackgroundEyedropper);
            grpBackground.Controls.Add(lblBackgroundLightness);
            grpBackground.Controls.Add(sliderBackground);
            grpBackground.Controls.Add(pnlBackgroundSwatch);
            grpBackground.Location = new Point(520, 258);
            grpBackground.Name = "grpBackground";
            grpBackground.Size = new Size(450, 180);
            grpBackground.TabIndex = 5;
            grpBackground.TabStop = false;
            grpBackground.Text = "Background Settings";
            // 
            // lblBackgroundAlpha
            // 
            lblBackgroundAlpha.AutoSize = true;
            lblBackgroundAlpha.Location = new Point(20, 70);
            lblBackgroundAlpha.Name = "lblBackgroundAlpha";
            lblBackgroundAlpha.Size = new Size(81, 15);
            lblBackgroundAlpha.TabIndex = 7;
            lblBackgroundAlpha.Text = "Alpha (0-255):";
            // 
            // numBackgroundAlpha
            // 
            numBackgroundAlpha.Location = new Point(130, 68);
            numBackgroundAlpha.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numBackgroundAlpha.Name = "numBackgroundAlpha";
            numBackgroundAlpha.Size = new Size(80, 23);
            numBackgroundAlpha.TabIndex = 8;
            numBackgroundAlpha.Value = new decimal(new int[] { 255, 0, 0, 0 });
            numBackgroundAlpha.ValueChanged += NumBackgroundAlpha_ValueChanged;
            // 
            // lblBackgroundColor
            // 
            lblBackgroundColor.AutoSize = true;
            lblBackgroundColor.Location = new Point(20, 30);
            lblBackgroundColor.Name = "lblBackgroundColor";
            lblBackgroundColor.Size = new Size(101, 15);
            lblBackgroundColor.TabIndex = 0;
            lblBackgroundColor.Text = "Color (#RRGGBB):";
            // 
            // txtBackgroundColor
            // 
            txtBackgroundColor.Location = new Point(130, 28);
            txtBackgroundColor.Name = "txtBackgroundColor";
            txtBackgroundColor.Size = new Size(80, 23);
            txtBackgroundColor.TabIndex = 1;
            txtBackgroundColor.Text = "#FFFFFF";
            txtBackgroundColor.TextChanged += TxtBackgroundColor_TextChanged;
            // 
            // btnBackgroundPicker
            // 
            btnBackgroundPicker.Location = new Point(220, 26);
            btnBackgroundPicker.Name = "btnBackgroundPicker";
            btnBackgroundPicker.Size = new Size(90, 23);
            btnBackgroundPicker.TabIndex = 2;
            btnBackgroundPicker.Text = "Color Picker";
            btnBackgroundPicker.UseVisualStyleBackColor = true;
            btnBackgroundPicker.Click += BtnBackgroundPicker_Click;
            // 
            // btnBackgroundEyedropper
            // 
            btnBackgroundEyedropper.Location = new Point(320, 26);
            btnBackgroundEyedropper.Name = "btnBackgroundEyedropper";
            btnBackgroundEyedropper.Size = new Size(90, 23);
            btnBackgroundEyedropper.TabIndex = 3;
            btnBackgroundEyedropper.Text = "Eye Dropper";
            btnBackgroundEyedropper.UseVisualStyleBackColor = true;
            btnBackgroundEyedropper.Click += BtnBackgroundEyedropper_Click;
            // 
            // lblBackgroundLightness
            // 
            lblBackgroundLightness.AutoSize = true;
            lblBackgroundLightness.Location = new Point(20, 110);
            lblBackgroundLightness.Name = "lblBackgroundLightness";
            lblBackgroundLightness.Size = new Size(60, 15);
            lblBackgroundLightness.TabIndex = 4;
            lblBackgroundLightness.Text = "Lightness:";
            // 
            // sliderBackground
            // 
            sliderBackground.Location = new Point(130, 100);
            sliderBackground.Maximum = 200;
            sliderBackground.Name = "sliderBackground";
            sliderBackground.Size = new Size(200, 45);
            sliderBackground.TabIndex = 5;
            sliderBackground.TickFrequency = 10;
            sliderBackground.Value = 100;
            sliderBackground.Scroll += SliderBackground_Scroll;
            // 
            // pnlBackgroundSwatch
            // 
            pnlBackgroundSwatch.BorderStyle = BorderStyle.FixedSingle;
            pnlBackgroundSwatch.Location = new Point(350, 100);
            pnlBackgroundSwatch.Name = "pnlBackgroundSwatch";
            pnlBackgroundSwatch.Size = new Size(40, 40);
            pnlBackgroundSwatch.TabIndex = 6;
            // 
            // grpTextSettings
            // 
            grpTextSettings.Controls.Add(lblTextSize);
            grpTextSettings.Controls.Add(numTextSize);
            grpTextSettings.Controls.Add(chkBold);
            grpTextSettings.Controls.Add(lblSampleText);
            grpTextSettings.Controls.Add(txtSampleText);
            grpTextSettings.Location = new Point(20, 444);
            grpTextSettings.Name = "grpTextSettings";
            grpTextSettings.Size = new Size(950, 110);
            grpTextSettings.TabIndex = 6;
            grpTextSettings.TabStop = false;
            grpTextSettings.Text = "Text Settings";
            // 
            // lblTextSize
            // 
            lblTextSize.AutoSize = true;
            lblTextSize.Location = new Point(20, 30);
            lblTextSize.Name = "lblTextSize";
            lblTextSize.Size = new Size(76, 15);
            lblTextSize.TabIndex = 0;
            lblTextSize.Text = "Text Size (pt):";
            // 
            // numTextSize
            // 
            numTextSize.Location = new Point(110, 28);
            numTextSize.Maximum = new decimal(new int[] { 72, 0, 0, 0 });
            numTextSize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numTextSize.Name = "numTextSize";
            numTextSize.Size = new Size(60, 23);
            numTextSize.TabIndex = 1;
            numTextSize.Value = new decimal(new int[] { 14, 0, 0, 0 });
            numTextSize.ValueChanged += NumTextSize_ValueChanged;
            // 
            // chkBold
            // 
            chkBold.AutoSize = true;
            chkBold.Location = new Point(180, 30);
            chkBold.Name = "chkBold";
            chkBold.Size = new Size(50, 19);
            chkBold.TabIndex = 2;
            chkBold.Text = "Bold";
            chkBold.UseVisualStyleBackColor = true;
            chkBold.CheckedChanged += ChkBold_CheckedChanged;
            // 
            // lblSampleText
            // 
            lblSampleText.AutoSize = true;
            lblSampleText.Location = new Point(20, 70);
            lblSampleText.Name = "lblSampleText";
            lblSampleText.Size = new Size(73, 15);
            lblSampleText.TabIndex = 3;
            lblSampleText.Text = "Sample Text:";
            // 
            // txtSampleText
            // 
            txtSampleText.Location = new Point(110, 68);
            txtSampleText.Name = "txtSampleText";
            txtSampleText.Size = new Size(300, 23);
            txtSampleText.TabIndex = 4;
            txtSampleText.Text = "Sample Text";
            txtSampleText.TextChanged += TxtSampleText_TextChanged;
            // 
            // lblContrastRatio
            // 
            lblContrastRatio.AutoSize = true;
            lblContrastRatio.Location = new Point(22, 647);
            lblContrastRatio.Name = "lblContrastRatio";
            lblContrastRatio.Size = new Size(85, 15);
            lblContrastRatio.TabIndex = 8;
            lblContrastRatio.Text = "Contrast Ratio:";
            // 
            // lblCompliance
            // 
            lblCompliance.AutoSize = true;
            lblCompliance.Location = new Point(22, 677);
            lblCompliance.Name = "lblCompliance";
            lblCompliance.Size = new Size(74, 15);
            lblCompliance.TabIndex = 9;
            lblCompliance.Text = "Compliance:";
            // 
            // btnLoadPalette
            // 
            btnLoadPalette.Location = new Point(133, 711);
            btnLoadPalette.Name = "btnLoadPalette";
            btnLoadPalette.Size = new Size(100, 23);
            btnLoadPalette.TabIndex = 13;
            btnLoadPalette.Text = "Load Palette";
            btnLoadPalette.UseVisualStyleBackColor = true;
            btnLoadPalette.Click += BtnLoadPalette_Click;
            // 
            // btnSavePalette
            // 
            btnSavePalette.Location = new Point(23, 711);
            btnSavePalette.Name = "btnSavePalette";
            btnSavePalette.Size = new Size(100, 23);
            btnSavePalette.TabIndex = 12;
            btnSavePalette.Text = "Save Palette";
            btnSavePalette.UseVisualStyleBackColor = true;
            btnSavePalette.Click += BtnSavePalette_Click;
            // 
            // pnlPreview
            // 
            pnlPreview.BorderStyle = BorderStyle.FixedSingle;
            pnlPreview.Controls.Add(lblPreviewText);
            pnlPreview.Location = new Point(20, 560);
            pnlPreview.Name = "pnlPreview";
            pnlPreview.Size = new Size(950, 80);
            pnlPreview.TabIndex = 14;
            // 
            // lblPreviewText
            // 
            lblPreviewText.Dock = DockStyle.Fill;
            lblPreviewText.Location = new Point(0, 0);
            lblPreviewText.Name = "lblPreviewText";
            lblPreviewText.Size = new Size(948, 78);
            lblPreviewText.TabIndex = 0;
            lblPreviewText.Text = "Sample Text";
            lblPreviewText.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            AllowDrop = true;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 744);
            Controls.Add(pnlContainer);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            Text = "AccessibleColors";
            pnlContainer.ResumeLayout(false);
            pnlContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            grpForeground.ResumeLayout(false);
            grpForeground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numForegroundAlpha).EndInit();
            ((System.ComponentModel.ISupportInitialize)sliderForeground).EndInit();
            grpBackground.ResumeLayout(false);
            grpBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numBackgroundAlpha).EndInit();
            ((System.ComponentModel.ISupportInitialize)sliderBackground).EndInit();
            grpTextSettings.ResumeLayout(false);
            grpTextSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numTextSize).EndInit();
            pnlPreview.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label lblExplanationHeading;
        private System.Windows.Forms.TextBox txtExplanation;
        private System.Windows.Forms.GroupBox grpForeground;
        private System.Windows.Forms.Label lblForegroundColor;
        private System.Windows.Forms.TextBox txtForegroundColor;
        private System.Windows.Forms.Button btnForegroundPicker;
        private System.Windows.Forms.Button btnForegroundEyedropper;
        private System.Windows.Forms.Label lblForegroundAlpha;
        private System.Windows.Forms.NumericUpDown numForegroundAlpha;
        private System.Windows.Forms.Label lblForegroundLightness;
        private System.Windows.Forms.TrackBar sliderForeground;
        private System.Windows.Forms.Panel pnlForegroundSwatch;
        private System.Windows.Forms.GroupBox grpBackground;
        private System.Windows.Forms.Label lblBackgroundAlpha;
        private System.Windows.Forms.NumericUpDown numBackgroundAlpha;
        private System.Windows.Forms.Label lblBackgroundColor;
        private System.Windows.Forms.TextBox txtBackgroundColor;
        private System.Windows.Forms.Button btnBackgroundPicker;
        private System.Windows.Forms.Button btnBackgroundEyedropper;
        private System.Windows.Forms.Label lblBackgroundLightness;
        private System.Windows.Forms.TrackBar sliderBackground;
        private System.Windows.Forms.Panel pnlBackgroundSwatch;
        private System.Windows.Forms.GroupBox grpTextSettings;
        private System.Windows.Forms.Label lblTextSize;
        private System.Windows.Forms.NumericUpDown numTextSize;
        private System.Windows.Forms.CheckBox chkBold;
        private System.Windows.Forms.Label lblSampleText;
        private System.Windows.Forms.TextBox txtSampleText;
        private System.Windows.Forms.Label lblContrastRatio;
        private System.Windows.Forms.Label lblCompliance;
        private System.Windows.Forms.Button btnSavePalette;
        private System.Windows.Forms.Button btnLoadPalette;
        private System.Windows.Forms.Panel pnlContainer;
        private DoubleBufferedPanel pnlPreview;
        private DoubleBufferedLabel lblPreviewText;
    }
}
