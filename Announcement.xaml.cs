﻿using Firebase.Database;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for Announcement.xaml
    /// </summary>
    public partial class Announcement : Window
    {
        public Announcement()
        {
            InitializeComponent();
            // Load announcements on startup
            LoadAnnouncements();


        }

        private async Task LoadAnnouncements()
        {

            var loadingWindow = new Loading { LoadingMessage = "Loading Announcements, please wait..." };
            loadingWindow.Show();
            // Disable the main window to prevent interaction
            this.IsEnabled = false;

            var announcements = await GetAnnouncementsFromFirebase();
            PopulateAnnouncements(announcements);
            loadingWindow.Close();
            // Re-enable the main window
            this.IsEnabled = true;

        }

        private void ScrollViewer_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            // Suppress the boundary feedback to prevent window movement
            e.Handled = true;
        }

        private void OpenAnnouncementDetail(AnnouncementItem announcement)
        {
            // Dim the announcement cards
            DimAnnouncementCards(true);

            Overlay.Visibility = Visibility.Visible;

            var announcementDetailWindow = new AnnouncementDetail(announcement);
            announcementDetailWindow.Closed += (sender, args) =>
            {
                Overlay.Visibility = Visibility.Collapsed;

                // Restore the announcement cards' opacity
                DimAnnouncementCards(false);
            };

            announcementDetailWindow.ShowDialog();

        }
        // Method to dim or restore the opacity of announcement cards
        private void DimAnnouncementCards(bool dim)
        {
            double opacityValue = dim ? 0.3 : 1.0; // Dim to 30% opacity or restore to 100%

            foreach (var child in AnnouncementWrapPanel.Children)
            {
                if (child is Border card)
                {
                    card.Opacity = opacityValue; // Set the opacity for each announcement card
                }
            }
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
                    Text = item.Object.Text,
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


        // Loads announcements from Firebase and updates the UI
        /*private async Task LoadAnnouncements()
        {
            var announcements = await GetAnnouncementsFromFirebase();
            PopulateAnnouncements(announcements);
        }
        */

        // Populates the UI with announcement cards
        private void PopulateAnnouncements(List<AnnouncementItem> announcements)
        {
            AnnouncementWrapPanel.Children.Clear();

            if (announcements.Count == 0)
            {
                DisplayNoAnnouncementsMessage();
                return;
            }
            // Determine current time for comparison
            DateTime now = DateTime.Now;

            // Mark new announcements and sort all announcements by Timestamp in descending order
            var sortedAnnouncements = announcements
                .Select(announcement =>
                {
                    announcement.IsNew = (now - announcement.Timestamp).TotalHours < 24; // Mark as new if posted within the last 3 hours
                    return announcement;
                })
                .OrderByDescending(announcement => announcement.Timestamp) // Sort from newest to oldest
                .ToList();

            // Populate the UI with the sorted announcements
            foreach (var announcement in sortedAnnouncements)
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
                Width = 400,
                Height = 300,
                Margin = new Thickness(15),
                Background = Background = announcement.IsNew ? Brushes.LightBlue : Brushes.White, // Change color if new
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

            // Create a Grid for the title and dot indicator
            var titleGrid = new Grid();

            // Add title
            var titleBlock = CreateTitleTextBlock(announcement.Title);
            titleGrid.Children.Add(titleBlock);

            // Add dot indicator for new announcements
            Ellipse dotIndicator = null;
            // Create a dot indicator if the announcement is new
            if (announcement.IsNew)
            {
                dotIndicator = new Ellipse
                {
                    Width = 10,
                    Height = 10,
                    Fill = Brushes.Red, // Red dot for new announcements
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 20, 20, 0)
                };
                titleGrid.Children.Add(dotIndicator); 
            }
            // Add components to the stack panel
            cardStackPanel.Children.Add(titleGrid);
            cardStackPanel.Children.Add(CreateAnnouncementImage(announcement.ImageUrl));

            cardBorder.Child = cardStackPanel;

            // Handle click event
            cardBorder.MouseDown += (s, e) =>
            {
                // Reset the background to default before opening detail
                cardBorder.Background = Brushes.White;
                announcement.IsNew = false; // Mark as no longer new
                                            // Remove the dot indicator if it exists
                if (dotIndicator != null)
                {
                    titleGrid.Children.Remove(dotIndicator);
                }

                OpenAnnouncementDetail(announcement);
            };

            // Hover effects with proper state management
            cardBorder.MouseEnter += (s, e) =>
            {
                if (announcement.IsNew)
                {
                    cardBorder.Background = new SolidColorBrush(Color.FromRgb(173, 216, 230)); // Slightly darker light blue
                }
                else
                {
                    cardBorder.Background = Brushes.LightGray;
                }

                cardBorder.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Black,
                    BlurRadius = 10,
                    ShadowDepth = 4
                };
            };

            cardBorder.MouseLeave += (s, e) =>
            {
                cardBorder.Background = announcement.IsNew ? Brushes.LightBlue : Brushes.White;
                cardBorder.Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    BlurRadius = 8,
                    ShadowDepth = 2
                };
            };

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
                TextAlignment = TextAlignment.Center,
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
        /*private Button CreateDetailsButton(AnnouncementItem announcement)
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
        */

        // Opens the announcement detail window and shows the overlay
        /*private void OpenAnnouncementDetail(AnnouncementItem announcement)
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
        */
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
            public required string ImageUrl { get; set; } // Mark as required
            public required string Text { get; set; }      // Mark as required
            public required string Title { get; set; }     // Mark as required
            public required DateTime Timestamp { get; set; } // Mark as required
            public bool IsNew { get; set; } = false;
        }
    }
}