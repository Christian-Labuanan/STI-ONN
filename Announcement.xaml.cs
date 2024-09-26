using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Firebase.Database; // Make sure to include the Firebase.Database library
using Firebase.Database.Query;
using System.Windows.Controls;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for Announcement.xaml
    /// </summary>
    public partial class Announcement : Window
    {
        private readonly DispatcherTimer _refreshTimer;
        private bool _announcementFetchedNotificationShown; // Flag to control notification display

        public Announcement()
        {
            InitializeComponent();
            _announcementFetchedNotificationShown = false; // Initialize the flag

            // Initialize the DispatcherTimer to refresh every minute (adjust the interval as needed)
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(0.5) // Adjust the interval as needed
            };
            _refreshTimer.Tick += async (sender, args) => await LoadAnnouncements();
            _refreshTimer.Start();

            // Load the initial announcements
            LoadAnnouncements();
        }

        private async Task LoadAnnouncements()
        {
            var announcements = await GetAnnouncementsFromFirebase();
            AnnouncementList.ItemsSource = announcements; // Bind the ListView to the retrieved announcements

            // Show notification if announcements are fetched for the first time
            if (announcements.Count > 0 && !_announcementFetchedNotificationShown)
            {
                MessageBox.Show($"Fetched {announcements.Count} announcements.");
                _announcementFetchedNotificationShown = true; // Set the flag to true
            }
            else if (announcements.Count == 0)
            {
                // Reset the flag if no announcements are found
                _announcementFetchedNotificationShown = false;
            }
        }

        private async Task<List<AnnouncementItem>> GetAnnouncementsFromFirebase()
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://sti-onn-d0161-default-rtdb.asia-southeast1.firebasedatabase.app/");
                var announcementList = await firebaseClient
                    .Child("posts")
                    .OnceAsync<AnnouncementItem>();

                if (announcementList == null || !announcementList.Any())
                {
                    return new List<AnnouncementItem>(); // Return an empty list
                }

                return announcementList.Select(item => new AnnouncementItem
                {
                    ImageUrl = item.Object.ImageUrl,     // Set ImageUrl
                    Text = item.Object.Text,               // Set Text
                    Timestamp = item.Object.Timestamp,     // Set Timestamp
                    Title = item.Object.Title               // Set Title
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching announcements: " + ex.Message);
                return new List<AnnouncementItem>();
            }
        }

        private void touch_Click_1(object sender, RoutedEventArgs e)
        {
            Home home1 = new Home();
            home1.Show();
            this.Hide();
        }

        private void AnnouncementList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AnnouncementList.SelectedItem is AnnouncementItem selectedAnnouncement)
            {
                // Show full details, e.g., in a new window or a message box
                MessageBox.Show($"Title: {selectedAnnouncement.Title}\nDetails: {selectedAnnouncement.Text}");
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image clickedImage)
            {
                var announcementItem = clickedImage.DataContext as AnnouncementItem;

                // Show details in the DetailsPanel
                DetailTitle.Text = announcementItem.Title;
                DetailImage.Source = new BitmapImage(new Uri(announcementItem.ImageUrl));
                DetailText.Text = announcementItem.Text;

                // Show the details panel
                DetailsPanel.Visibility = Visibility.Visible;
            }
        }
    }

    public class AnnouncementItem
    {
        public string ImageUrl { get; set; }   // Corresponds to imageUrl
        public string Text { get; set; }        // Corresponds to text
        public string Timestamp { get; set; }   // Corresponds to timestamp
        public string Title { get; set; }       // Corresponds to title
    }
}