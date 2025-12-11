using Microsoft.AspNetCore.Builder;
using Microsoft.Win32;
using SignerUI.Common;

namespace SignerUI
{
    public partial class Main : Form
    {
        private readonly ToolStripMenuItem aboutMenuItem = new("Về chúng tôi");
        private readonly ToolStripMenuItem startMenuItem = new("Khởi động dịch vụ");
        private readonly ToolStripMenuItem stopMenuItem = new("Dừng dịch vụ");
        private readonly ToolStripMenuItem exitMenuItem = new("Thoát");
        private readonly ToolStripMenuItem autoStartMenuItem = new("Khởi động cùng hệ thống", null, null, "AutoStartMenuItem")
        {
            CheckOnClick = true,
            Checked = false
        };

        private string? HostURL;
        private readonly WebApplication webApp;
        private Task? runTask;
        private ManualResetEventSlim? startupEvent = new ManualResetEventSlim(false);
        private bool IsRunning;

        private void UpdateMenuState(bool running)
        {
            startMenuItem.Enabled = !running;
            stopMenuItem.Enabled = !running;
        }

        public Main()
        {
            InitializeComponent();

            startMenuItem.Click += new EventHandler(StartMenuItem_Click!);
            stopMenuItem.Click += new EventHandler(StopMenuItem_Click!);
            exitMenuItem.Click += new EventHandler(KillMenuItem_Click!);
            autoStartMenuItem.Click += new EventHandler(AutoStartMenuItem_Click!);
            autoStartMenuItem.Checked = IsEnableAutoStart;

            startMenuItem.Image = Properties.Resources.start;
            stopMenuItem.Image = Properties.Resources.stop;

            contextMenuStrip.Items.Add(aboutMenuItem);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add(startMenuItem);
            contextMenuStrip.Items.Add(stopMenuItem);
            contextMenuStrip.Items.Add(autoStartMenuItem);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add(exitMenuItem);

            notifyIcon.Icon = Properties.Resources.app_icon;
            notifyIcon.ContextMenuStrip = contextMenuStrip;

            // Init
            webApp = SignerAPI.ApiHost.Create((urls) =>
            {
                HostURL = urls.First() ?? "";
                startupEvent.Set();
            });

            runTask = Task.Run(() => SignerAPI.ApiHost.StartApp(webApp));
            stopMenuItem.Visible = true;
            startMenuItem.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //if (startupEvent != null && startupEvent.Wait(10))
            //{
            //    Startup startup = new(HostURL ?? "");
            //    startup.ShowDialog();
            //}
        }

        private async void StartMenuItem_Click(object sender, EventArgs e)
        {
            UpdateMenuState(true);

            try
            {
                runTask = Task.Run(() => SignerAPI.ApiHost.StartApp(webApp));
                MessageBox.Show("Đã bắt đầu dịch vụ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                stopMenuItem.Visible = true;
                startMenuItem.Visible = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi trong quá trình khởi động: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UpdateMenuState(false);
            }
        }

        private async void StopMenuItem_Click(object sender, EventArgs e)
        {
            UpdateMenuState(true);

            try
            {
                startupEvent = null;
                await Task.Run(() => SignerAPI.ApiHost.StopApp(webApp, runTask));
                MessageBox.Show("Đã dừng dịch vụ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                startMenuItem.Visible = true;
                stopMenuItem.Visible = false;
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

        private void AutoStartMenuItem_Click(object sender, EventArgs e)
        {
            bool isChecked = autoStartMenuItem.Checked;

            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(Constants.RunPathRegistry, true) ??
                    throw new Exception("Không thể truy cập Registry.");

                string exePath = Application.ExecutablePath;

                if (isChecked)
                {
                    // Bật tự động khởi động
                    key.SetValue(Constants.AppName, $"\"{exePath}\"");
                }
                else
                {
                    // Tắt tự động khởi động
                    if (key.GetValue(Constants.AppName) != null)
                        key.DeleteValue(Constants.AppName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi ghi Registry: " + ex.Message);
                autoStartMenuItem.Checked = !isChecked;
            }
        }

        private static bool IsEnableAutoStart
        {
            get
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(Constants.RunPathRegistry);

                if (key != null)
                {
                    string? value = (string)key.GetValue(Constants.AppName);
                    return !string.IsNullOrEmpty(value);
                }

                return false;
            }
        }
    }
}
