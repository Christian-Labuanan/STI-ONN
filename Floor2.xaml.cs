using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace STI_ONN
{
    public partial class Floor2 : Window
    {
        private double originalWidth;
        private double originalHeight;
        private double squareWidth;
        private double squareHeight;
        private double originalImageLeft;
        private double originalImageTop;

        // Define the zoom limits
        private const double minScale = 0.8; // Minimum zoom level
        private const double maxScale = 1.5; // Maximum zoom level

        private double originalScale = 1.0;
        //201
        private double originalClickableSectionLeft;
        private double originalClickableSectionTop;
        private double originalClickableSectionWidth;
        private double originalClickableSectionHeight;
        //202
        private double originalClickableSectionCopyLeft;
        private double originalClickableSectionCopyTop;
        private double originalClickableSectionCopyWidth;
        private double originalClickableSectionCopyHeight;
        //203
        private double originalClickableSectionCopyLeft303;
        private double originalClickableSectionCopyTop303;
        private double originalClickableSectionCopyWidth303;
        private double originalClickableSectionCopyHeight303;
        //204
        private double originalClickableSectionCopyLeft304;
        private double originalClickableSectionCopyTop304;
        private double originalClickableSectionCopyWidth304;
        private double originalClickableSectionCopyHeight304;
        //205
        private double originalClickableSectionCopyLeft305;
        private double originalClickableSectionCopyTop305;
        private double originalClickableSectionCopyWidth305;
        private double originalClickableSectionCopyHeight305;
        //206
        private double originalClickableSectionCopyLeft306;
        private double originalClickableSectionCopyTop306;
        private double originalClickableSectionCopyWidth306;
        private double originalClickableSectionCopyHeight306;
        //207
        private double originalClickableSectionCopyLeft307;
        private double originalClickableSectionCopyTop307;
        private double originalClickableSectionCopyWidth307;
        private double originalClickableSectionCopyHeight307;
        //208
        private double originalClickableSectionCopyLeft3072;
        private double originalClickableSectionCopyTop3072;
        private double originalClickableSectionCopyWidth3072;
        private double originalClickableSectionCopyHeight3072;
        //209
        private double originalClickableSectionCopyLeft308;
        private double originalClickableSectionCopyTop308;
        private double originalClickableSectionCopyWidth308;
        private double originalClickableSectionCopyHeight308;
        //210
        private double originalClickableSectionCopyLeft309;
        private double originalClickableSectionCopyTop309;
        private double originalClickableSectionCopyWidth309;
        private double originalClickableSectionCopyHeight309;
        //211
        private double originalClickableSectionCopyLeft310;
        private double originalClickableSectionCopyTop310;
        private double originalClickableSectionCopyWidth310;
        private double originalClickableSectionCopyHeight310;
        //212
        private double originalClickableSectionCopyLeft311;
        private double originalClickableSectionCopyTop311;
        private double originalClickableSectionCopyWidth311;
        private double originalClickableSectionCopyHeight311;
        //213
        private double originalClickableSectionCopyLeft312;
        private double originalClickableSectionCopyTop312;
        private double originalClickableSectionCopyWidth312;
        private double originalClickableSectionCopyHeight312;

        private bool isDragging = false;
        private Point lastPosition;

        private DispatcherTimer interactionTimer;

        public Floor2()
        {
            InitializeComponent();
            CreateArrows(5); // Number of arrows you want to create
            CreateArrows2(5); // Number of arrows you want to create
            // Store the original size of the image
            originalWidth = image.Width;
            originalHeight = image.Height;
            squareWidth = clickableSection.Width;
            squareHeight = clickableSection.Height;

            // Store the original position of the image
            originalImageLeft = Canvas.GetLeft(image);
            originalImageTop = Canvas.GetTop(image);

            // Store the original position and size of the clickable section
            originalClickableSectionLeft = Canvas.GetLeft(clickableSection);
            originalClickableSectionTop = Canvas.GetTop(clickableSection);
            originalClickableSectionWidth = clickableSection.Width;
            originalClickableSectionHeight = clickableSection.Height;
            //202
            originalClickableSectionCopyLeft = Canvas.GetLeft(clickableSection_Copy);
            originalClickableSectionCopyTop = Canvas.GetTop(clickableSection_Copy);
            originalClickableSectionCopyWidth = clickableSection_Copy.Width;
            originalClickableSectionCopyHeight = clickableSection_Copy.Height;
            //203
            originalClickableSectionCopyLeft303 = Canvas.GetLeft(clickableSection_Copy1);
            originalClickableSectionCopyTop303 = Canvas.GetTop(clickableSection_Copy1);
            originalClickableSectionCopyWidth303 = clickableSection_Copy1.Width;
            originalClickableSectionCopyHeight303 = clickableSection_Copy1.Height;
            //204
            originalClickableSectionCopyLeft304 = Canvas.GetLeft(clickableSection_Copy2);
            originalClickableSectionCopyTop304 = Canvas.GetTop(clickableSection_Copy2);
            originalClickableSectionCopyWidth304 = clickableSection_Copy2.Width;
            originalClickableSectionCopyHeight304 = clickableSection_Copy2.Height;
            //205
            originalClickableSectionCopyLeft305 = Canvas.GetLeft(clickableSection_Copy3);
            originalClickableSectionCopyTop305 = Canvas.GetTop(clickableSection_Copy3);
            originalClickableSectionCopyWidth305 = clickableSection_Copy3.Width;
            originalClickableSectionCopyHeight305 = clickableSection_Copy3.Height;
            //206
            originalClickableSectionCopyLeft306 = Canvas.GetLeft(clickableSection_Copy4);
            originalClickableSectionCopyTop306 = Canvas.GetTop(clickableSection_Copy4);
            originalClickableSectionCopyWidth306 = clickableSection_Copy4.Width;
            originalClickableSectionCopyHeight306 = clickableSection_Copy4.Height;
            //207
            originalClickableSectionCopyLeft307 = Canvas.GetLeft(clickableSection_Copy5);
            originalClickableSectionCopyTop307 = Canvas.GetTop(clickableSection_Copy5);
            originalClickableSectionCopyWidth307 = clickableSection_Copy5.Width;
            originalClickableSectionCopyHeight307 = clickableSection_Copy5.Height;
            //208
            originalClickableSectionCopyLeft3072 = Canvas.GetLeft(clickableSection_Copy6);
            originalClickableSectionCopyTop3072 = Canvas.GetTop(clickableSection_Copy6);
            originalClickableSectionCopyWidth3072 = clickableSection_Copy6.Width;
            originalClickableSectionCopyHeight3072 = clickableSection_Copy6.Height;
            //209
            originalClickableSectionCopyLeft308 = Canvas.GetLeft(clickableSection_Copy7);
            originalClickableSectionCopyTop308 = Canvas.GetTop(clickableSection_Copy7);
            originalClickableSectionCopyWidth308 = clickableSection_Copy7.Width;
            originalClickableSectionCopyHeight308 = clickableSection_Copy7.Height;
            //210
            originalClickableSectionCopyLeft309 = Canvas.GetLeft(clickableSection_Copy8);
            originalClickableSectionCopyTop309 = Canvas.GetTop(clickableSection_Copy8);
            originalClickableSectionCopyWidth309 = clickableSection_Copy8.Width;
            originalClickableSectionCopyHeight309 = clickableSection_Copy8.Height;
            //211
            originalClickableSectionCopyLeft310 = Canvas.GetLeft(clickableSection_Copy9);
            originalClickableSectionCopyTop310 = Canvas.GetTop(clickableSection_Copy9);
            originalClickableSectionCopyWidth310 = clickableSection_Copy9.Width;
            originalClickableSectionCopyHeight310 = clickableSection_Copy9.Height;
            //212
            originalClickableSectionCopyLeft311 = Canvas.GetLeft(clickableSection_Copy10);
            originalClickableSectionCopyTop311 = Canvas.GetTop(clickableSection_Copy10);
            originalClickableSectionCopyWidth311 = clickableSection_Copy10.Width;
            originalClickableSectionCopyHeight311 = clickableSection_Copy10.Height;
            //213
            originalClickableSectionCopyLeft312 = Canvas.GetLeft(clickableSection_Copy11);
            originalClickableSectionCopyTop312 = Canvas.GetTop(clickableSection_Copy11);
            originalClickableSectionCopyWidth312 = clickableSection_Copy11.Width;
            originalClickableSectionCopyHeight312 = clickableSection_Copy11.Height;


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
            clickableSection.ReleaseMouseCapture();
            ResetInteractionTimer();
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                HideArrows();
                var currentPosition = e.GetPosition(canvas);
                var deltaX = currentPosition.X - lastPosition.X;
                var deltaY = currentPosition.Y - lastPosition.Y;
                lastPosition = currentPosition;

                Canvas.SetLeft(image, Canvas.GetLeft(image) + deltaX);
                Canvas.SetTop(image, Canvas.GetTop(image) + deltaY);

                //201
                Canvas.SetLeft(clickableSection, Canvas.GetLeft(clickableSection) + deltaX);
                Canvas.SetTop(clickableSection, Canvas.GetTop(clickableSection) + deltaY);
                //202
                Canvas.SetLeft(clickableSection_Copy, Canvas.GetLeft(clickableSection_Copy) + deltaX);
                Canvas.SetTop(clickableSection_Copy, Canvas.GetTop(clickableSection_Copy) + deltaY);
                //203
                Canvas.SetLeft(clickableSection_Copy1, Canvas.GetLeft(clickableSection_Copy1) + deltaX);
                Canvas.SetTop(clickableSection_Copy1, Canvas.GetTop(clickableSection_Copy1) + deltaY);
                //204
                Canvas.SetLeft(clickableSection_Copy2, Canvas.GetLeft(clickableSection_Copy2) + deltaX);
                Canvas.SetTop(clickableSection_Copy2, Canvas.GetTop(clickableSection_Copy2) + deltaY);
                //205
                Canvas.SetLeft(clickableSection_Copy3, Canvas.GetLeft(clickableSection_Copy3) + deltaX);
                Canvas.SetTop(clickableSection_Copy3, Canvas.GetTop(clickableSection_Copy3) + deltaY);
                //206
                Canvas.SetLeft(clickableSection_Copy4, Canvas.GetLeft(clickableSection_Copy4) + deltaX);
                Canvas.SetTop(clickableSection_Copy4, Canvas.GetTop(clickableSection_Copy4) + deltaY);
                //207
                Canvas.SetLeft(clickableSection_Copy5, Canvas.GetLeft(clickableSection_Copy5) + deltaX);
                Canvas.SetTop(clickableSection_Copy5, Canvas.GetTop(clickableSection_Copy5) + deltaY);
                //208
                Canvas.SetLeft(clickableSection_Copy6, Canvas.GetLeft(clickableSection_Copy6) + deltaX);
                Canvas.SetTop(clickableSection_Copy6, Canvas.GetTop(clickableSection_Copy6) + deltaY);
                //209
                Canvas.SetLeft(clickableSection_Copy7, Canvas.GetLeft(clickableSection_Copy7) + deltaX);
                Canvas.SetTop(clickableSection_Copy7, Canvas.GetTop(clickableSection_Copy7) + deltaY);
                //210
                Canvas.SetLeft(clickableSection_Copy8, Canvas.GetLeft(clickableSection_Copy8) + deltaX);
                Canvas.SetTop(clickableSection_Copy8, Canvas.GetTop(clickableSection_Copy8) + deltaY);
                //211
                Canvas.SetLeft(clickableSection_Copy9, Canvas.GetLeft(clickableSection_Copy9) + deltaX);
                Canvas.SetTop(clickableSection_Copy9, Canvas.GetTop(clickableSection_Copy9) + deltaY);
                //212
                Canvas.SetLeft(clickableSection_Copy10, Canvas.GetLeft(clickableSection_Copy10) + deltaX);
                Canvas.SetTop(clickableSection_Copy10, Canvas.GetTop(clickableSection_Copy10) + deltaY);
                //213
                Canvas.SetLeft(clickableSection_Copy11, Canvas.GetLeft(clickableSection_Copy11) + deltaX);
                Canvas.SetTop(clickableSection_Copy11, Canvas.GetTop(clickableSection_Copy11) + deltaY);

                ResetInteractionTimer();
            }
        }

        private void ZoomResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Check if the image is already at its original size
            if (image.Width == originalWidth && image.Height == originalHeight)
            {
                // Reset the position of the image to its original position
                Canvas.SetLeft(image, originalImageLeft);
                Canvas.SetTop(image, originalImageTop);
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

            // Reset the position and size of the clickable section to its original values
            Canvas.SetLeft(clickableSection, originalClickableSectionLeft);
            Canvas.SetTop(clickableSection, originalClickableSectionTop);
            clickableSection.Width = originalClickableSectionWidth;
            clickableSection.Height = originalClickableSectionHeight;
            //202
            Canvas.SetLeft(clickableSection_Copy, originalClickableSectionCopyLeft);
            Canvas.SetTop(clickableSection_Copy, originalClickableSectionCopyTop);
            clickableSection_Copy.Width = originalClickableSectionCopyWidth;
            clickableSection_Copy.Height = originalClickableSectionCopyHeight;
            //203
            Canvas.SetLeft(clickableSection_Copy1, originalClickableSectionCopyLeft303);
            Canvas.SetTop(clickableSection_Copy1, originalClickableSectionCopyTop303);
            clickableSection_Copy1.Width = originalClickableSectionCopyWidth303;
            clickableSection_Copy1.Height = originalClickableSectionCopyHeight303;
            //204
            Canvas.SetLeft(clickableSection_Copy2, originalClickableSectionCopyLeft304);
            Canvas.SetTop(clickableSection_Copy2, originalClickableSectionCopyTop304);
            clickableSection_Copy2.Width = originalClickableSectionCopyWidth304;
            clickableSection_Copy2.Height = originalClickableSectionCopyHeight304;
            //205
            Canvas.SetLeft(clickableSection_Copy3, originalClickableSectionCopyLeft305);
            Canvas.SetTop(clickableSection_Copy3, originalClickableSectionCopyTop305);
            clickableSection_Copy3.Width = originalClickableSectionCopyWidth305;
            clickableSection_Copy3.Height = originalClickableSectionCopyHeight305;
            //206
            Canvas.SetLeft(clickableSection_Copy4, originalClickableSectionCopyLeft306);
            Canvas.SetTop(clickableSection_Copy4, originalClickableSectionCopyTop306);
            clickableSection_Copy4.Width = originalClickableSectionCopyWidth306;
            clickableSection_Copy4.Height = originalClickableSectionCopyHeight306;
            //207
            Canvas.SetLeft(clickableSection_Copy5, originalClickableSectionCopyLeft307);
            Canvas.SetTop(clickableSection_Copy5, originalClickableSectionCopyTop307);
            clickableSection_Copy5.Width = originalClickableSectionCopyWidth307;
            clickableSection_Copy5.Height = originalClickableSectionCopyHeight307;
            //208
            Canvas.SetLeft(clickableSection_Copy6, originalClickableSectionCopyLeft3072);
            Canvas.SetTop(clickableSection_Copy6, originalClickableSectionCopyTop3072);
            clickableSection_Copy6.Width = originalClickableSectionCopyWidth3072;
            clickableSection_Copy6.Height = originalClickableSectionCopyHeight3072;
            //209
            Canvas.SetLeft(clickableSection_Copy7, originalClickableSectionCopyLeft308);
            Canvas.SetTop(clickableSection_Copy7, originalClickableSectionCopyTop308);
            clickableSection_Copy7.Width = originalClickableSectionCopyWidth308;
            clickableSection_Copy7.Height = originalClickableSectionCopyHeight308;
            //210
            Canvas.SetLeft(clickableSection_Copy8, originalClickableSectionCopyLeft309);
            Canvas.SetTop(clickableSection_Copy8, originalClickableSectionCopyTop309);
            clickableSection_Copy8.Width = originalClickableSectionCopyWidth309;
            clickableSection_Copy8.Height = originalClickableSectionCopyHeight309;
            //211
            Canvas.SetLeft(clickableSection_Copy9, originalClickableSectionCopyLeft310);
            Canvas.SetTop(clickableSection_Copy9, originalClickableSectionCopyTop310);
            clickableSection_Copy9.Width = originalClickableSectionCopyWidth310;
            clickableSection_Copy9.Height = originalClickableSectionCopyHeight310;
            //212
            Canvas.SetLeft(clickableSection_Copy10, originalClickableSectionCopyLeft311);
            Canvas.SetTop(clickableSection_Copy10, originalClickableSectionCopyTop311);
            clickableSection_Copy10.Width = originalClickableSectionCopyWidth311;
            clickableSection_Copy10.Height = originalClickableSectionCopyHeight311;
            //213
            Canvas.SetLeft(clickableSection_Copy11, originalClickableSectionCopyLeft312);
            Canvas.SetTop(clickableSection_Copy11, originalClickableSectionCopyTop312);
            clickableSection_Copy11.Width = originalClickableSectionCopyWidth312;
            clickableSection_Copy11.Height = originalClickableSectionCopyHeight312;

            ShowArrows();

            // Reset the screensaver timer
            ResetInteractionTimer();
        }

        private void ApplyZoom(double scale)
        {
            HideArrows();
            image.Width = originalWidth * scale;
            image.Height = originalHeight * scale;
            //201
            Canvas.SetLeft(clickableSection, originalClickableSectionLeft * scale);
            Canvas.SetTop(clickableSection, originalClickableSectionTop * scale);
            clickableSection.Width = originalClickableSectionWidth * scale;
            clickableSection.Height = originalClickableSectionHeight * scale;
            //202
            Canvas.SetLeft(clickableSection_Copy, originalClickableSectionCopyLeft * scale);
            Canvas.SetTop(clickableSection_Copy, originalClickableSectionCopyTop * scale);
            clickableSection_Copy.Width = originalClickableSectionCopyWidth * scale;
            clickableSection_Copy.Height = originalClickableSectionCopyHeight * scale;
            //203
            Canvas.SetLeft(clickableSection_Copy1, originalClickableSectionCopyLeft303 * scale);
            Canvas.SetTop(clickableSection_Copy1, originalClickableSectionCopyTop303 * scale);
            clickableSection_Copy1.Width = originalClickableSectionCopyWidth303 * scale;
            clickableSection_Copy1.Height = originalClickableSectionCopyHeight303 * scale;
            //204
            Canvas.SetLeft(clickableSection_Copy2, originalClickableSectionCopyLeft304 * scale);
            Canvas.SetTop(clickableSection_Copy2, originalClickableSectionCopyTop304 * scale);
            clickableSection_Copy2.Width = originalClickableSectionCopyWidth304 * scale;
            clickableSection_Copy2.Height = originalClickableSectionCopyHeight304 * scale;
            //205
            Canvas.SetLeft(clickableSection_Copy3, originalClickableSectionCopyLeft305 * scale);
            Canvas.SetTop(clickableSection_Copy3, originalClickableSectionCopyTop305 * scale);
            clickableSection_Copy3.Width = originalClickableSectionCopyWidth305 * scale;
            clickableSection_Copy3.Height = originalClickableSectionCopyHeight305 * scale;
            //206
            Canvas.SetLeft(clickableSection_Copy4, originalClickableSectionCopyLeft306 * scale);
            Canvas.SetTop(clickableSection_Copy4, originalClickableSectionCopyTop306 * scale);
            clickableSection_Copy4.Width = originalClickableSectionCopyWidth306 * scale;
            clickableSection_Copy4.Height = originalClickableSectionCopyHeight306 * scale;
            //207
            Canvas.SetLeft(clickableSection_Copy5, originalClickableSectionCopyLeft307 * scale);
            Canvas.SetTop(clickableSection_Copy5, originalClickableSectionCopyTop307 * scale);
            clickableSection_Copy5.Width = originalClickableSectionCopyWidth307 * scale;
            clickableSection_Copy5.Height = originalClickableSectionCopyHeight307 * scale;
            //208
            Canvas.SetLeft(clickableSection_Copy6, originalClickableSectionCopyLeft3072 * scale);
            Canvas.SetTop(clickableSection_Copy6, originalClickableSectionCopyTop3072 * scale);
            clickableSection_Copy6.Width = originalClickableSectionCopyWidth3072 * scale;
            clickableSection_Copy6.Height = originalClickableSectionCopyHeight3072 * scale;
            //209
            Canvas.SetLeft(clickableSection_Copy7, originalClickableSectionCopyLeft308 * scale);
            Canvas.SetTop(clickableSection_Copy7, originalClickableSectionCopyTop308 * scale);
            clickableSection_Copy7.Width = originalClickableSectionCopyWidth308 * scale;
            clickableSection_Copy7.Height = originalClickableSectionCopyHeight308 * scale;
            //210
            Canvas.SetLeft(clickableSection_Copy8, originalClickableSectionCopyLeft309 * scale);
            Canvas.SetTop(clickableSection_Copy8, originalClickableSectionCopyTop309 * scale);
            clickableSection_Copy8.Width = originalClickableSectionCopyWidth309 * scale;
            clickableSection_Copy8.Height = originalClickableSectionCopyHeight309 * scale;
            //211
            Canvas.SetLeft(clickableSection_Copy9, originalClickableSectionCopyLeft310 * scale);
            Canvas.SetTop(clickableSection_Copy9, originalClickableSectionCopyTop310 * scale);
            clickableSection_Copy9.Width = originalClickableSectionCopyWidth310 * scale;
            clickableSection_Copy9.Height = originalClickableSectionCopyHeight310 * scale;
            //312
            Canvas.SetLeft(clickableSection_Copy10, originalClickableSectionCopyLeft311 * scale);
            Canvas.SetTop(clickableSection_Copy10, originalClickableSectionCopyTop311 * scale);
            clickableSection_Copy10.Width = originalClickableSectionCopyWidth311 * scale;
            clickableSection_Copy10.Height = originalClickableSectionCopyHeight311 * scale;
            //313
            Canvas.SetLeft(clickableSection_Copy11, originalClickableSectionCopyLeft312 * scale);
            Canvas.SetTop(clickableSection_Copy11, originalClickableSectionCopyTop312 * scale);
            clickableSection_Copy11.Width = originalClickableSectionCopyWidth312 * scale;
            clickableSection_Copy11.Height = originalClickableSectionCopyHeight312 * scale;
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            if (originalScale < maxScale)
            {
                HideArrows();
                originalScale += 0.1;
                ApplyZoom(originalScale);
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
            interactionTimer.Stop();
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

        #region navbar
        private void ground_btn(object sender, RoutedEventArgs e)
        {
            interactionTimer.Stop();
            Building b1 = new Building();
            b1.Show();
            this.Close();
        }
        private void home_btn(object sender, RoutedEventArgs e)
        {
            interactionTimer.Stop();
            Home h1 = new Home();
            h1.Show();
            this.Close();
        }

        private void announcement_btn(object sender, RoutedEventArgs e)
        {
            interactionTimer.Stop();
            Announcement a1 = new Announcement();
            a1.Show();
            this.Close();
        }
        private void instructor_schedule_btn(object sender, RoutedEventArgs e)
        {
            interactionTimer.Stop();
            ProfessorSchedule ps = new ProfessorSchedule();
            ps.Show();
            this.Close();
        }

        private void mezzanine_btn(object sender, RoutedEventArgs e)
        {
            interactionTimer.Stop();
            Mezzanine m1 = new Mezzanine();
            m1.Show();
            this.Close();
        }
        private void floor3_btn(object sender, RoutedEventArgs e)
        {
            interactionTimer.Stop();
            Floor3 floor3 = new Floor3();
            floor3.Show();
            this.Close();
        }

        private void floor4_btn(object sender, RoutedEventArgs e)
        {
            interactionTimer.Stop();
            Floor4 floor4 = new Floor4();
            floor4.Show();
            this.Close();
        }

        private void roofdeck_btn(object sender, RoutedEventArgs e)
        {
            interactionTimer.Stop();
            Roofdeck roofdeck = new Roofdeck();
            roofdeck.Show();
            this.Close();
        }
        private void court_btn(object sender, RoutedEventArgs e)
        {
            interactionTimer.Stop();
            Court court = new Court();
            court.Show();
            this.Close();
        }

        #endregion

        private void room201(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(1, "Room 201 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room202(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(2, "Room 202 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room203(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(3, "Room 203 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room204(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(4, "Room 204 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room205(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(5, "Room 205 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room206(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(6, "Room 206 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room207(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(7, "Room 207 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room208(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(8, "Room 208 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room209(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(9, "Room 209 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room210(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(10, "Room 210 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room211(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(11, "Room 211 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room212(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(12, "Room 212 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
        private void room213(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            Floor2Schedules floor2 = new Floor2Schedules(13, "Room 213 Schedules");
            floor2.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            floor2.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            floor2.ShowDialog();
        }
    }
}
