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

        // Define the zoom limits
        private const double minScale = 0.8; // Minimum zoom level
        private const double maxScale = 1.5; // Maximum zoom level


        //101
        private double originalClickableSectionLeft;
        private double originalClickableSectionTop;
        private double originalClickableSectionWidth;
        private double originalClickableSectionHeight;

        //102
        private double originalClickableSectionCopyLeft;
        private double originalClickableSectionCopyTop;
        private double originalClickableSectionCopyWidth;
        private double originalClickableSectionCopyHeight;
        //103
        private double originalClickableSectionCopyLeft303;
        private double originalClickableSectionCopyTop303;
        private double originalClickableSectionCopyWidth303;
        private double originalClickableSectionCopyHeight303;
        //cashier
        private double originalClickableSectionCopyLeft304;
        private double originalClickableSectionCopyTop304;
        private double originalClickableSectionCopyWidth304;
        private double originalClickableSectionCopyHeight304;

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

            // Store the original position and size of the clickable section
            originalClickableSectionLeft = Canvas.GetLeft(clickableSection);
            originalClickableSectionTop = Canvas.GetTop(clickableSection);
            originalClickableSectionWidth = clickableSection.Width;
            originalClickableSectionHeight = clickableSection.Height;
            //102
            originalClickableSectionCopyLeft = Canvas.GetLeft(clickableSection_Copy);
            originalClickableSectionCopyTop = Canvas.GetTop(clickableSection_Copy);
            originalClickableSectionCopyWidth = clickableSection_Copy.Width;
            originalClickableSectionCopyHeight = clickableSection_Copy.Height;
            //103
            originalClickableSectionCopyLeft303 = Canvas.GetLeft(clickableSection_Copy1);
            originalClickableSectionCopyTop303 = Canvas.GetTop(clickableSection_Copy1);
            originalClickableSectionCopyWidth303 = clickableSection_Copy1.Width;
            originalClickableSectionCopyHeight303 = clickableSection_Copy1.Height;
            //cashier
            originalClickableSectionCopyLeft304 = Canvas.GetLeft(clickableSection_Copy2);
            originalClickableSectionCopyTop304 = Canvas.GetTop(clickableSection_Copy2);
            originalClickableSectionCopyWidth304 = clickableSection_Copy2.Width;
            originalClickableSectionCopyHeight304 = clickableSection_Copy2.Height;

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

                //101
                Canvas.SetLeft(clickableSection, Canvas.GetLeft(clickableSection) + deltaX);
                Canvas.SetTop(clickableSection, Canvas.GetTop(clickableSection) + deltaY);
                //102
                Canvas.SetLeft(clickableSection_Copy, Canvas.GetLeft(clickableSection_Copy) + deltaX);
                Canvas.SetTop(clickableSection_Copy, Canvas.GetTop(clickableSection_Copy) + deltaY);
                //103
                Canvas.SetLeft(clickableSection_Copy1, Canvas.GetLeft(clickableSection_Copy1) + deltaX);
                Canvas.SetTop(clickableSection_Copy1, Canvas.GetTop(clickableSection_Copy1) + deltaY);
                //cashier
                Canvas.SetLeft(clickableSection_Copy2, Canvas.GetLeft(clickableSection_Copy2) + deltaX);
                Canvas.SetTop(clickableSection_Copy2, Canvas.GetTop(clickableSection_Copy2) + deltaY);

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

            // Reset the position and size of the clickable section to its original values
            Canvas.SetLeft(clickableSection, originalClickableSectionLeft);
            Canvas.SetTop(clickableSection, originalClickableSectionTop);
            clickableSection.Width = originalClickableSectionWidth;
            clickableSection.Height = originalClickableSectionHeight;
            //102
            Canvas.SetLeft(clickableSection_Copy, originalClickableSectionCopyLeft);
            Canvas.SetTop(clickableSection_Copy, originalClickableSectionCopyTop);
            clickableSection_Copy.Width = originalClickableSectionCopyWidth;
            clickableSection_Copy.Height = originalClickableSectionCopyHeight;
            //103
            Canvas.SetLeft(clickableSection_Copy1, originalClickableSectionCopyLeft303);
            Canvas.SetTop(clickableSection_Copy1, originalClickableSectionCopyTop303);
            clickableSection_Copy1.Width = originalClickableSectionCopyWidth303;
            clickableSection_Copy1.Height = originalClickableSectionCopyHeight303;
            //cashier
            Canvas.SetLeft(clickableSection_Copy2, originalClickableSectionCopyLeft304);
            Canvas.SetTop(clickableSection_Copy2, originalClickableSectionCopyTop304);
            clickableSection_Copy2.Width = originalClickableSectionCopyWidth304;
            clickableSection_Copy2.Height = originalClickableSectionCopyHeight304;

            ShowArrows(); // Show the arrows after resetting
            ResetInteractionTimer(); // Reset the screensaver timer
        }


        private void ApplyZoom(double scale)
        {
            // Zoom image
            image.Width = originalWidth * scale;
            image.Height = originalHeight * scale;

            //101
            Canvas.SetLeft(clickableSection, originalClickableSectionLeft * scale);
            Canvas.SetTop(clickableSection, originalClickableSectionTop * scale);
            clickableSection.Width = originalClickableSectionWidth * scale;
            clickableSection.Height = originalClickableSectionHeight * scale;
            //102
            Canvas.SetLeft(clickableSection_Copy, originalClickableSectionCopyLeft * scale);
            Canvas.SetTop(clickableSection_Copy, originalClickableSectionCopyTop * scale);
            clickableSection_Copy.Width = originalClickableSectionCopyWidth * scale;
            clickableSection_Copy.Height = originalClickableSectionCopyHeight * scale;
            //103
            Canvas.SetLeft(clickableSection_Copy1, originalClickableSectionCopyLeft303 * scale);
            Canvas.SetTop(clickableSection_Copy1, originalClickableSectionCopyTop303 * scale);
            clickableSection_Copy1.Width = originalClickableSectionCopyWidth303 * scale;
            clickableSection_Copy1.Height = originalClickableSectionCopyHeight303 * scale;
            //cashier
            Canvas.SetLeft(clickableSection_Copy2, originalClickableSectionCopyLeft304 * scale);
            Canvas.SetTop(clickableSection_Copy2, originalClickableSectionCopyTop304 * scale);
            clickableSection_Copy2.Width = originalClickableSectionCopyWidth304 * scale;
            clickableSection_Copy2.Height = originalClickableSectionCopyHeight304 * scale;

            HideArrows();
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            if (originalScale < maxScale)
            {
                originalScale += 0.1;
                ApplyZoom(originalScale);
                HideArrows();
                ResetInteractionTimer();
            }
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (originalScale > minScale)
            {
                originalScale -= 0.1;
                ApplyZoom(originalScale);
                HideArrows();
                ResetInteractionTimer();
            }
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

        private void room101(object sender, RoutedEventArgs e)
        {
            // Create and show the room 101 window
            Floor1Schedule floor1 = new Floor1Schedule(1, "Room 101 Schedules");
            floor1.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor1.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor1.ShowDialog();
        }
        private void room102(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor1Schedule floor1 = new Floor1Schedule(2, "Room 102 Schedules");
            floor1.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 102 window to hide the dimming overlay when the window is closed
            floor1.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor1.ShowDialog();
        }
        private void room103(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor1Schedule floor1 = new Floor1Schedule(3, "Room 103 Schedules");
            floor1.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor1.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor1.ShowDialog();
        }
        private void ShowCashierSchedule(object sender, MouseButtonEventArgs e)
        {
            // Show dimming overlay
            dimmingOverlay.Visibility = Visibility.Visible;
            StaticSchedule scheduleWindow = new StaticSchedule();
            scheduleWindow.Owner = this;
            scheduleWindow.Closed += (s, args) =>
            {
                // Hide dimming overlay after StaticSchedule window is closed
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            scheduleWindow.ShowDialog();
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
        private void instructor_schedule_btn(object sender, RoutedEventArgs e)
        {
            ProfessorSchedule ps = new ProfessorSchedule();
            ps.Show();
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