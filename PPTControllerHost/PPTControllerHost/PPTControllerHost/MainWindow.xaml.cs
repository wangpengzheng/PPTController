using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Presentation;

namespace PPTControllerHost
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        private static MainWindow _instance;

        public static MainWindow Instance
        {
            get { return MainWindow._instance; }
        }

        Link hotSpotLink;

        public MainWindow()
        {
            InitializeComponent();

            #region Load customer setting.
            string curTheme = Properties.Settings.Default.Theme;
            string curFontSize = Properties.Settings.Default.FontSize;
            Color curColor = Properties.Settings.Default.FormColor;
            Uri curThemeSource;

            if (Uri.TryCreate(curTheme, UriKind.Relative, out curThemeSource) &&
                !String.IsNullOrWhiteSpace(curTheme))
            {
                AppearanceManager.Current.ThemeSource = curThemeSource;
            }

            AppearanceManager.Current.FontSize = curFontSize == "LARGE" ?
                FirstFloor.ModernUI.Presentation.FontSize.Large :
                FirstFloor.ModernUI.Presentation.FontSize.Small;

            AppearanceManager.Current.AccentColor = curColor;
            this.Height = Properties.Settings.Default.WindowsHeight;
            this.Width = Properties.Settings.Default.WindowsWidth;

            _instance = this;
            foreach (Link link in this.MenuLinkGroups[0].Links)
            {
                if (link.DisplayName == "Hotspot")
                {
                    hotSpotLink = link;
                    break;
                }
            }
            #endregion
        }

        private void ModernWindow_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.WindowsHeight = this.Height;
            Properties.Settings.Default.WindowsWidth = this.Width;
            Properties.Settings.Default.Save();
        }

        public void DisableHotSpotLink()
        {
            if (this.MenuLinkGroups[0].Links.Contains(hotSpotLink))
            {
                this.MenuLinkGroups[0].Links.Remove(hotSpotLink);
            }

        }

        public void EnableHotSpotLink()
        {
            if (!this.MenuLinkGroups[0].Links.Contains(hotSpotLink))
            {
                this.MenuLinkGroups[0].Links.Insert(1, hotSpotLink);
            }
        }
    }
}
