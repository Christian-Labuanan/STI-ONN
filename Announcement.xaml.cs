using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Firebase.Database;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for Announcement.xaml
    /// </summary>
    public partial class Announcement : Window
    {
        private DispatcherTimer _refreshTimer;
        private DispatcherTimer _inactivityTimer;
        private readonly DispatcherTimer interactionTimer;

        public Announcement()
        {
            InitializeComponent();
            // Initialize and start the interaction timer
            interactionTimer = new DispatcherTimer();
            interactionTimer.Interval = TimeSpan.FromMinutes(0.5); // Set the timeout duration here
            ResetInteractionTimer();

            // Initialize the inactivity timer
            _inactivityTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(0.2) // Set the inactivity duration as needed
            };
            _inactivityTimer.Tick += OnInactivityTimerTick;
            _inactivityTimer.Start();

            // Set up a timer to refresh announcements at regular intervals
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(30) // Adjust the interval as needed
            };
            _refreshTimer.Tick += async (sender, args) => await LoadAnnouncements();
            _refreshTimer.Start();

            // Load announcements on startup
            LoadAnnouncements();

            // Handle user interaction events to reset the inactivity timer
            this.MouseMove += ResetInactivityTimer;
            this.KeyDown += ResetInactivityTimer;
            this.MouseDown += ResetInactivityTimer;
            this.GotFocus += ResetInactivityTimer;
            this.MouseLeftButtonDown += ResetInactivityTimer;
            this.MouseLeftButtonUp += ResetInactivityTimer;
            this.MouseEnter += ResetInactivityTimer; // Reset on mouse entering the window
        }


        // Initializes timers and sets their properties
        private void InitializeTimers()
        {
            _inactivityTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(0.2)
            };
            _inactivityTimer.Tick += OnInactivityTimerTick;
            _inactivityTimer.Start();

            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(30)
            };
            _refreshTimer.Tick += async (sender, args) => await LoadAnnouncements();
            _refreshTimer.Start();
        }

        // Registers UI interaction events to reset the inactivity timer
        private void RegisterInteractionHandlers()
        {
            MouseMove += ResetInactivityTimer;
            KeyDown += ResetInactivityTimer;
            MouseDown += ResetInactivityTimer;
            GotFocus += ResetInactivityTimer;
            MouseLeftButtonDown += ResetInactivityTimer;
            MouseLeftButtonUp += ResetInactivityTimer;
            MouseRightButtonDown += ResetInactivityTimer;
            MouseRightButtonUp += ResetInactivityTimer;
        }

        // Fetches announcements from Firebase
        private async Task<List<AnnouncementItem>> GetAnnouncementsFromFirebase()
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://sti-onn-d0161-default-rtdb.asia-southeast1.firebasedatabase.app/");
                var announcementList = await firebaseClient.Child("posts").OnceAsync<AnnouncementItem>();

                return announcementList.Select(item => new AnnouncementItem
                {
                    ImageUrl = item.Object.ImageUrl,
                    Text = RemoveHtmlTags(item.Object.Text),
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

        // Removes HTML tags from the given input string
        private static string RemoveHtmlTags(string input)
        {
            return string.IsNullOrWhiteSpace(input) ? input : Regex.Replace(input, "<.*?>", string.Empty);
        }

        // Loads announcements from Firebase and updates the UI
        private async Task LoadAnnouncements()
        {
            var announcements = await GetAnnouncementsFromFirebase();
            PopulateAnnouncements(announcements);
        }

        // Populates the UI with announcement cards
        private void PopulateAnnouncements(List<AnnouncementItem> announcements)
        {
            AnnouncementWrapPanel.Children.Clear();

            if (announcements.Count == 0)
            {
                DisplayNoAnnouncementsMessage();
                return;
            }

            foreach (var announcement in announcements)
            {
                var card = CreateAnnouncementCard(announcement);
                AnnouncementWrapPanel.Children.Add(card);
            }
        }

        // Displays a message indicating no announcements are available
        private void DisplayNoAnnouncementsMessage()
        {
            var noAnnouncementsText = new TextBlock
            {
                Text = "No announcements available.",
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20),
                Foreground = Brushes.Gray
            };
            AnnouncementWrapPanel.Children.Add(noAnnouncementsText);
        }

        // Creates a card UI element for the given announcement
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

            var cardStackPanel = new StackPanel
            {
                Children =
                {
                    CreateTitleTextBlock(announcement.Title),
                    CreateAnnouncementImage(announcement.ImageUrl),
                    CreateDetailsButton(announcement)
                }
            };

            cardBorder.Child = cardStackPanel;
            return cardBorder;
        }

        // Creates a TextBlock for the announcement title
        private static TextBlock CreateTitleTextBlock(string title)
        {
            return new TextBlock
            {
                Text = title,
                FontWeight = FontWeights.Bold,
                FontSize = 18,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };
        }

        // Creates an Image element for the announcement image
        private Image CreateAnnouncementImage(string imageUrl)
        {
            try
            {
                return new Image
                {
                    Source = new BitmapImage(new Uri(imageUrl)),
                    Height = 150,
                    Stretch = Stretch.Uniform,
                    Margin = new Thickness(10),
                    VerticalAlignment = VerticalAlignment.Top
                };
            }
            catch (UriFormatException)
            {
                return new Image
                {
                    Source = new BitmapImage(new Uri("/assets/default-image.png", UriKind.Relative)),
                    Height = 150,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(10),
                    VerticalAlignment = VerticalAlignment.Top
                };
            }
        }

        // Creates a button to view details of the announcement
        private Button CreateDetailsButton(AnnouncementItem announcement)
        {
            var button = new Button
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
            button.Click += (s, e) => OpenAnnouncementDetail(announcement);
            return button;
        }

        // Opens the announcement detail window and shows the overlay
        private void OpenAnnouncementDetail(AnnouncementItem announcement)
        {
            Overlay.Visibility = Visibility.Visible;

            var announcementDetailWindow = new AnnouncementDetail(announcement);
            announcementDetailWindow.Closed += (sender, args) => Overlay.Visibility = Visibility.Collapsed;

            announcementDetailWindow.ShowDialog(); // Open as modal to prevent interactions with other windows
        }

        // Handles the inactivity timer tick event
        private void OnInactivityTimerTick(object sender, EventArgs e)
        {
            _inactivityTimer.Stop(); // Stop the timer to prevent re-entering this event
            ReturnToMainWindow(); // Call the method to go back to the main window
        }

        private void ResetInteractionTimer()
        {
            // Reset the interaction timer
            interactionTimer.Stop();
            interactionTimer.Start();
        }

        // Returns to the main window
        private void ReturnToMainWindow()
        {
            this.Close(); // Close the Announcement window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show(); // Show the MainWindow
            interactionTimer.Stop();
        }

        // Resets the inactivity timer on user interaction
        private void ResetInactivityTimer(object sender, EventArgs e)
        {
            _inactivityTimer.Stop();
            _inactivityTimer.Start(); // Restart the timer on any interaction
            ResetInteractionTimer(); // Ensure interaction timer is also reset
        }
        // Handles touch button click event to navigate to the Home window
        private void touch_Click_1(object sender, RoutedEventArgs e)
        {
            var homeWindow = new Home();
            homeWindow.Show();
            Hide(); // Hides the current Announcement window
        }

        // Prevents actions when the overlay is clicked
        private void Overlay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        // Represents an announcement item
        public class AnnouncementItem
        {
            public string ImageUrl { get; set; }
            public string Text { get; set; }
            public string Timestamp { get; set; }
            public string Title { get; set; }
        }
    }
}