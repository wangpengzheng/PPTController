using Microsoft.Win32;
using PPTControllerHost.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
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

namespace PPTControllerHost.Pages
{
    /// <summary>
    /// Interaction logic for PPTInformation.xaml
    /// </summary>
    public partial class PPTInformation : UserControl
    {
        public PPTInformation()
        {
            InitializeComponent();

            this.DataContext = new PPTInformationViewModel();
        }

        private void ModernButton_Click_1(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".pptx";
            dlg.Filter = "Microsoft PowerPoint (*.pptx, *.ppt)|*.pptx;*.ppt";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                var curDC = DataContext as PPTInformationViewModel;

                curDC.PPTLocation = dlg.FileName;
            }
        }


    }
}