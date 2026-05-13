using Kinect_20_12_2023_V2;
using Kinect_20_12_2023_V2.Utilities;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Kinect_01_12_V2
{
    /// <summary>
    /// Interaction logic for POCO_Kinect.xaml
    /// </summary>
    public partial class POCO_Kinect : Window
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

        bool Start = false;

        public POCO_Kinect(string ID)
        {
            InitializeComponent();
            Proband = ID;
            writer = new StreamWriter("POCO_" + Proband + ".txt");
        }

        private void POCOK_Loaded(object sender, RoutedEventArgs e)
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

        private void POCOK_Closed(object sender, EventArgs e)
        {
            if (_reader != null) { _reader.Dispose(); }
            if (_sensor != null) { _sensor.Close(); }

            // Cierra el StreamWriter al cerrar la aplicación
            if (writer != null) { writer.Close(); }

            Kinect_20_12_2023_V2.POCO objPOCO = new Kinect_20_12_2023_V2.POCO();
            objPOCO.Close();
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
                            coordinateMapping(body);
                            RemoveUnusedEllipses();
                            if (!Start)
                            {
                                Start = AbstandFuessen(body);
                                if (Start)
                                {
                                    executeSound(0);
                                    State = true;
                                    break;
                                }
                            }

                            if (Start)
                            {
                                // Detectar si los pies estan separados
                                State = AbstandFuessen(body);

                                if (State)
                                {
                                status_indicator.Content = "Closed Feet";
                                status_indicator.Foreground = Brushes.GreenYellow;
                                Speichern = true;
                                }

                                if (!State)
                                {
                                status_indicator.Content = "Open Feet";
                                status_indicator.Foreground = Brushes.OrangeRed;
                                }
                                if (Speichern == true) { recordData(body); }
                            }
                        }
                        RemoveUnusedEllipses();
                    }
                    
                    if (Stop == 2 || Stop == 5)
                    {
                        executeSound(1);
                        string mensaje = "Pause!";
                        MessageBox.Show(mensaje, "Nachricht!");
                        Stop++;
                        Start = false;
                    }
                    if (Stop == 8)
                    {
                        executeSound(1);
                        string mensaje = "Task completed!";
                        MessageBox.Show(mensaje, "Nachricht!");
                        this.Close();
                    }
                    RemoveUnusedEllipses();
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
            Kling++; // 20s
            if (Kling == 600) {
                if (Stop == 0 || Stop == 4 || Stop == 7)
                {
                    executeSound(0);
                }
                Kling = 0;
                Stop++;
            }
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
            if (State) { ellipse.Fill = Brushes.Blue; } else { ellipse.Fill = Brushes.DarkOrange; }
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

        private void executeSound(int select)
        {
            Uri s = new Uri("C:\\Users\\DVeloz\\Documents\\WS 23-24\\Hauptseminar - Final\\Software\\Kinect_20_12_2023_V2\\Buzzer1.wav");
            switch (select)
            {
                case 0: s = new Uri("C:\\Users\\DVeloz\\Documents\\WS 23-24\\Hauptseminar - Final\\Software\\Kinect_20_12_2023_V2\\SendTextDone.wav"); break;
                case 1: s = new Uri("C:\\Users\\DVeloz\\Documents\\WS 23-24\\Hauptseminar - Final\\Software\\Kinect_20_12_2023_V2\\Bell3.wav"); break;
                default: s = new Uri("C:\\Users\\DVeloz\\Documents\\WS 23-24\\Hauptseminar - Final\\Software\\Kinect_20_12_2023_V2\\Buzzer1.wav"); break;
            }
            sound.Source = s;
        }

        private bool AbstandFuessen(Body body)
        {
            //                  O    
            //                 /|\   
            //                / | \  
            //               /  |  \ 
            //                  |     
            //                 / \    
            //                /   \     
            //               /     \
            //              /       \
            //              o_______o
            //               Abstand
            // Guardar la informacion de cada articulacion que nos interesa en una variable:
            Joint ankleRight = body.Joints[JointType.AnkleRight];
            Joint ankleLeft = body.Joints[JointType.AnkleLeft];
            Joint footRight = body.Joints[JointType.FootRight];
            Joint footLeft = body.Joints[JointType.FootLeft];

            double AbstandA = Math.Abs( ankleLeft.Position.X - ankleRight.Position.X );
            double AbstandF = Math.Abs(footLeft.Position.X - footRight.Position.X);

            double Abstand = Math.Abs(AbstandA - AbstandF) / 2;

            if (AbstandF < 0.1) { return true; }
            return false;
        }
    }
}