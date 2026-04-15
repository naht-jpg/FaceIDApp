namespace FaceIDApp.UserControls
{
    partial class UCFaceRegistration
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            _glowTimer?.Stop();
            _glowTimer?.Dispose();
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
        }
    }
}
