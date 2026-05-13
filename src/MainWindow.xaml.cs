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

namespace Kinect_20_12_2023_V2
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnFrei_Click(object sender, RoutedEventArgs e)
        {
            Kinect_20_12_2023_V2.Frei_Kinect objFrei = new Kinect_20_12_2023_V2.Frei_Kinect();
            this.Visibility = Visibility.Hidden;        // We're hidding the current Window using this
            objFrei.Show();
        }

        private void btnSAS_Click(object sender, RoutedEventArgs e)
        {
            Kinect_20_12_2023_V2.SAS objSAS = new Kinect_20_12_2023_V2.SAS();
            this.Visibility = Visibility.Hidden;        // We're hidding the current Window using this
            objSAS.Show();
        }

        private void btnPOCO_Click(object sender, RoutedEventArgs e)
        {
            Kinect_20_12_2023_V2.POCO objPOCO = new Kinect_20_12_2023_V2.POCO();
            this.Visibility = Visibility.Hidden;        // We're hidding the current Window using this
            objPOCO.Show();
        }

        private void btnSIP_Click(object sender, RoutedEventArgs e)
        {
            Kinect_01_12_V2.SIP objSIP = new Kinect_01_12_V2.SIP();
            this.Visibility = Visibility.Hidden;        // We're hidding the current Window using this
            objSIP.Show();
        }

        private void MainWindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
