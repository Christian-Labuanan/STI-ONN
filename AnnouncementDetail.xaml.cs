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
            DetailTitle.Text = announcement.Title;


            // Check if the announcement image URL is valid; if not, use the default image
            if (!string.IsNullOrEmpty(announcement.ImageUrl))
            {
                try
                {
                    DetailImage.Source = new BitmapImage(new Uri(announcement.ImageUrl));
                }
                catch (UriFormatException)
                {
                    // Log the error or handle it as needed
                    DetailImage.Source = new BitmapImage(new Uri("pack://application:,,,/assets/Icons/default-image.png"));
                }
            }
            else
            {
                // Use the default image if the announcement image URL is invalid or empty
                DetailImage.Source = new BitmapImage(new Uri("pack://application:,,,/assets/Icons/default-image.png"));
            }

            DetailText.Text = RemoveHtmlTags(announcement.Text);
            // Center the window on the screen
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Set a desired width and height
            this.Width = 600; // Adjust as necessary
            this.Height = 450; // Adjust as necessary

            // Alternatively, position manually relative to the main window
            var mainWindow = Application.Current.MainWindow;
            this.Top = mainWindow.Top + (mainWindow.Height / 2) - (this.Height / 2);
            this.Left = mainWindow.Left + (mainWindow.Width / 2) - (this.Width / 2);
        }
        // Override to prevent closing when clicking outside
        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            // Prevent the window from closing
            this.Activate();
        }
        // Override OnMouseLeftButtonDown to prevent closing/hiding the window when clicking outside
        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            // Prevent any default behavior for clicking outside the window
            e.Handled = true;
        }

        // Method to remove HTML tags from the text
        private string RemoveHtmlTags(string text)
        {
            return System.Text.RegularExpressions.Regex.Replace(text, "<.*?>", string.Empty);
        }
    }
}