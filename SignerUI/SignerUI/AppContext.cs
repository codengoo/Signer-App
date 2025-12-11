using Microsoft.AspNetCore.Builder;
using Microsoft.Win32;
using SignerUI.Common;
using SignerUI.Views;

namespace SignerUI
{
    public class AppContext : ApplicationContext
    {
        private readonly NotifyIcon notifyIcon;
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
        private WebApplication? webApp;
        private Task? runTask;
        private ManualResetEventSlim startupEvent = new(false);

        private void UpdateMenuState(bool running)
        {
            startMenuItem.Enabled = !running;
            stopMenuItem.Enabled = !running;
        }

        public AppContext()
        {
            startMenuItem.Click += new EventHandler(StartMenuItem_Click!);
            stopMenuItem.Click += new EventHandler(StopMenuItem_Click!);
            exitMenuItem.Click += new EventHandler(KillMenuItem_Click!);
            autoStartMenuItem.Click += new EventHandler(AutoStartMenuItem_Click!);
            aboutMenuItem.Click += new EventHandler(About_Click!);
            autoStartMenuItem.Checked = IsEnableAutoStart;

            startMenuItem.Image = Properties.Resources.start;
            stopMenuItem.Image = Properties.Resources.stop;
            
            stopMenuItem.Visible = true;
            startMenuItem.Visible = false;

            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add(aboutMenuItem);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add(startMenuItem);
            contextMenuStrip.Items.Add(stopMenuItem);
            contextMenuStrip.Items.Add(autoStartMenuItem);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add(exitMenuItem);

            notifyIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.app_icon,
                Visible = true,
                ContextMenuStrip = contextMenuStrip,
                Text = "Signer"
            };

            // Init
            StartService();
            ShowServiceInfo();
        }

        private void StartService()
        {
            webApp = SignerAPI.ApiHost.Create((urls) =>
            {
                HostURL = urls.First() ?? "";
                startupEvent.Set();
            });
            runTask = Task.Run(() => webApp.Run());
        }

        private async void StopService()
        {
            if (webApp == null) throw new Exception("No web app service instance!");
            await Task.Run(() => SignerAPI.ApiHost.StopApp(webApp, runTask));

            // Reset
            startupEvent = new(false);
            webApp = null;
            runTask = null;
        }

        private void ShowServiceInfo()
        {
            if (startupEvent.Wait(10))
            {
                Startup startup = new(HostURL ?? "");
                startup.ShowDialog();
            }
        }

        private async void StartMenuItem_Click(object sender, EventArgs e)
        {
            UpdateMenuState(true);

            try
            {
                StartService();
                ShowServiceInfo();

                // UI
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
                StopService();

                // UI
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

        private void About_Click(object sender, EventArgs e)
        {
            About about = new();
            about.ShowDialog();
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
