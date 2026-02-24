using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using Application = System.Windows.Application;

namespace GKPanStats
{
    public partial class App : Application
    {
        private NotifyIcon _trayIcon = null!;
        private Timer _timer = null!;
        private SystemMonitor _monitor = null!;
        private string _displayModule = "CPU";
        private string _cpuText = "CPU: --";
        private string _ramText = "RAM: --";
        private string _diskText = "Disk: --";
        private string _netText = "Net: --";
        private string _batText = "Battery: --";
        private int _cpuPct, _ramPct, _diskPct, _batPct;
        private Icon? _appIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _monitor = new SystemMonitor();
            _displayModule = Settings.Load();
            _appIcon = LoadIcon();

            _trayIcon = new NotifyIcon
            {
                Icon = _appIcon,
                Text = "GKPanStats",
                Visible = true
            };

            BuildContextMenu();

            _timer = new Timer { Interval = 2000 };
            _timer.Tick += (s, args) => Refresh();
            _timer.Start();

            var startTimer = new Timer { Interval = 1000 };
            startTimer.Tick += (s, args) => { startTimer.Stop(); Refresh(); };
            startTimer.Start();
        }

        private Icon LoadIcon()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Resources/app.ico");
                var stream = Application.GetResourceStream(uri)?.Stream;
                if (stream != null)
                    return new Icon(stream);
            }
            catch { }
            return SystemIcons.Application;
        }

        private void Refresh()
        {
            var L = Strings.GetStrings();

            _cpuPct = _monitor.ReadCPU();
            _cpuText = $"{L["cpu"]}: {_cpuPct}%";

            var (ramUsed, ramTotal, ramPct) = _monitor.ReadRAM();
            _ramPct = ramPct;
            _ramText = $"{L["ram"]}: {ramPct}%  ({ramUsed} / {ramTotal})";

            var (diskPct, diskUsed, diskTotal) = _monitor.ReadDisk();
            _diskPct = diskPct;
            _diskText = $"{L["disk"]}: {diskPct}%  ({diskUsed} / {diskTotal})";

            var (netDown, netUp) = _monitor.ReadNetwork();
            _netText = $"{L["network"]}: \u2193{netDown}  \u2191{netUp}";

            _batText = _monitor.ReadBattery(L["battery"], L["no_battery"]);
            _batPct = _monitor.LastBatteryPct;

            var textMap = new System.Collections.Generic.Dictionary<string, string>
            {
                ["CPU"] = _cpuText, ["RAM"] = _ramText, ["Disk"] = _diskText,
                ["Net"] = _netText, ["Battery"] = _batText
            };

            _trayIcon.Text = textMap.ContainsKey(_displayModule)
                ? TruncateText(textMap[_displayModule])
                : TruncateText(_cpuText);

            UpdateTrayIcon();
            BuildContextMenu();
        }

        private void UpdateTrayIcon()
        {
            string text;
            Color color;

            switch (_displayModule)
            {
                case "CPU":
                    text = $"{_cpuPct}%";
                    color = Color.FromArgb(52, 152, 219);
                    break;
                case "RAM":
                    text = $"{_ramPct}%";
                    color = Color.FromArgb(155, 89, 182);
                    break;
                case "Disk":
                    text = $"{_diskPct}%";
                    color = Color.FromArgb(230, 126, 34);
                    break;
                case "Net":
                    text = "NET";
                    color = Color.FromArgb(46, 204, 113);
                    break;
                case "Battery":
                    text = $"{_batPct}%";
                    color = Color.FromArgb(241, 196, 15);
                    break;
                default:
                    text = "GK";
                    color = Color.FromArgb(52, 152, 219);
                    break;
            }

            var icon = CreateTextIcon(text, color);
            _trayIcon.Icon = icon;
        }

        private Icon CreateTextIcon(string text, Color bgColor)
        {
            var bmp = new Bitmap(32, 32);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

                using (var bg = new SolidBrush(bgColor))
                {
                    g.FillRectangle(bg, 0, 0, 32, 32);
                }

                var fontSize = text.Length <= 2 ? 14f : text.Length <= 3 ? 11f : 9f;
                using (var font = new Font("Segoe UI", fontSize, System.Drawing.FontStyle.Bold))
                using (var brush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString(text, font, brush, new RectangleF(0, 0, 32, 32), sf);
                }
            }

            var handle = bmp.GetHicon();
            var icon = Icon.FromHandle(handle);
            return (Icon)icon.Clone();
        }

        private string TruncateText(string text)
        {
            return text.Length > 63 ? text.Substring(0, 63) : text;
        }

        private void BuildContextMenu()
        {
            var L = Strings.GetStrings();
            var menu = new ContextMenuStrip();

            var modules = new[] {
                ("CPU", _cpuText),
                ("RAM", _ramText),
                ("Disk", _diskText),
                ("Net", _netText),
                ("Battery", _batText)
            };

            foreach (var (key, text) in modules)
            {
                var item = new ToolStripMenuItem(text);
                item.Checked = key == _displayModule;
                var k = key;
                item.Click += (s, e) =>
                {
                    _displayModule = k;
                    Settings.Save(k);
                    Refresh();
                };
                menu.Items.Add(item);
            }

            menu.Items.Add(new ToolStripSeparator());

            var aboutItem = new ToolStripMenuItem(L["about"]);
            aboutItem.Click += (s, e) => ShowAbout();
            menu.Items.Add(aboutItem);

            menu.Items.Add(new ToolStripSeparator());

            var quitItem = new ToolStripMenuItem(L["quit"]);
            quitItem.Click += (s, e) =>
            {
                _trayIcon.Visible = false;
                _timer.Stop();
                Application.Current.Shutdown();
            };
            menu.Items.Add(quitItem);

            _trayIcon.ContextMenuStrip = menu;
        }

        private void ShowAbout()
        {
            var about = new AboutWindow();
            about.Show();
            about.Activate();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            _timer.Stop();
            base.OnExit(e);
        }
    }
}
