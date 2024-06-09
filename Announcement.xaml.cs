using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for Announcement.xaml
    /// </summary>
    public partial class Announcement : Window
    {
        private readonly StorageClient _storageClient;
        private readonly DispatcherTimer _refreshTimer;
        private string _latestImagePath;

        public Announcement()
        {
            InitializeComponent();
            _storageClient = StorageClient.Create(GetGoogleCredential());

            // Initialize the DispatcherTimer to refresh every minute (adjust the interval as needed)
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(.5) // Adjust the interval as needed
            };
            _refreshTimer.Tick += async (sender, args) => await LoadLatestImage();
            _refreshTimer.Start();

            // Load the initial image
            LoadLatestImage();
        }

        private void touch_Click_1(object sender, RoutedEventArgs e)
        {
            Home home1 = new Home();
            home1.Show();
            this.Hide();
        }

        private GoogleCredential GetGoogleCredential()
        {
            return GoogleCredential.FromFile(@"D:\FILES\COLLEGE\Thesis Code\STI ONN\Firebase\sti-onn-d0161-firebase-adminsdk-dgl8a-44eb665f77.json");
        }

        private async Task LoadLatestImage()
        {
            string bucketName = "sti-onn-d0161.appspot.com"; // Bucket name without 'gs://'

            try
            {
                var objects = _storageClient.ListObjects(bucketName, "Files/")
                    .OrderByDescending(o => o.Updated).FirstOrDefault(); // Sort objects by the last modified date

                if (objects == null)
                {
                    MessageBox.Show("No images found in the bucket.");
                    return;
                }

                string latestImagePath = objects.Name;

                // Only reload if the image has changed
                if (_latestImagePath == latestImagePath)
                {
                    return;
                }

                _latestImagePath = latestImagePath;

                // Generate a signed URL for the image
                var urlSigner = UrlSigner.FromCredential(GetGoogleCredential());
                string url = urlSigner.Sign(bucketName, latestImagePath, TimeSpan.FromHours(1), HttpMethod.Get);

                // Download and display the image
                using (var httpClient = new HttpClient())
                {
                    var imageBytes = await httpClient.GetByteArrayAsync(url);
                    var bitmap = new BitmapImage();
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    }

                    MyImage.Source = bitmap; // Display the image in the MyImage element
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading image: " + ex.Message);
            }
        }
    }
}
