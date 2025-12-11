using System.Diagnostics;

namespace SignerUI
{
    public partial class Startup : Form
    {
        private readonly string _targetUrl = "https://gemini.google.com/app";
        public Startup(string targetUrl)
        {
            InitializeComponent();
            _targetUrl = targetUrl;
            messageText.Text = "Ứng dụng khởi động thành công! Đang chạy tại: " + _targetUrl;

            string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "app_icon.ico");
            Icon = new Icon(iconPath);
        }

        private void BtnOpenLink_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(_targetUrl) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể mở liên kết: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.Close();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;

                const int WS_EX_TOOLWINDOW = 0x00000080;

                // Loại bỏ toolwindow → hiện lại Alt+Tab, TaskView
                cp.ExStyle &= ~WS_EX_TOOLWINDOW;

                return cp;
            }
        }
    }
}
