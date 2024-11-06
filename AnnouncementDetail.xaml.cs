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
        
        public AnnouncementDetail(AnnouncementItem announcement)
        {
            InitializeComponent();
            
            // Set announcement details
            DetailTitle.Text = announcement.Title;
            LoadImage(announcement.ImageUrl);
            DetailText.Text = RemoveHtmlTags(announcement.Text);

            // Load images and text from the announcement
            LoadAnnouncementContent(announcement);

            // Center window and set size
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Width = 1150; // Adjust as necessary
            this.Height = 950; // Adjust as necessary

            // Inside the constructor or initialization method
            ScrollViewer scrollViewer = new ScrollViewer
            {
                PanningMode = PanningMode.Both, // Enable touch panning
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
            };
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
                try
                {
                    // Attempt to create and load the image
                    var image = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri(match.Value)),
                        Width = 1150,
                        Height = 950,
                        Margin = new Thickness(5)
                    };
                    ImageContainer.Children.Add(image);
                }
                catch (Exception)
                {
                    // If an error occurs, you could add a placeholder image or skip adding the image
                    var placeholder = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/assets/Icons/default-image.png")),
                        Width = 300,
                        Height = 200,
                        Margin = new Thickness(5)
                    };
                    ImageContainer.Children.Add(placeholder);
                }
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