using Microsoft.Kinect;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Kinect_20_12_2023_V2.Utilities;
using Ellipse = System.Windows.Shapes.Ellipse;

namespace Kinect_20_12_2023_V2
{
    /// <summary>
    /// Interaction logic for SAS_Kinect.xaml
    /// </summary>
    public partial class SAS_Kinect : Window
    {
        // Kinect sensor and Kinect stream reader objects
        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;
        CameraMode _mode = CameraMode.Color;
        private List<Ellipse> ellipses = new List<Ellipse>();

        private StreamWriter writer;                // Data recording file
        private string Proband;                     // Name of file
        int contador = 0;                           // Frame counter
        bool State = false;                         // State of the tester
        int Stop = 0;                               // Counter to control when to stop the test
        int Kling = 0;                              // Counter for the sound
        bool Speichern = false;                     // State of data acquisition
        bool Start = false;                         // Start the acquisition

        public SAS_Kinect(string ID)
        {
            InitializeComponent();
            Proband = ID;
            writer = new StreamWriter("SAS_" + Proband + ".txt");
        }

        private void SASK_Loaded(object sender, RoutedEventArgs e)
        {
            // Kinect sensor initialization
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();
                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
                status_indicator.Content = "Not Detected!";
            }
        }

        private void SASK_Closed(object sender, EventArgs e)
        {
            if (_reader != null) { _reader.Dispose(); }
            if (_sensor != null) { _sensor.Close(); }

            // Cierra el StreamWriter al cerrar la aplicación
            if (writer != null) { writer.Close(); }

            Kinect_20_12_2023_V2.SAS objSAS = new Kinect_20_12_2023_V2.SAS();
            objSAS.Close();
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            { if (frame != null) { if (_mode == CameraMode.Color) { camera.Source = frame.ToBitmap(); } } }

            // Depth
            using (var frame = reference.DepthFrameReference.AcquireFrame())
            { if (frame != null) { if (_mode == CameraMode.Depth) { camera.Source = frame.ToBitmap(); } } }

            // Infrared
            using (var frame = reference.InfraredFrameReference.AcquireFrame())
            { if (frame != null) { if (_mode == CameraMode.Infrared) { camera.Source = frame.ToBitmap(); } } }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    _bodies = new Body[frame.BodyFrameSource.BodyCount];
                    frame.GetAndRefreshBodyData(_bodies);
                    foreach (var body in _bodies)
                    {
                        if (body.IsTracked)
                        {
                            // Detectar la posicion de la persona
                            State = PoseSchaetzung(body);
                            if (!Speichern) { executeSound(0); Speichern = true; }
                            if (State) { status_indicator.Content = "Sit"; status_indicator.Foreground = Brushes.Blue; }
                            if (!State) { status_indicator.Content = "Stand"; status_indicator.Foreground = Brushes.Yellow; }
                            if (Speichern) { recordData(body); }
                            coordinateMapping(body);
                            RemoveUnusedEllipses();
                        }
                        RemoveUnusedEllipses();
                    }
                    RemoveUnusedEllipses();
                    if (Stop == 11)
                    {
                        executeSound(1);
                        string mensaje = "Task completed!";
                        MessageBox.Show(mensaje, "Nachricht!");
                        this.Close();
                    }
                }
            }
            RemoveUnusedEllipses();
        }

        
        private void coordinateMapping(Body body) 
        {
            foreach (Joint joint in body.Joints.Values)
            {
                if (joint.TrackingState == TrackingState.Tracked)
                {
                    // 3D space point
                    CameraSpacePoint jointPosition = joint.Position;

                    // 2D space point
                    Point point = new Point();

                    if (_mode == CameraMode.Color)
                    {
                        ColorSpacePoint colorPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(jointPosition);

                        point.X = float.IsInfinity(colorPoint.X) ? 0 : colorPoint.X;
                        point.Y = float.IsInfinity(colorPoint.Y) ? 0 : colorPoint.Y;
                    }
                    else if (_mode == CameraMode.Depth || _mode == CameraMode.Infrared) // Change the Image and Canvas dimensions to 512x424
                    {
                        DepthSpacePoint depthPoint = _sensor.CoordinateMapper.MapCameraPointToDepthSpace(jointPosition);

                        point.X = float.IsInfinity(depthPoint.X) ? 0 : depthPoint.X;
                        point.Y = float.IsInfinity(depthPoint.Y) ? 0 : depthPoint.Y;
                    }
                    UpdateOrCreateEllipse(joint, point);                    
                }
            }
        }

        private void recordData(Body body)
        {
            contador++;
            Kling++;
            if (Kling == 150) { if (Stop != 10) { executeSound(0); } Kling = 0; Stop++; }
            foreach (Joint joint in body.Joints.Values)
            {
                // 3D space point
                CameraSpacePoint jointPosition = joint.Position;
                writer.WriteLine($"{contador}, {(int)joint.JointType}, {jointPosition.X}, {jointPosition.Y}, {jointPosition.Z}");
            }
        }

        private void UpdateOrCreateEllipse(Joint joint, Point point)
        {
            // 1. Buscar si ya existe una elipse asociada a la articulación
            Ellipse ellipse = ellipses.FirstOrDefault(e => (int)e.Tag == (int)joint.JointType);

            if (ellipse == null)
            {
                // 2. Si no existe, crea un nuevo círculo
                ellipse = new Ellipse
                {
                    Width = 30,
                    Height = 30,
                    Tag = (int)joint.JointType  // Etiqueta que identifica la articulación asociada al círculo
                };

                // 3. Agregar la elipse a la lista de elipses y al lienzo
                ellipses.Add(ellipse);
                canvas.Children.Add(ellipse);
            }
            if (State) { ellipse.Fill = Brushes.Green; } else { ellipse.Fill = Brushes.Blue; }        
            // 4. Actualizar la posición del círculo en el lienzo
            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);
        }

        private void RemoveUnusedEllipses()
        {
            // Itera a través de todas las elipses existentes
            foreach (var ellipse in ellipses)
            {
                // Obtén el tipo de articulación asociada a la elipse
                int jointType = (int)ellipse.Tag;

                // Verifica si la articulación asociada aún está siendo rastreada en algún cuerpo
                if (!_bodies.Any(body => body.IsTracked && body.Joints.ContainsKey((JointType)jointType)))
                {
                    // Si la articulación ya no está rastreada, oculta la elipse
                    ellipse.Visibility = Visibility.Hidden;
                }
                else
                {
                    // Si la articulación está rastreada, muestra la elipse
                    ellipse.Visibility = Visibility.Visible;

                    // También podrías actualizar la posición de la elipse aquí si es necesario
                }
            }
        }

        private bool PoseSchaetzung(Body body)
        {
            //                  O       
            //                 /|\      |
            //                / | \     |   V1 -> Neck:Hip
            //               /  |  \    |
            //                  |       |
            //                 / \              |
            //                /   \             | V2 -> Hip:Knee
            //               /     \
            //              /       \
            //
            // Guardar la informacion de cada articulacion que nos interesa en una variable:
            Joint Neck = body.Joints[JointType.Neck];
            Joint hipLeft = body.Joints[JointType.HipLeft];
            Joint hipRight = body.Joints[JointType.HipRight];
            Joint kneeLeft = body.Joints[JointType.KneeLeft];
            Joint kneeRight = body.Joints[JointType.KneeRight];

            // Se obtiene la pendiente de cada recta: m = y2-y1 / x2-x1
            double V1_Right = (hipRight.Position.Y - Neck.Position.Y) / (hipRight.Position.X - Neck.Position.X);
            double V2_Right = (kneeRight.Position.Y - hipRight.Position.Y) / (kneeRight.Position.X - hipRight.Position.X);
            double V1_Left = (hipLeft.Position.Y - Neck.Position.Y) / (hipLeft.Position.X - Neck.Position.X);
            double V2_Left = (kneeLeft.Position.Y - hipLeft.Position.Y) / (kneeLeft.Position.X - hipLeft.Position.X);

            // Se calcula el angulo entre las rectas: Cos(theta) = || (1*1 + m1*m2) / (sqrt(1^2 + m1^2)*sqrt(1^2 + m2^2)) ||
            double theta_Right = Math.Acos(Math.Abs((1 * 1 + V1_Right * V2_Right) / (Math.Sqrt(1 * 1 + V1_Right * V1_Right) * Math.Sqrt(1 * 1 + V2_Right * V2_Right)))) * (180.0 / Math.PI);
            double theta_Left = Math.Acos(Math.Abs((1 * 1 + V1_Left * V2_Left) / (Math.Sqrt(1 * 1 + V1_Left * V1_Left) * Math.Sqrt(1 * 1 + V2_Left * V2_Left)))) * (180.0 / Math.PI);

            double pose = (theta_Right + theta_Left) / 2;

            if (pose > 25) { return true; }
            return false;
        }

        private void executeSound(int select)
        {
            Uri s = new Uri("C:\\Users\\DVeloz\\Documents\\WS 23-24\\Hauptseminar\\Kinect_20_12_2023_V2\\Buzzer1.wav");
            switch (select)
            {
                case 0: s = new Uri("C:\\Users\\DVeloz\\Documents\\WS 23-24\\Hauptseminar\\Kinect_20_12_2023_V2\\SendTextDone.wav"); break;
                case 1: s = new Uri("C:\\Users\\DVeloz\\Documents\\WS 23-24\\Hauptseminar\\Kinect_20_12_2023_V2\\Bell3.wav"); break;
                default: s = new Uri("C:\\Users\\DVeloz\\Documents\\WS 23-24\\Hauptseminar\\Kinect_20_12_2023_V2\\Buzzer1.wav"); break;
            }
            sound.Source = s;
        }

    }
}