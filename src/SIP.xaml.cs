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
using System.Windows.Shapes;

namespace Kinect_01_12_V2
{
    /// <summary>
    /// Lógica de interacción para SIP.xaml
    /// </summary>
    public partial class SIP : Window
    {
        private string ID;

        public SIP()
        {
            InitializeComponent();
        }

        private void SIP_Closed(object sender, EventArgs e)
        {
            Kinect_20_12_2023_V2.MainWindow objMW = new Kinect_20_12_2023_V2.MainWindow();
            objMW.Visibility = Visibility.Visible;
        }

        private void StartVideoSIP(object sender, EventArgs e)
        {
            string f = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SIP_P.wmv");
            axMediaElement1.Source = new Uri(f);
        }

        private void RepeatSIP(object sender, RoutedEventArgs e)
        {
            string f = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SIP_P.wmv");
            axMediaElement1.Source = new Uri(f);
        }

        private void bntEinverstanden_Click(object sender, RoutedEventArgs e)
        {
            ID = Name.Text;
            Kinect_01_12_V2.SIP_Kinect objSIP = new Kinect_01_12_V2.SIP_Kinect(ID);
            this.Visibility = Visibility.Hidden;        // We're hidding the current Window using this
            objSIP.Show();
        }
    }
}
