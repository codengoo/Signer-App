namespace SignerUI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            ICollection<string> apiUrls = null;
            using var startupEvent = new ManualResetEventSlim(false);

            Task.Run(() =>
            {
                Action<ICollection<string>> onStarted = (urls) =>
                 {
                     apiUrls = urls;
                     startupEvent.Set();
                 };

                var app = SignerAPI.ApiHost.Create(onStarted);
                app.Run();
            });

            // *** 2. Chờ đợi API khởi động thành công và lấy URL ***
            // Chờ cho đến khi Task nền gọi startupEvent.Set() (có timeout 10s đề phòng lỗi)
            if (startupEvent.Wait(TimeSpan.FromSeconds(10)))
            {
                var urls = string.Join(", ", apiUrls ?? ["Không tìm thấy URL"]);
                Application.Run(new Main(urls));
            }
            else
            {
                MessageBox.Show("Lỗi: SignerAPI không khởi động kịp sau 10 giây.", "API Status", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}