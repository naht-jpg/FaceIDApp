namespace FaceIDApp
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                clockTimer?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        // InitializeComponent is now handled in MainForm.cs → InitializeMainForm()
        // This file is kept for Designer compatibility only.
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
        }
    }
}
