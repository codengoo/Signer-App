using Microsoft.AspNetCore.Builder;
using System.Diagnostics;

namespace SignerUI
{
    public partial class Main : Form
    {
        private readonly ToolStripMenuItem startMenuItem = new("Bắt đầu");
        private readonly ToolStripMenuItem stopMenuItem = new("Kết thúc");
        private readonly ToolStripMenuItem killMenuItem = new("Thoát");
        private readonly string HostURL;
        private readonly WebApplication webApp;
        private readonly Task runTask;
        private bool isActionRunning = false;

        private void UpdateMenuState(bool running)
        {
            isActionRunning = running;
            startMenuItem.Enabled = !running;
            stopMenuItem.Enabled = !running;
        }

        public Main(WebApplication app, string Host, Task runTask)
        {
            HostURL = Host;
            webApp = app;
            this.runTask = runTask;
            InitializeComponent();
            Hide();
            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;

            try
            {
                startMenuItem.Click += new EventHandler(StartMenuItem_Click!);
                stopMenuItem.Click += new EventHandler(StopMenuItem_Click!);
                killMenuItem.Click += new EventHandler(KillMenuItem_Click!);

                contextMenuStrip.Items.Add(startMenuItem);
                contextMenuStrip.Items.Add(stopMenuItem);
                contextMenuStrip.Items.Add(new ToolStripSeparator());
                contextMenuStrip.Items.Add(killMenuItem);

                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "app_icon.ico");
                notifyIcon.Icon = new Icon(iconPath);
                notifyIcon.ContextMenuStrip = contextMenuStrip;
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Không tìm thấy file icon: " + ex.Message, "Lỗi Icon", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Startup startup = new Startup(HostURL);
            startup.ShowDialog();
        }

        private void StartMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Đã click 'Bắt đầu'", "Hành động", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Debug.WriteLine("Hành động BẮT ĐẦU được kích hoạt.");
        }

        private async void StopMenuItem_Click(object sender, EventArgs e)
        {
            UpdateMenuState(true);

            try
            {
                await Task.Run(() => SignerAPI.ApiHost.StopApp(webApp, runTask));
                MessageBox.Show("Đã dừng dịch vụ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình dừng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UpdateMenuState(false);
            }
        }

        private void KillMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
