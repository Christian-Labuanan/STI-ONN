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
using static System.Net.Mime.MediaTypeNames;

namespace STI_ONN
{
    public partial class Floor3 : Window
    {
        private double originalWidth;
        private double originalHeight;
        private double squareWidth;
        private double squareHeight;
        private double originalImageLeft;
        private double originalImageTop;

        private double originalScale = 1.0;
        //301
        private double originalClickableSectionLeft;
        private double originalClickableSectionTop;
        private double originalClickableSectionWidth;
        private double originalClickableSectionHeight;
        //302
        private double originalClickableSectionCopyLeft;
        private double originalClickableSectionCopyTop;
        private double originalClickableSectionCopyWidth;
        private double originalClickableSectionCopyHeight;
        //303
        private double originalClickableSectionCopyLeft303;
        private double originalClickableSectionCopyTop303;
        private double originalClickableSectionCopyWidth303;
        private double originalClickableSectionCopyHeight303;
        //304
        private double originalClickableSectionCopyLeft304;
        private double originalClickableSectionCopyTop304;
        private double originalClickableSectionCopyWidth304;
        private double originalClickableSectionCopyHeight304;
        //305
        private double originalClickableSectionCopyLeft305;
        private double originalClickableSectionCopyTop305;
        private double originalClickableSectionCopyWidth305;
        private double originalClickableSectionCopyHeight305;
        //305
        private double originalClickableSectionCopyLeft306;
        private double originalClickableSectionCopyTop306;
        private double originalClickableSectionCopyWidth306;
        private double originalClickableSectionCopyHeight306;

        private bool isDragging = false;
        private Point lastPosition;

        private DispatcherTimer interactionTimer;
        public Floor3()
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
            //302
            originalClickableSectionCopyLeft = Canvas.GetLeft(clickableSection_Copy);
            originalClickableSectionCopyTop = Canvas.GetTop(clickableSection_Copy);
            originalClickableSectionCopyWidth = clickableSection_Copy.Width;
            originalClickableSectionCopyHeight = clickableSection_Copy.Height;
            //303
            originalClickableSectionCopyLeft303 = Canvas.GetLeft(clickableSection_Copy1);
            originalClickableSectionCopyTop303 = Canvas.GetTop(clickableSection_Copy1);
            originalClickableSectionCopyWidth303 = clickableSection_Copy1.Width;
            originalClickableSectionCopyHeight303 = clickableSection_Copy1.Height;
            //304
            originalClickableSectionCopyLeft304 = Canvas.GetLeft(clickableSection_Copy2);
            originalClickableSectionCopyTop304 = Canvas.GetTop(clickableSection_Copy2);
            originalClickableSectionCopyWidth304 = clickableSection_Copy2.Width;
            originalClickableSectionCopyHeight304 = clickableSection_Copy2.Height;
            //305
            originalClickableSectionCopyLeft305 = Canvas.GetLeft(clickableSection_Copy3);
            originalClickableSectionCopyTop305 = Canvas.GetTop(clickableSection_Copy3);
            originalClickableSectionCopyWidth305 = clickableSection_Copy3.Width;
            originalClickableSectionCopyHeight305 = clickableSection_Copy3.Height;
            //306
            originalClickableSectionCopyLeft306 = Canvas.GetLeft(clickableSection_Copy4);
            originalClickableSectionCopyTop306 = Canvas.GetTop(clickableSection_Copy4);
            originalClickableSectionCopyWidth306 = clickableSection_Copy4.Width;
            originalClickableSectionCopyHeight306 = clickableSection_Copy4.Height;


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
            //room301
            double sectionWidthAdjustment = clickableSection.Width * (scale - 1);
            double sectionHeightAdjustment = clickableSection.Height * (scale - 1);
            //room 302
            double sectionWidthAdjustment302 = clickableSection_Copy.Width * (scale - 1);
            double sectionHeightAdjustment302 = clickableSection_Copy.Height * (scale - 1);
            //room 303
            double sectionWidthAdjustment303 = clickableSection_Copy1.Width * (scale - 1);
            double sectionHeightAdjustment303 = clickableSection_Copy1.Height * (scale - 1);
            //room 304
            double sectionWidthAdjustment304 = clickableSection_Copy2.Width * (scale - 1);
            double sectionHeightAdjustment304 = clickableSection_Copy2.Height * (scale - 1);
            //room 305
            double sectionWidthAdjustment305 = clickableSection_Copy3.Width * (scale - 1);
            double sectionHeightAdjustment305 = clickableSection_Copy3.Height * (scale - 1);
            //room 306
            double sectionWidthAdjustment306 = clickableSection_Copy4.Width * (scale - 1);
            double sectionHeightAdjustment306 = clickableSection_Copy4.Height * (scale - 1);

            // Apply the new width and height to the image
            image.Width = newWidth;
            image.Height = newHeight;

            // Adjust the size of the clickable section
            //301
            clickableSection.Width += sectionWidthAdjustment;
            clickableSection.Height += sectionHeightAdjustment;
            //302
            clickableSection_Copy.Width += sectionWidthAdjustment302;
            clickableSection_Copy.Height += sectionHeightAdjustment302;
            //303
            clickableSection_Copy1.Width += sectionWidthAdjustment303;
            clickableSection_Copy1.Height += sectionHeightAdjustment303;
            //304
            clickableSection_Copy2.Width += sectionWidthAdjustment304;
            clickableSection_Copy2.Height += sectionHeightAdjustment304;
            //305
            clickableSection_Copy3.Width += sectionWidthAdjustment305;
            clickableSection_Copy3.Height += sectionHeightAdjustment305;
            //306
            clickableSection_Copy4.Width += sectionWidthAdjustment306;
            clickableSection_Copy4.Height += sectionHeightAdjustment306;

            // Calculate the new position of the clickable section relative to the image
            //301
            double sectionLeftRelativeToImage = Canvas.GetLeft(clickableSection) - Canvas.GetLeft(image);
            double sectionTopRelativeToImage = Canvas.GetTop(clickableSection) - Canvas.GetTop(image);
            //302
            double sectionLeftRelativeToImage302 = Canvas.GetLeft(clickableSection_Copy) - Canvas.GetLeft(image);
            double sectionTopRelativeToImage302 = Canvas.GetTop(clickableSection_Copy) - Canvas.GetTop(image);
            //303
            double sectionLeftRelativeToImage303 = Canvas.GetLeft(clickableSection_Copy1) - Canvas.GetLeft(image);
            double sectionTopRelativeToImage303 = Canvas.GetTop(clickableSection_Copy1) - Canvas.GetTop(image);
            //304
            double sectionLeftRelativeToImage304 = Canvas.GetLeft(clickableSection_Copy2) - Canvas.GetLeft(image);
            double sectionTopRelativeToImage304 = Canvas.GetTop(clickableSection_Copy2) - Canvas.GetTop(image);
            //305
            double sectionLeftRelativeToImage305 = Canvas.GetLeft(clickableSection_Copy3) - Canvas.GetLeft(image);
            double sectionTopRelativeToImage305 = Canvas.GetTop(clickableSection_Copy3) - Canvas.GetTop(image);
            //306
            double sectionLeftRelativeToImage306 = Canvas.GetLeft(clickableSection_Copy4) - Canvas.GetLeft(image);
            double sectionTopRelativeToImage306 = Canvas.GetTop(clickableSection_Copy4) - Canvas.GetTop(image);

            // Calculate the new position of the clickable section
            //301
            double newSectionLeftRelativeToImage = sectionLeftRelativeToImage * scale;
            double newSectionTopRelativeToImage = sectionTopRelativeToImage * scale;
            //302
            double newSectionLeftRelativeToImage302 = sectionLeftRelativeToImage302 * scale;
            double newSectionTopRelativeToImage302 = sectionTopRelativeToImage302 * scale;
            //303
            double newSectionLeftRelativeToImage303 = sectionLeftRelativeToImage303 * scale;
            double newSectionTopRelativeToImage303 = sectionTopRelativeToImage303 * scale;
            //304
            double newSectionLeftRelativeToImage304 = sectionLeftRelativeToImage304 * scale;
            double newSectionTopRelativeToImage304 = sectionTopRelativeToImage304 * scale;
            //305
            double newSectionLeftRelativeToImage305 = sectionLeftRelativeToImage305 * scale;
            double newSectionTopRelativeToImage305 = sectionTopRelativeToImage305 * scale;
            //306
            double newSectionLeftRelativeToImage306 = sectionLeftRelativeToImage306 * scale;
            double newSectionTopRelativeToImage306 = sectionTopRelativeToImage306 * scale;

            // Apply the new position of the clickable section
            Canvas.SetLeft(clickableSection, Canvas.GetLeft(image) + newSectionLeftRelativeToImage);
            Canvas.SetTop(clickableSection, Canvas.GetTop(image) + newSectionTopRelativeToImage);
            //302
            Canvas.SetLeft(clickableSection_Copy, Canvas.GetLeft(image) + newSectionLeftRelativeToImage302);
            Canvas.SetTop(clickableSection_Copy, Canvas.GetTop(image) + newSectionTopRelativeToImage302);
            //303
            Canvas.SetLeft(clickableSection_Copy1, Canvas.GetLeft(image) + newSectionLeftRelativeToImage303);
            Canvas.SetTop(clickableSection_Copy1, Canvas.GetTop(image) + newSectionTopRelativeToImage303);
            //304
            Canvas.SetLeft(clickableSection_Copy2, Canvas.GetLeft(image) + newSectionLeftRelativeToImage304);
            Canvas.SetTop(clickableSection_Copy2, Canvas.GetTop(image) + newSectionTopRelativeToImage304);
            //305
            Canvas.SetLeft(clickableSection_Copy3, Canvas.GetLeft(image) + newSectionLeftRelativeToImage305);
            Canvas.SetTop(clickableSection_Copy3, Canvas.GetTop(image) + newSectionTopRelativeToImage305);
            //306
            Canvas.SetLeft(clickableSection_Copy4, Canvas.GetLeft(image) + newSectionLeftRelativeToImage306);
            Canvas.SetTop(clickableSection_Copy4, Canvas.GetTop(image) + newSectionTopRelativeToImage306);

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

                //301
                Canvas.SetLeft(clickableSection, Canvas.GetLeft(clickableSection) + deltaX);
                Canvas.SetTop(clickableSection, Canvas.GetTop(clickableSection) + deltaY);
                //302
                Canvas.SetLeft(clickableSection_Copy, Canvas.GetLeft(clickableSection_Copy) + deltaX);
                Canvas.SetTop(clickableSection_Copy, Canvas.GetTop(clickableSection_Copy) + deltaY);
                //303
                Canvas.SetLeft(clickableSection_Copy1, Canvas.GetLeft(clickableSection_Copy1) + deltaX);
                Canvas.SetTop(clickableSection_Copy1, Canvas.GetTop(clickableSection_Copy1) + deltaY);
                //304
                Canvas.SetLeft(clickableSection_Copy2, Canvas.GetLeft(clickableSection_Copy2) + deltaX);
                Canvas.SetTop(clickableSection_Copy2, Canvas.GetTop(clickableSection_Copy2) + deltaY);
                //305
                Canvas.SetLeft(clickableSection_Copy3, Canvas.GetLeft(clickableSection_Copy3) + deltaX);
                Canvas.SetTop(clickableSection_Copy3, Canvas.GetTop(clickableSection_Copy3) + deltaY);
                //306
                Canvas.SetLeft(clickableSection_Copy4, Canvas.GetLeft(clickableSection_Copy4) + deltaX);
                Canvas.SetTop(clickableSection_Copy4, Canvas.GetTop(clickableSection_Copy4) + deltaY);

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
            //302
            Canvas.SetLeft(clickableSection_Copy, originalClickableSectionCopyLeft);
            Canvas.SetTop(clickableSection_Copy, originalClickableSectionCopyTop);
            clickableSection_Copy.Width = originalClickableSectionCopyWidth;
            clickableSection_Copy.Height = originalClickableSectionCopyHeight;
            //303
            Canvas.SetLeft(clickableSection_Copy1, originalClickableSectionCopyLeft303);
            Canvas.SetTop(clickableSection_Copy1, originalClickableSectionCopyTop303);
            clickableSection_Copy1.Width = originalClickableSectionCopyWidth303;
            clickableSection_Copy1.Height = originalClickableSectionCopyHeight303;
            //304
            Canvas.SetLeft(clickableSection_Copy2, originalClickableSectionCopyLeft304);
            Canvas.SetTop(clickableSection_Copy2, originalClickableSectionCopyTop304);
            clickableSection_Copy2.Width = originalClickableSectionCopyWidth304;
            clickableSection_Copy2.Height = originalClickableSectionCopyHeight304;
            //305
            Canvas.SetLeft(clickableSection_Copy3, originalClickableSectionCopyLeft305);
            Canvas.SetTop(clickableSection_Copy3, originalClickableSectionCopyTop305);
            clickableSection_Copy3.Width = originalClickableSectionCopyWidth305;
            clickableSection_Copy3.Height = originalClickableSectionCopyHeight305;
            //306
            Canvas.SetLeft(clickableSection_Copy4, originalClickableSectionCopyLeft306);
            Canvas.SetTop(clickableSection_Copy4, originalClickableSectionCopyTop306);
            clickableSection_Copy4.Width = originalClickableSectionCopyWidth306;
            clickableSection_Copy4.Height = originalClickableSectionCopyHeight306;

            // Reset the screensaver timer
            ResetInteractionTimer();
        }

        private void ApplyZoom(double scale)
        {
            image.Width = originalWidth * scale;
            image.Height = originalHeight * scale;
            //301
            Canvas.SetLeft(clickableSection, originalClickableSectionLeft * scale);
            Canvas.SetTop(clickableSection, originalClickableSectionTop * scale);
            clickableSection.Width = originalClickableSectionWidth * scale;
            clickableSection.Height = originalClickableSectionHeight * scale;
            //302
            Canvas.SetLeft(clickableSection_Copy, originalClickableSectionCopyLeft * scale);
            Canvas.SetTop(clickableSection_Copy, originalClickableSectionCopyTop * scale);
            clickableSection_Copy.Width = originalClickableSectionCopyWidth * scale;
            clickableSection_Copy.Height = originalClickableSectionCopyHeight * scale;
            //303
            Canvas.SetLeft(clickableSection_Copy1, originalClickableSectionCopyLeft303 * scale);
            Canvas.SetTop(clickableSection_Copy1, originalClickableSectionCopyTop303 * scale);
            clickableSection_Copy1.Width = originalClickableSectionCopyWidth303 * scale;
            clickableSection_Copy1.Height = originalClickableSectionCopyHeight303 * scale;
            //304
            Canvas.SetLeft(clickableSection_Copy2, originalClickableSectionCopyLeft304 * scale);
            Canvas.SetTop(clickableSection_Copy2, originalClickableSectionCopyTop304 * scale);
            clickableSection_Copy2.Width = originalClickableSectionCopyWidth304 * scale;
            clickableSection_Copy2.Height = originalClickableSectionCopyHeight304 * scale;
            //305
            Canvas.SetLeft(clickableSection_Copy3, originalClickableSectionCopyLeft305 * scale);
            Canvas.SetTop(clickableSection_Copy3, originalClickableSectionCopyTop305 * scale);
            clickableSection_Copy3.Width = originalClickableSectionCopyWidth305 * scale;
            clickableSection_Copy3.Height = originalClickableSectionCopyHeight305 * scale;
            //306
            Canvas.SetLeft(clickableSection_Copy4, originalClickableSectionCopyLeft306 * scale);
            Canvas.SetTop(clickableSection_Copy4, originalClickableSectionCopyTop306 * scale);
            clickableSection_Copy4.Width = originalClickableSectionCopyWidth306 * scale;
            clickableSection_Copy4.Height = originalClickableSectionCopyHeight306 * scale;
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

        private void InteractionTimer_Tick(object? sender, EventArgs e)
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

        #region navbarleft
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
        private void ground_btn(object sender, RoutedEventArgs e)
        {
            Building b1 = new Building();
            b1.Show();
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
        #endregion

        private void room301(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            room301 rm301 = new room301(1,"Room 301 Schedules");
            rm301.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            rm301.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            rm301.ShowDialog();
        }
        private void room302(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            room301 rm301 = new room301(2, "Room 302 Schedules");
            rm301.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            rm301.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            rm301.ShowDialog();
        }
        private void room303(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            room301 rm301 = new room301(3, "Room 303 Schedules");
            rm301.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            rm301.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            rm301.ShowDialog();
        }
        private void room304(object sender, RoutedEventArgs e)
        {
            // Create and show the room 301 window
            room301 rm301 = new room301(4, "Room 304 Schedules");
            rm301.Owner = this;

            // Show the dimming overlay to dim the window
            dimmingOverlay.Visibility = Visibility.Visible;

            // Handle the Closed event of the room 301 window to hide the dimming overlay when the window is closed
            rm301.Closed += (s, args) =>
            {
                // Hide the dimming overlay
                dimmingOverlay.Visibility = Visibility.Collapsed;
            };

            // Show the room 301 window
            rm301.ShowDialog();
        }
    }
}
