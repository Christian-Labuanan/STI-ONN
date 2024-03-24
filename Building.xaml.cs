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
using static System.Net.Mime.MediaTypeNames;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for Building.xaml
    /// </summary>
    public partial class Building : Window
    {
        private double _zoomLevel = 1.0;
        public Building()
        {
            InitializeComponent();
        }

        private void touch_Click_1(object sender, RoutedEventArgs e)
        {
            this.Hide();
            Home home = new Home(); 
            home.Show();
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            Zoom(1.1);
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            Zoom(0.9);
        }

        private void Zoom(double zoomFactor)
        {
            _zoomLevel *= zoomFactor;
            var transform = new ScaleTransform(_zoomLevel, _zoomLevel);
            image.LayoutTransform = transform;
            CenterImage();
        }

        private void CenterImage()
        {
            double offsetX = (canvas.ActualWidth - (image.ActualWidth * _zoomLevel)) / 2;
            double offsetY = (canvas.ActualHeight - (image.ActualHeight * _zoomLevel)) / 2;
            Canvas.SetLeft(image, offsetX);
            Canvas.SetTop(image, offsetY);
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoomFactor = e.Delta > 0 ? 1.1 : 0.9;
            Zoom(zoomFactor);
        }
        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CenterImage();
        }
    }
}
