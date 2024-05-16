using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

            // Calculate the adjustment for the clickable section size and position
            double sectionWidthAdjustment = clickableSection.Width * (scale - 1);
            double sectionHeightAdjustment = clickableSection.Height * (scale - 1);

            // Apply the new width and height to the image
            image.Width = newWidth;
            image.Height = newHeight;

            // Adjust the size of the clickable section
            clickableSection.Width += sectionWidthAdjustment;
            clickableSection.Height += sectionHeightAdjustment;

            // Calculate the new position of the clickable section relative to the image
            double sectionLeftRelativeToImage = Canvas.GetLeft(clickableSection) - Canvas.GetLeft(image);
            double sectionTopRelativeToImage = Canvas.GetTop(clickableSection) - Canvas.GetTop(image);

            // Calculate the new position of the clickable section
            double newSectionLeftRelativeToImage = sectionLeftRelativeToImage * scale;
            double newSectionTopRelativeToImage = sectionTopRelativeToImage * scale;

            // Apply the new position of the clickable section
            Canvas.SetLeft(clickableSection, Canvas.GetLeft(image) + newSectionLeftRelativeToImage);
            Canvas.SetTop(clickableSection, Canvas.GetTop(image) + newSectionTopRelativeToImage);

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
            clickableSection.ReleaseMouseCapture();
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

                Canvas.SetLeft(clickableSection, Canvas.GetLeft(clickableSection) + deltaX);
                Canvas.SetTop(clickableSection, Canvas.GetTop(clickableSection) + deltaY);
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

            // Reset the position and size of the clickable section to its original values
            Canvas.SetLeft(clickableSection, originalClickableSectionLeft);
            Canvas.SetTop(clickableSection, originalClickableSectionTop);
            clickableSection.Width = originalClickableSectionWidth;
            clickableSection.Height = originalClickableSectionHeight;

            // Reset the screensaver timer
            ResetInteractionTimer();
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            // Zoom in by increasing the scale factor
            originalScale += 0.1;
            ApplyZoom();

            ResetInteractionTimer();
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            // Zoom out by decreasing the scale factor
            originalScale -= 0.1;
            ApplyZoom();

            ResetInteractionTimer();
        }

        private void ApplyZoom()
        {
            // Limit the scale factor to avoid extreme zoom levels
            originalScale = Math.Max(0.1, Math.Min(10.0, originalScale));

            // Calculate the new width and height after zooming
            double newWidth = originalWidth * originalScale;
            double newHeight = originalHeight * originalScale;

            // Calculate the new width and height for the clickable section
            double newSquareWidth = squareWidth * originalScale;
            double newSquareHeight = squareHeight * originalScale;

            // Calculate the zooming center point
            Point zoomCenter = new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2);

            // Calculate the current position of the clickable section relative to the image
            double sectionLeftRelativeToImage = Canvas.GetLeft(clickableSection) - Canvas.GetLeft(image);
            double sectionTopRelativeToImage = Canvas.GetTop(clickableSection) - Canvas.GetTop(image);

            // Calculate the new position of the clickable section relative to the image after zooming
            double newSectionLeftRelativeToImage = (sectionLeftRelativeToImage / image.Width) * newWidth;
            double newSectionTopRelativeToImage = (sectionTopRelativeToImage / image.Height) * newHeight;

            // Apply the scale transformation to the image
            image.Width = newWidth;
            image.Height = newHeight;

            // Apply the new size to the clickable section
            clickableSection.Width = newSquareWidth;
            clickableSection.Height = newSquareHeight;

            // Calculate the new position of the clickable section relative to the image
            double newSectionLeft = Canvas.GetLeft(image) + newSectionLeftRelativeToImage;
            double newSectionTop = Canvas.GetTop(image) + newSectionTopRelativeToImage;

            // Set the new position of the clickable section
            Canvas.SetLeft(clickableSection, newSectionLeft);
            Canvas.SetTop(clickableSection, newSectionTop);

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

        private void ClickableSection_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Room 101/ Bar");

            ResetInteractionTimer();
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
    }
}
