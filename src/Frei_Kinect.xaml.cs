using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// Extra
using Kinect_20_12_2023_V2.Utilities;
using Microsoft.Kinect;
using Ellipse = System.Windows.Shapes.Ellipse;
using Microsoft.Kinect.Face;
using System.Globalization;

namespace Kinect_20_12_2023_V2
{
    public partial class Frei_Kinect : Window
    {
        // Kinect sensor and Kinect stream reader objects
        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;
        private List<Ellipse> ellipses = new List<Ellipse>();

        CameraMode _mode = CameraMode.Color;

        private StreamWriter writer;                            // Data recording file
        int contador = 0;                                       // Frame counter

        private FaceFrameResult[] faceFrameResults = null;      // Storage for face frame results
        private BodyFrameReader bodyFrameReader = null;         // Reader for body frames
        private FaceFrameSource[] faceFrameSources = null;      // Face frame sources
        private DrawingGroup drawingGroup;                      // Drawing group for body rendering output
        private Rect displayRect;                               // Display rectangle
        private int bodyCount;                                  // Number of bodies tracked
        private List<Brush> faceBrush;                          // List of brushes for each face tracked

        public Frei_Kinect()
        {
            InitializeComponent();
            writer = new StreamWriter("Frei_Proband123.txt");

            this.faceBrush = new List<Brush>()
            {
                Brushes.White,
                Brushes.Orange,
                Brushes.Green,
                Brushes.Red,
                Brushes.LightBlue,
                Brushes.Yellow
            };

        }

        private void FK_Loaded(object sender, RoutedEventArgs e)
        {
            // Kinect sensor initialization
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();
                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
        }

        private void FK_Closed(object sender, EventArgs e)
        {
            if (_reader != null) { _reader.Dispose(); }
            if (_sensor != null) { _sensor.Close(); }
            if (writer != null) { writer.Close(); }
            Kinect_20_12_2023_V2.MainWindow objMW = new Kinect_20_12_2023_V2.MainWindow();
            objMW.Visibility = Visibility.Visible;
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
                            recordData(body);
                        }
                        RemoveUnusedEllipses();
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
            switch ((int)joint.JointType)
            {
                case 0: ellipse.Fill = Brushes.Blue; break;
                case 1: ellipse.Fill = Brushes.Violet; break;
                case 2: ellipse.Fill = Brushes.Green; break;
                case 3: ellipse.Fill = Brushes.Orange; break;
                case 4: ellipse.Fill = Brushes.Yellow; break;
                case 5: ellipse.Fill = Brushes.Pink; break;
                case 6: ellipse.Fill = Brushes.Purple; break;
                case 7: ellipse.Fill = Brushes.Cyan; break;
                case 8: ellipse.Fill = Brushes.Magenta; break;
                case 9: ellipse.Fill = Brushes.Brown; break;
                case 10: ellipse.Fill = Brushes.DarkBlue; break;
                case 11: ellipse.Fill = Brushes.DarkGreen; break;
                case 12: ellipse.Fill = Brushes.DarkRed; break;
                case 13: ellipse.Fill = Brushes.DarkOrange; break;
                case 14: ellipse.Fill = Brushes.DarkGoldenrod; break;
                case 15: ellipse.Fill = Brushes.DarkGray; break;
                case 16: ellipse.Fill = Brushes.DarkKhaki; break;
                case 17: ellipse.Fill = Brushes.DarkCyan; break;
                case 18: ellipse.Fill = Brushes.DarkMagenta; break;
                case 19: ellipse.Fill = Brushes.LightBlue; break;
                case 20: ellipse.Fill = Brushes.LightGreen; break;
                case 21: ellipse.Fill = Brushes.LightCoral; break;
                case 22: ellipse.Fill = Brushes.LightSalmon; break;
                case 23: ellipse.Fill = Brushes.LightYellow; break;
                case 24: ellipse.Fill = Brushes.LightPink; break;
                default:
                    ellipse.Fill = Brushes.Black; // color por defecto si no coincide con ningún caso
                    break;
            }
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

        private void recordData(Body body)
        {
            contador++;
            foreach (Joint joint in body.Joints.Values)
            {
                // 3D space point
                CameraSpacePoint jointPosition = joint.Position;
                writer.WriteLine($"{contador}, {(int)joint.JointType}, {jointPosition.X}, {jointPosition.Y}, {jointPosition.Z}");
            }
        }

        private void DrawFaceFrameResults(int faceIndex, FaceFrameResult faceResult, DrawingContext drawingContext)
        {
            // choose the brush based on the face index
            Brush drawingBrush = this.faceBrush[0];
            if (faceIndex < this.bodyCount)
            {
                drawingBrush = this.faceBrush[faceIndex];
            }

            string faceText = string.Empty;

            // extract each face property information and store it in faceText
            if (faceResult.FaceProperties != null)
            {
                foreach (var item in faceResult.FaceProperties)
                {
                    faceText += item.Key.ToString() + " : ";

                    // consider a "maybe" as a "no" to restrict 
                    // the detection result refresh rate
                    if (item.Value == DetectionResult.Maybe)
                    {
                        faceText += DetectionResult.No + "\n";
                    }
                    else
                    {
                        faceText += item.Value.ToString() + "\n";
                    }
                }
            }
        }


    }

    enum CameraMode
    {
        Color,
        Depth,
        Infrared
    }

}
