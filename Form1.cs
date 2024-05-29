using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ClipBoardMon
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            /*            InitializeComponent();
                        File.WriteAllText("log.txt", string.Empty); // create an empty log file
                        btnOpenLog.Click += btnOpenLog_Click;*/
            InitializeComponent();
            string logFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "log.txt");
            if (!File.Exists(logFilePath))
            {
                File.Create(logFilePath).Close(); // create an empty log file if it doesn't exist
            }
            //btnOpenLog.Click += btnOpenLog_Click; // subscribe to btnOpenLog Click event
        }

        private void btnOpenLog_Click(object sender, EventArgs e)
        {
            string logFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "log.txt");
            Process.Start("notepad.exe", logFilePath);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Clipboard.Clear(); // clear clipboard to prevent duplicate values
            ClipboardMonitor.ClipboardChanged += ClipboardMonitor_ClipboardChanged; // subscribe to clipboard changed event
            ClipboardMonitor.Start(); // start monitoring clipboard
            rtbLog.AppendText("Started Monitoring..");
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ClipboardMonitor.Stop(); // stop monitoring clipboard
            ClipboardMonitor.ClipboardChanged -= ClipboardMonitor_ClipboardChanged; // unsubscribe from clipboard changed event
            rtbLog.AppendText("Stoped Monitoring..");
        }

        private void ClipboardMonitor_ClipboardChanged(object sender, EventArgs e)
        {
            Invoke(new MethodInvoker(() =>
            {
                rtbLog.AppendText($"Clipboard value changed to: {Clipboard.GetText()}\n");
            }));
        }
        public static class ClipboardMonitor
        {
            private static string lastClipboardValue = string.Empty;

            public static event EventHandler ClipboardChanged;

            public static void Start()
            {
                lastClipboardValue = string.Empty;
                Timer timer = new Timer();
                timer.Interval = 100;
                timer.Tick += Timer_Tick;
                timer.Start();
            }

            public static void Stop()
            {
                lastClipboardValue = string.Empty;
            }

            private static void LogClipboardValue(string clipboardValue)
            {
                string logFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "log.txt");
                using (StreamWriter sw = new StreamWriter(logFilePath, true))
                {
                    sw.WriteLine($"{DateTime.Now.ToString()}: {clipboardValue}");
                }
            }

            private static void Timer_Tick(object sender, EventArgs e)
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardValue = Clipboard.GetText();
                    if (clipboardValue != lastClipboardValue)
                    {
                        lastClipboardValue = clipboardValue;
                        LogClipboardValue(clipboardValue);
                        ClipboardChanged?.Invoke(null, EventArgs.Empty);
                    }
                }
            }
        }
    }
}