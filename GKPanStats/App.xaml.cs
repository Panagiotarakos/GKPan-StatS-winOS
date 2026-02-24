using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _monitor = new SystemMonitor();

            var saved = Settings.Load();
            if (!string.IsNullOrEmpty(saved))
                _displayModule = saved;

            var icon = LoadIcon();
            _trayIcon = new NotifyIcon
            {
                Icon = icon,
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
            var uri = new Uri("pack://application:,,,/Resources/app.ico");
            var stream = Application.GetResourceStream(uri)?.Stream;
            if (stream != null)
                return new Icon(stream);
            return SystemIcons.Application;
        }

        private void Refresh()
        {
            var L = Strings.GetStrings();

            _cpuText = $"{L["cpu"]}: {_monitor.ReadCPU()}%";
            var (ramUsed, ramTotal, ramPct) = _monitor.ReadRAM();
            _ramText = $"{L["ram"]}: {ramPct}%  ({ramUsed} / {ramTotal})";
            var (diskPct, diskUsed, diskTotal) = _monitor.ReadDisk();
            _diskText = $"{L["disk"]}: {diskPct}%  ({diskUsed} / {diskTotal})";
            var (netDown, netUp) = _monitor.ReadNetwork();
            _netText = $"{L["network"]}: \u2193{netDown}  \u2191{netUp}";
            _batText = _monitor.ReadBattery(L["battery"], L["no_battery"]);

            var textMap = new System.Collections.Generic.Dictionary<string, string>
            {
                ["CPU"] = _cpuText, ["RAM"] = _ramText, ["Disk"] = _diskText,
                ["Net"] = _netText, ["Battery"] = _batText
            };

            _trayIcon.Text = textMap.ContainsKey(_displayModule)
                ? TruncateText(textMap[_displayModule])
                : TruncateText(_cpuText);

            BuildContextMenu();
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
