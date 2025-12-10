namespace SignerUI
{
    partial class Startup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            messageText = new Label();
            OkBtn = new Button();
            var mainLayout = new TableLayoutPanel();
            var bottomFlow = new FlowLayoutPanel();

            SuspendLayout();

            // 
            // mainLayout
            // 
            mainLayout.ColumnCount = 1;
            mainLayout.RowCount = 3;
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Padding = new Padding(10);
            mainLayout.AutoSize = true;
            mainLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainLayout.ColumnStyles.Clear();
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            mainLayout.RowStyles.Clear();
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // message row
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 8F)); // spacer
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // bottom (button) row

            // 
            // messageText
            // 
            // Allow wrapping by limiting maximum width and let the label auto-size vertically.
            messageText.AutoSize = true;
            messageText.Name = "messageText";
            messageText.TabIndex = 0;
            messageText.Text = " "; // placeholder so form has a visible design-time size
            messageText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            messageText.MaximumSize = new System.Drawing.Size(600, 0); // wrap if long
            messageText.Anchor = AnchorStyles.Top;
            messageText.Margin = new Padding(0);

            // Put message label into the single column, top row
            mainLayout.Controls.Add(messageText, 0, 0);

            // 
            // bottomFlow
            // 
            bottomFlow.FlowDirection = FlowDirection.RightToLeft;
            bottomFlow.Dock = DockStyle.Fill;
            bottomFlow.AutoSize = true;
            bottomFlow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            bottomFlow.Margin = new Padding(0);

            // 
            // OkBtn
            // 
            OkBtn.Name = "OkBtn";
            OkBtn.Size = new System.Drawing.Size(75, 23);
            OkBtn.TabIndex = 1;
            OkBtn.Text = "Finish";
            OkBtn.UseMnemonic = false;
            OkBtn.UseVisualStyleBackColor = true;
            OkBtn.Click += this.BtnOpenLink_Click;
            OkBtn.Margin = new Padding(0);

            bottomFlow.Controls.Add(OkBtn);
            mainLayout.Controls.Add(bottomFlow, 0, 2);

            // 
            // Startup (Form)
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;

            // Make the form grow to fit message and button. MinimumSize ensures usability when no message set.
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new System.Drawing.Size(300, 140);
            MinimumSize = new System.Drawing.Size(300, 120);
            MaximumSize = new System.Drawing.Size(300, 600);

            Controls.Add(mainLayout);
            Name = "Startup";
            Text = "Thông báo";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label messageText;
        private Button OkBtn;
    }
}