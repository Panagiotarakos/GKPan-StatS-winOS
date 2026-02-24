using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace GKPanStats
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            var L = Strings.GetStrings();
            VersionText.Text = $"{L["version"]} 2602001";
            MonitorText.Text = L["monitor"];
            CreatedByText.Text = L["created_by"];
        }

        private void Hyperlink_Click(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}
