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
using System.Windows.Threading;

namespace STI_ONN
{
    public partial class Floor2 : Window
    {
        private double originalWidth;
        private double originalHeight;
        private double squareWidth;
        private double squareHeight;
        private double originalScale = 1.0;

        private bool isDragging = false;
        private Point lastPosition;

        private DispatcherTimer interactionTimer;

        public Floor2()
        {
            InitializeComponent();
            // Store the original size of the image
            originalWidth = image.Width;
            originalHeight = image.Height;
            squareWidth = clickableSection.Width;
            squareHeight = clickableSection.Height;

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

            // Calculate the zooming center point
            Point zoomCenter = e.GetPosition(image);

            // Calculate the new width and height after zooming
            double newWidth = image.Width * scale;
            double newHeight = image.Height * scale;

            // Calculate the adjustment for the left and top offsets to maintain the center position
            double deltaX = (newWidth - image.Width) * zoomCenter.X / image.Width;
            double deltaY = (newHeight - image.Height) * zoomCenter.Y / image.Height;

            // Apply the new width and height to the image
            image.Width = newWidth;
            image.Height = newHeight;

            // Adjust the left and top offsets to maintain the center position
            Canvas.SetLeft(image, Canvas.GetLeft(image) - deltaX);
            Canvas.SetTop(image, Canvas.GetTop(image) - deltaY);

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

            // Get the size of the Canvas
            double canvasWidth = canvas.ActualWidth;
            double canvasHeight = canvas.ActualHeight;

            // Calculate the new position to center the image within the Canvas
            double leftOffset = (canvasWidth - originalWidth) / 2;
            double topOffset = (canvasHeight - originalHeight) / 2;

            // Limit the offset to keep the image within the bounds
            leftOffset = Math.Max(0, leftOffset);
            topOffset = Math.Max(0, topOffset);

            // Set the new position of the image
            Canvas.SetLeft(image, leftOffset);
            Canvas.SetTop(image, topOffset);

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
    }
}
