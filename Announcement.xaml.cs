using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Firebase.Database;
using System.Text.RegularExpressions;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for Announcement.xaml
    /// </summary>
    public partial class Announcement : Window
    {
        private readonly DispatcherTimer _refreshTimer;
        private readonly DispatcherTimer _inactivityTimer; // New inactivity timer
        private readonly DispatcherTimer interactionTimer; // Timer for user interaction
        private const double InactivityTimeout = 0.5; // Inactivity timeout in minutes
        private const double InteractionTimeout = 0.5; // Interaction timeout in minutes

        public Announcement()
        {
            InitializeComponent();

            // Set up a timer to refresh announcements at regular intervals
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(0.5) // Adjust the interval as needed
            };
            _refreshTimer.Tick += async (sender, args) => await LoadAnnouncements();
            _refreshTimer.Start();

            // Initialize the inactivity timer
            _inactivityTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(InactivityTimeout) // Set inactivity timeout duration
            };
            _inactivityTimer.Tick += (sender, args) => Close(); // Close window after inactivity
            _inactivityTimer.Start();

            // Initialize the interaction timer
            interactionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(InteractionTimeout) // Set interaction timeout duration
            };
            interactionTimer.Tick += InteractionTimer_Tick; // Handle interaction timer ticks
            interactionTimer.Start();

            // Load initial announcements on startup
            LoadAnnouncements();

            // Reset timers on user interaction
            this.MouseMove += ResetInteractionTimer; // Track mouse movement
            this.KeyDown += ResetInteractionTimer; // Track key presses
            this.Loaded += Window_Loaded; // Track when the window is loaded

        }

        // Removes HTML tags from the provided string
        private string RemoveHtmlTags(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? input : Regex.Replace(input, "<.*?>", string.Empty);
        }

        // Fetches announcements from Firebase
        private async Task<List<AnnouncementItem>> GetAnnouncementsFromFirebase()
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://sti-onn-d0161-default-rtdb.asia-southeast1.firebasedatabase.app/");
                var announcementList = await firebaseClient
                    .Child("posts")
                    .OnceAsync<AnnouncementItem>();

                return announcementList.Select(item => new AnnouncementItem
                {
                    ImageUrl = item.Object.ImageUrl,
                    Text = RemoveHtmlTags(item.Object.Text), // Remove HTML tags from text
                    Timestamp = item.Object.Timestamp,
                    Title = item.Object.Title
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching announcements: " + ex.Message);
                return new List<AnnouncementItem>();
            }
        }

        // Loads and displays announcements on the UI
        private async Task LoadAnnouncements()
        {
            var announcements = await GetAnnouncementsFromFirebase();
            PopulateAnnouncements(announcements);
        }

        // Populates the WrapPanel with announcement cards
        private void PopulateAnnouncements(List<AnnouncementItem> announcements)
        {
            AnnouncementWrapPanel.Children.Clear(); // Clear existing items

            if (announcements.Count == 0)
            {
                // Display a message when no announcements are available
                var noAnnouncementsText = new TextBlock
                {
                    Text = "No announcements available.",
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(20),
                    Foreground = Brushes.Gray
                };
                AnnouncementWrapPanel.Children.Add(noAnnouncementsText);
                return;
            }

            foreach (var announcement in announcements)
            {
                var cardBorder = CreateAnnouncementCard(announcement);
                AnnouncementWrapPanel.Children.Add(cardBorder); // Add card to the panel
            }
        }

        // Creates an announcement card with a title, image, and button
        private Border CreateAnnouncementCard(AnnouncementItem announcement)
        {
            var cardBorder = new Border
            {
                Width = 350,
                Height = 300,
                Margin = new Thickness(15),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(15),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    BlurRadius = 8,
                    ShadowDepth = 2
                }
            };

            var cardStackPanel = new StackPanel();

            // Add title to the card
            var announcementTitle = new TextBlock
            {
                Text = announcement.Title,
                FontWeight = FontWeights.Bold,
                FontSize = 18,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };
            cardStackPanel.Children.Add(announcementTitle);

            // Add image to the card
            var announcementImage = CreateAnnouncementImage(announcement.ImageUrl);
            cardStackPanel.Children.Add(announcementImage);

            // Add a button to view details
            var fullScreenButton = new Button
            {
                Content = "View Details",
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Thickness(10),
                Background = Brushes.LightBlue,
                BorderBrush = Brushes.Transparent,
                FontSize = 14,
                Cursor = Cursors.Hand
            };
            fullScreenButton.Click += (s, e) => OpenAnnouncementDetail(announcement);
            cardStackPanel.Children.Add(fullScreenButton);

            cardBorder.Child = cardStackPanel;
            return cardBorder;
        }

        // Creates an Image element for the announcement card
        private Image CreateAnnouncementImage(string imageUrl)
        {
            Image announcementImage;
            try
            {
                announcementImage = new Image
                {
                    Source = new BitmapImage(new Uri(imageUrl)),
                    Height = 150,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(10),
                    VerticalAlignment = VerticalAlignment.Top
                };
            }
            catch (UriFormatException)
            {
                // Use a default image in case of an invalid URL
                announcementImage = new Image
                {
                    Source = new BitmapImage(new Uri("/assets/default-image.png", UriKind.Relative)),
                    Height = 150,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(10),
                    VerticalAlignment = VerticalAlignment.Top
                };
            }

            return announcementImage;
        }

        // Opens the announcement details window and shows the overlay
        private void OpenAnnouncementDetail(AnnouncementItem announcement)
        {
            Overlay.Visibility = Visibility.Visible;

            var announcementDetailWindow = new AnnouncementDetail(announcement);
            announcementDetailWindow.Closed += (sender, args) =>
            {
                // Hide the overlay when the detail window is closed
                Overlay.Visibility = Visibility.Collapsed;
            };

            announcementDetailWindow.ShowDialog(); // Open as modal to prevent interactions with other windows
        }

        // Navigation button click event
        private void touch_Click_1(object sender, RoutedEventArgs e)
        {
            Home home1 = new Home();
            home1.Show();
            this.Hide();
        }

        // Prevents any action when the overlay is clicked
        private void Overlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true; // Mark event as handled to prevent further processing
        }

        #region timer
        // Resets the interaction timer
        private void ResetInteractionTimer(object sender, EventArgs e) // Updated signature
        {
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

        // Event handler to reset the timer on window load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ResetInteractionTimer(null, null); // Call with null arguments
        }

        #endregion
        // Class to represent each announcement item
        public class AnnouncementItem
        {
            public string ImageUrl { get; set; }
            public string Text { get; set; }
            public string Timestamp { get; set; }
            public string Title { get; set; } // Required property
        }
    }
}