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

namespace Kinect_20_12_2023_V2
{
    /// <summary>
    /// Interaction logic for POCO.xaml
    /// </summary>
    public partial class POCO : Window
    {
        private string ID;

        public POCO()
        {
            InitializeComponent();
        }

        private void StartVideoPOCO(object sender, EventArgs e)
        {
            string f = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "POCO_P.wmv");
            axMediaElement1.Source = new Uri(f);

        }

        private void RepeatPOCO(object sender, RoutedEventArgs e)
        {
            string f = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "POCO_P.wmv");
            axMediaElement1.Source = new Uri(f);
        }

        private void bntEinverstanden_Click(object sender, RoutedEventArgs e)
        {
            ID = Name.Text;
            Kinect_01_12_V2.POCO_Kinect objPOCO = new Kinect_01_12_V2.POCO_Kinect(ID);
            this.Visibility = Visibility.Hidden;        // We're hidding the current Window using this
            objPOCO.Show();
        }

        private void POCO_Closed(object sender, EventArgs e)
        {
            Kinect_20_12_2023_V2.MainWindow objMW = new Kinect_20_12_2023_V2.MainWindow();
            objMW.Visibility = Visibility.Visible;
        }

    }
}
