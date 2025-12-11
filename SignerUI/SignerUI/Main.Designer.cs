namespace SignerUI
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(components);
            contextMenuStrip = new ContextMenuStrip(components);
            SuspendLayout();
            // 
            // notifyIcon
            // 
            notifyIcon.Text = "Signer";
            notifyIcon.Visible = true;
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.Name = "contextMenuStrip1";
            contextMenuStrip.Size = new Size(181, 26);
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            Hide();

            Name = "Main";
            Text = "Main";
            Load += Form1_Load;

            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
    }
}
