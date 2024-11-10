using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for Roofdeck.xaml
    /// </summary>
    public partial class Roofdeck : Window
    {
        private double originalWidth;
        private double originalHeight;
        private double squareWidth;
        private double squareHeight;
        private double originalImageLeft;
        private double originalImageTop;

        private double originalScale = 1.0;

        private bool isDragging = false;
        private Point lastPosition;

        private DispatcherTimer interactionTimer;

        public Roofdeck()
        {
            InitializeComponent();

            // Store the original size of the image
            originalWidth = image.Width;
            originalHeight = image.Height;

            // Store the original position of the image
            originalImageLeft = Canvas.GetLeft(image);
            originalImageTop = Canvas.GetTop(image);

            // Initialize and start the interaction timer
            interactionTimer = new DispatcherTimer();
            interactionTimer.Interval = TimeSpan.FromMinutes(30);//set the time limit here
            interactionTimer.Tick += InteractionTimer_Tick;
            ResetInteractionTimer();
        }
        // Zoom in and out 
        // Drag left and right
        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var delta = e.Delta;
            var scale = delta > 0 ? 1.1 : 0.9; // Increase or decrease zoom factor

            // Calculate the zooming center point relative to the image
            Point zoomCenter = e.GetPosition(image);

            // Calculate the new width and height after zooming
            double newWidth = image.Width * scale;
            double newHeight = image.Height * scale;

            // Apply the new width and height to the image
            image.Width = newWidth;
            image.Height = newHeight;
            ResetInteractionTimer();
        }


        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            lastPosition = e.GetPosition(canvas);
            image.CaptureMouse();
            ResetInteractionTimer();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            image.ReleaseMouseCapture();
            ResetInteractionTimer();
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                var currentPosition = e.GetPosition(canvas);
                var deltaX = currentPosition.X - lastPosition.X;
                var deltaY = currentPosition.Y - lastPosition.Y;
                lastPosition = currentPosition;

                Canvas.SetLeft(image, Canvas.GetLeft(image) + deltaX);
                Canvas.SetTop(image, Canvas.GetTop(image) + deltaY);
                ResetInteractionTimer();
            }
        }

        private void ZoomResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the image is already at its original size
            if (image.Width == originalWidth && image.Height == originalHeight)
            {
                // Do nothing if the image is already at its original size
                return;
            }

            // Reset the image size to its original size
            image.Width = originalWidth;
            image.Height = originalHeight;

            // Reset the image scale
            originalScale = 1.0;

            // Reset the image transformation
            image.LayoutTransform = Transform.Identity;

            // Reset the position of the image to its original position
            Canvas.SetLeft(image, originalImageLeft);
            Canvas.SetTop(image, originalImageTop);

            // Reset the screensaver timer
            ResetInteractionTimer();
        }

        private void ApplyZoom(double scale)
        {
            image.Width = originalWidth * scale;
            image.Height = originalHeight * scale;
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            originalScale += 0.1;
            ApplyZoom(originalScale);
            ResetInteractionTimer();
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            originalScale -= 0.1;
            ApplyZoom(originalScale);
            ResetInteractionTimer();
        }

        #region timer
        // Timer of screensaver
        private void ResetInteractionTimer()
        {
            // Reset the timer
            interactionTimer.Stop();
            interactionTimer.Start();
        }

        private void InteractionTimer_Tick(object sender, EventArgs e)
        {
            // Timer ticked without any interaction, return to the previous window
            ReturnToPreviousWindow();
        }

        private void ReturnToPreviousWindow()
        {
            // Close the current window and show the previous window
            this.Close();
            MainWindow main = new MainWindow();
            main.Show();
            interactionTimer.Stop();
        }

        // Event handlers to track user interaction
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetInteractionTimer();
        }

        #endregion

        private void ground_btn(object sender, RoutedEventArgs e)
        {
            Building b1 = new Building();
            b1.Show();
            this.Close();
        }
        private void home_btn(object sender, RoutedEventArgs e)
        {
            Home h1 = new Home();
            h1.Show();
            this.Close();
        }

        private void announcement_btn(object sender, RoutedEventArgs e)
        {
            Announcement a1 = new Announcement();
            a1.Show();
            this.Close();
        }

        private void mezzanine_btn(object sender, RoutedEventArgs e)
        {
            Mezzanine m1 = new Mezzanine();
            m1.Show();
            this.Close();
        }
        private void instructor_schedule_btn(object sender, RoutedEventArgs e)
        {
            ProfessorSchedule ps = new ProfessorSchedule();
            ps.Show();
            this.Close();
        }
        private void floor2_btn(object sender, RoutedEventArgs e)
        {
            Floor2 f2 = new Floor2();
            f2.Show();
            this.Close();
        }

        private void floor3_btn(object sender, RoutedEventArgs e)
        {
            Floor3 floor3 = new Floor3();
            floor3.Show();
            this.Close();
        }

        private void floor4_btn(object sender, RoutedEventArgs e)
        {
            Floor4 floor4 = new Floor4();
            floor4.Show();
            this.Close();
        }
        private void court_btn(object sender, RoutedEventArgs e)
        {
            Court court = new Court();
            court.Show();
            this.Close();
        }
    }
}
