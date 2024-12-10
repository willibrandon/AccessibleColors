namespace AccessibleColors.UI
{
    partial class EyedropperOverlay
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = 0.7;
            this.BackColor = Color.Black;
            this.WindowState = FormWindowState.Maximized;
        }
    }
}