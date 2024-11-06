using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace STI_ONN
{
    public partial class Building : Window
    {
        private double originalWidth;
        private double originalHeight;
        private double squareWidth;
        private double squareHeight;
        private double originalImageLeft;
        private double originalImageTop;

        private double originalScale = 1.0;

        private double originalClickableSectionLeft;
        private double originalClickableSectionTop;
        private double originalClickableSectionWidth;
        private double originalClickableSectionHeight;

        private bool isDragging = false;
        private Point lastPosition;

        private DispatcherTimer interactionTimer;

        public Building()
        {
            InitializeComponent();
            CreateArrows(5); // Number of arrows you want to create
            CreateArrows2(5); // Number of arrows you want to create

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
        private void CreateArrows(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var arrow = new Polygon
                {
                    // Define points for a right-facing arrow
                    Points = new PointCollection
                    {
                        new Point(0, 5),    // Left point (base of the arrow)
                        new Point(0, 15),   // Top point (tip of the arrow)
                        new Point(15, 10)   // Bottom point (tip of the arrow)
                    },
                    Fill = Brushes.Green,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(10, 0, 10, 0) // Spacing between arrows
                };

                // Apply scaling transformation to make the arrow bigger
                ScaleTransform scaleTransform = new ScaleTransform(2.0, 2.0); // Scale by a factor of 2
                arrow.RenderTransform = scaleTransform;

                // Add arrow to the ItemsControl
                ArrowsContainer.Items.Add(arrow);

                // Create blinking animation for each arrow
                DoubleAnimation blinkAnimation = new DoubleAnimation
                {
                    From = 1.0,          // Fully visible
                    To = 0.0,            // Invisible
                    Duration = new Duration(TimeSpan.FromSeconds(1)), // Duration of fade
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    BeginTime = TimeSpan.FromSeconds(i * 0.2) // Stagger the start time
                };

                // Apply the animation to the Opacity property of the arrow
                arrow.BeginAnimation(Polygon.OpacityProperty, blinkAnimation);
            }
        }
        private void CreateArrows2(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var arrow = new Polygon
                {
                    // Define points for a right-facing arrow
                    Points = new PointCollection
            {
                new Point(0, 5),    // Left point (base of the arrow)
                new Point(15, 0),   // Top point (tip of the arrow)
                new Point(15, 10)   // Bottom point (tip of the arrow)
            },
                    Fill = Brushes.Green,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(10, 0, 10, 0) // Spacing between arrows
                };

                // Apply scaling transformation to make the arrow bigger
                ScaleTransform scaleTransform = new ScaleTransform(2.0, 2.0); // Scale by a factor of 2
                arrow.RenderTransform = scaleTransform;

                // Add arrow to the ItemsControl
                ArrowsContainer2.Items.Add(arrow);

                // Create blinking animation for each arrow
                DoubleAnimation blinkAnimation = new DoubleAnimation
                {
                    From = 1.0,          // Start from fully visible
                    To = 0.0,            // Fade to fully transparent
                    Duration = new Duration(TimeSpan.FromSeconds(1)), // Duration of fade
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    BeginTime = TimeSpan.FromSeconds((count - i - 1) * 0.2) // Start from the last arrow
                };

                // Apply the animation to the Opacity property of the arrow
                arrow.BeginAnimation(Polygon.OpacityProperty, blinkAnimation);
            }
        }

        // Zoom in and out 
        // Drag left and right
        private void Image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            HideArrows();
            var delta = e.Delta;
            var scale = delta > 0 ? 1.1 : 0.9; // Increase or decrease zoom factor

            // Calculate the zooming center point relative to the image
            Point zoomCenter = e.GetPosition(image);

            // Calculate the new width and height after zooming
            double newWidth = image.Width * scale;
            double newHeight = image.Height * scale;

            // Calculate the adjustment for the clickable section size and position

            // Apply the new width and height to the image
            image.Width = newWidth;
            image.Height = newHeight;
            ResetInteractionTimer();
        }


        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            HideArrows();
            isDragging = true;
            lastPosition = e.GetPosition(canvas);
            image.CaptureMouse();
            ResetInteractionTimer();
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            HideArrows();
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
            // Reset the position of the image to its original position
            Canvas.SetLeft(image, originalImageLeft);
            Canvas.SetTop(image, originalImageTop);

            // If the image size is not the original, reset it to its original size
            if (image.Width != originalWidth || image.Height != originalHeight)
            {
                image.Width = originalWidth;
                image.Height = originalHeight;
                originalScale = 1.0;
                image.LayoutTransform = Transform.Identity;
            }

            ShowArrows(); // Show the arrows after resetting
            ResetInteractionTimer(); // Reset the screensaver timer
        }


        private void ApplyZoom(double scale)
        {
            // Zoom image
            image.Width = originalWidth * scale;
            image.Height = originalHeight * scale;

            HideArrows();
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

        // Function to hide the arrows
        private void HideArrows()
        {
            desc1.Visibility = Visibility.Collapsed;
            desc2.Visibility = Visibility.Collapsed;
            ArrowsContainer.Visibility = Visibility.Collapsed;
            ArrowsContainer2.Visibility = Visibility.Collapsed;
        }

        // Function to show the arrows
        private void ShowArrows()
        {
            ArrowsContainer.Visibility = Visibility.Visible;
            ArrowsContainer2.Visibility = Visibility.Visible;
            desc1.Visibility = Visibility.Visible;
            desc2.Visibility = Visibility.Visible;
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

        private void roofdeck_btn(object sender, RoutedEventArgs e)
        {
            Roofdeck roofdeck = new Roofdeck();
            roofdeck.Show();
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