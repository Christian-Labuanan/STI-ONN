using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using static STI_ONN.Announcement;


namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for AnnouncementDetail.xaml
    /// </summary>
    public partial class AnnouncementDetail : Window
    {
        private DispatcherTimer _inactivityTimer; // Inactivity timer
        public AnnouncementDetail(AnnouncementItem announcement)
        {
            InitializeComponent();
            // Initialize the inactivity timer
            _inactivityTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(0.5) // Set your inactivity duration
            };
            _inactivityTimer.Tick += OnInactivityTimerTick; // Event for inactivity timer
            _inactivityTimer.Start(); // Start the timer

            // Handle user interaction events to reset the inactivity timer
            this.MouseMove += ResetInactivityTimer; // Reset timer on mouse move
            this.MouseDown += ResetInactivityTimer; // Reset timer on mouse down
            this.KeyDown += ResetInactivityTimer; // Reset timer on key down
            this.PreviewMouseDown += ResetInactivityTimer; // Reset timer on mouse down (preview event)

            // Set announcement details
            DetailTitle.Text = announcement.Title;
            LoadImage(announcement.ImageUrl);
            DetailText.Text = RemoveHtmlTags(announcement.Text);

            // Load images and text from the announcement
            LoadAnnouncementContent(announcement);

            // Center window and set size
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Width = 600; // Adjust as necessary
            this.Height = 450; // Adjust as necessary
        }

        // Reset the inactivity timer on user interaction
        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            _inactivityTimer.Stop(); // Stop the timer to reset it
            _inactivityTimer.Start(); // Restart the timer
        }

        // Handles the inactivity timer tick event
        private void OnInactivityTimerTick(object sender, EventArgs e)
        {
            _inactivityTimer.Stop(); // Stop the timer to prevent re-entering this event
            this.Close(); // Close the AnnouncementDetail window
        }

        private void LoadAnnouncementContent(AnnouncementItem announcement)
        {
            // Load text without HTML tags
            string cleanText = RemoveHtmlTags(announcement.Text);
            DetailText.Text = cleanText;

            // Extract and load images from the text
            LoadImagesFromText(announcement.Text);
        }

        private void LoadImagesFromText(string text)
        {
            // Regular expression to find image URLs in the text
            var regex = new Regex(@"(https?://[^\s]+\.(jpg|jpeg|png|gif))", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                // Create an Image control for each URL found
                var image = new System.Windows.Controls.Image
                {
                    Source = new BitmapImage(new Uri(match.Value)),
                    Width = 300, // Set desired width
                    Height = 200, // Set desired height
                    Margin = new Thickness(5) // Add some margin
                };

                // Add the image to the layout (assuming you have a container like a StackPanel in your XAML)
                ImageContainer.Children.Add(image); // Make sure ImageContainer is the name of your panel in XAML
            }
        }

        private void LoadImage(string imageUrl)
        {
            try
            {
                DetailImage.Source = new BitmapImage(new Uri(imageUrl));
            }
            catch (UriFormatException)
            {
                DetailImage.Source = new BitmapImage(new Uri("pack://application:,,,/assets/Icons/default-image.png"));
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close the AnnouncementDetail window
        }

        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                CenterWindow();
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void CenterWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Top = (SystemParameters.PrimaryScreenHeight - this.Height) / 2;
            this.Left = (SystemParameters.PrimaryScreenWidth - this.Width) / 2;
        }

        private string RemoveHtmlTags(string text)
        {
            return Regex.Replace(text, "<.*?>", string.Empty);
        }
    }
}