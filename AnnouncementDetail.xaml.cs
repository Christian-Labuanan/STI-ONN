using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static STI_ONN.Announcement;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for AnnouncementDetail.xaml
    /// </summary>
    public partial class AnnouncementDetail : Window
    {
        private readonly DispatcherTimer _inactivityTimer;
        public AnnouncementDetail(AnnouncementItem announcement)
        {
            InitializeComponent();

            // Set announcement title and text
            DetailTitle.Text = announcement.Title;
            DetailText.Text = RemoveHtmlTags(announcement.Text);

            // Set announcement image or use a default image if URL is invalid or empty
            SetAnnouncementImage(announcement.ImageUrl);

            // Center the window on the screen
            CenterWindow();

            // Initialize and start inactivity timer
            _inactivityTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(0.5) // Set the timer interval as needed
            };
            _inactivityTimer.Tick += OnInactivityTimerTick;
            _inactivityTimer.Start();

            // Handle window events to reset the inactivity timer
            this.MouseMove += ResetInactivityTimer;
            this.KeyDown += ResetInactivityTimer;

            // Prevent closing or hiding when clicking outside the window
            this.Deactivated += (s, e) => this.Activate();
        }

        /// <summary>
        /// Sets the image source for the detail window.
        /// </summary>
        /// <param name="imageUrl">The URL of the image to display.</param>
        private void SetAnnouncementImage(string imageUrl)
        {
            var defaultImageUri = new Uri("pack://application:,,,/assets/Icons/default-image.png");

            try
            {
                DetailImage.Source = !string.IsNullOrEmpty(imageUrl) ? new BitmapImage(new Uri(imageUrl)) : new BitmapImage(defaultImageUri);
            }
            catch (UriFormatException)
            {
                DetailImage.Source = new BitmapImage(defaultImageUri);
            }
        }

        /// <summary>
        /// Centers the AnnouncementDetail window on the screen.
        /// </summary>
        private void CenterWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Width = 600; // Set desired width
            this.Height = 400; // Set desired height

            // Position manually relative to the main window
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                this.Top = mainWindow.Top + (mainWindow.Height / 2) - (this.Height / 2);
                this.Left = mainWindow.Left + (mainWindow.Width / 2) - (this.Width / 2);
            }
        }

        /// <summary>
        /// Removes HTML tags from the provided text.
        /// </summary>
        /// <param name="text">The text with HTML tags.</param>
        /// <returns>Cleaned text without HTML tags.</returns>
        private string RemoveHtmlTags(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", string.Empty);
        }

        /// <summary>
        /// Handles the inactivity timer tick event.
        /// </summary>
        private void OnInactivityTimerTick(object sender, EventArgs e)
        {
            _inactivityTimer.Stop(); // Stop the timer to prevent re-entering this event
            this.Close(); // Close the window after the period of inactivity
        }
        /// <summary>
        /// Resets the inactivity timer on user interaction.
        /// </summary>
        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            _inactivityTimer.Stop();
            _inactivityTimer.Start(); // Restart the timer on any interaction
        }

        // Override OnMouseLeftButtonDown to prevent closing/hiding the window when clicking outside
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true; // Prevent any default behavior for clicking outside the window
        }
    }
}
