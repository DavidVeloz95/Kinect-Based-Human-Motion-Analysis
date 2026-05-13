using System;
using System.Windows;

namespace Kinect_20_12_2023_V2
{
    /// <summary>
    /// Lógica de interacción para SAS.xaml
    /// </summary>
    public partial class SAS : Window
    {
        private string ID;

        public SAS()
        {
            InitializeComponent();
        }

        private void StartVideoSAS(object sender, EventArgs e)
        {
            string f = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SAS_P.wmv");
            axMediaElement1.Source = new Uri(f);
        }

        private void RepeatSAS(object sender, RoutedEventArgs e)
        {
            string f = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SAS_P.wmv");
            axMediaElement1.Source = new Uri(f);
        }

        private void bntEinverstanden_Click(object sender, RoutedEventArgs e)
        {
            ID = Name.Text;
            Kinect_20_12_2023_V2.SAS_Kinect objSASK = new Kinect_20_12_2023_V2.SAS_Kinect(ID);
            this.Visibility = Visibility.Hidden;        // We're hidding the current Window using this
            objSASK.Show();
        }

        private void SAS_Closed(object sender, EventArgs e)
        {
            Kinect_20_12_2023_V2.MainWindow objMW = new Kinect_20_12_2023_V2.MainWindow();
            objMW.Visibility = Visibility.Visible;
        }
    }
}
