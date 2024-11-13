using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Markup;
using System.IO;
using System.Xml;

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
            string htmlContent = announcement.Text;
            // Replace HTML entities like &nbsp; with a compatible XAML character
            htmlContent = htmlContent.Replace("&nbsp;", " ");


            // Remove <a> tags, including attributes, by matching opening and closing tags
            htmlContent = System.Text.RegularExpressions.Regex.Replace(htmlContent, @"<a[^>]*>|</a>", string.Empty);


            // Convert some basic HTML tags to XAML-compatible tags for display
            htmlContent = htmlContent.Replace("<b>", "<Bold>").Replace("</b>", "</Bold>");
            htmlContent = htmlContent.Replace("<i>", "<Italic>").Replace("</i>", "</Italic>");
            htmlContent = htmlContent.Replace("<u>", "<Underline>").Replace("</u>", "</Underline>");
            htmlContent = htmlContent.Replace("<strong>", "<Bold>").Replace("</strong>", "</Bold>");
            htmlContent = htmlContent.Replace("<p>", "<Paragraph>").Replace("</p>", "</Paragraph>");
            htmlContent = htmlContent.Replace("<br>", "<LineBreak/>");

            // Handle <img> tags
            htmlContent = System.Text.RegularExpressions.Regex.Replace(htmlContent, @"<img[^>]*src=""(.*?)""[^>]*>", match =>
            {
                string imgSrc = match.Groups[1].Value;

                // Check if the image source is base64 encoded
                if (imgSrc.StartsWith("data:image"))
                {
                    // It's a base64 image, so use it directly in the Image Source
                    return $"<InlineUIContainer><Image Source=\"{imgSrc}\" Width=\"100\" Height=\"100\" /></InlineUIContainer>";
                }

                // Check if the image source URL indicates an emoji (based on common patterns)
                bool isEmoji = imgSrc.Contains("emoji.php") || imgSrc.Contains("twemoji") || imgSrc.Contains("emojipedia");

                // Determine image extension (check for known image formats)
                string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".svg" };
                bool isImage = imageExtensions.Any(ext => imgSrc.EndsWith(ext, StringComparison.OrdinalIgnoreCase));

                // Set image XAML based on type (emoji vs regular image)
                string imageXaml;
                if (isEmoji)
                {
                    // For emojis, use smaller size
                    imageXaml = $"<InlineUIContainer><Image Source=\"{imgSrc}\" Width=\"16\" Height=\"16\" /></InlineUIContainer>";
                }
                else if (isImage)
                {
                    // For regular images, use larger size or default size
                    imageXaml = $"<InlineUIContainer><Image Source=\"{imgSrc}\" Width=\"100\" Height=\"100\" /></InlineUIContainer>";
                }
                else
                {
                    // If it's not recognized as an image, return an empty string or a placeholder
                    imageXaml = "";
                }

                return imageXaml;
            });

            // Replace <ul> with <List> and <li> with <ListItem>
            htmlContent = htmlContent.Replace("<ul>", "<List>").Replace("</ul>", "</List>");
            htmlContent = System.Text.RegularExpressions.Regex.Replace(htmlContent, @"<li>(.*?)</li>", "<ListItem><Paragraph>$1</Paragraph></ListItem>");

            // Wrap the text in a FlowDocument tag for use in the RichTextBox
            string xamlContent = $"<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>{htmlContent}</FlowDocument>";

            try
            {
                // Parse the XAML string into a FlowDocument
                FlowDocument flowDoc = (FlowDocument)XamlReader.Parse(xamlContent);
                DetailTextBox.Document = flowDoc;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error parsing announcement content: " + ex.Message);
                // You may set a default document or show plain text in case of an error
                DetailTextBox.Document = new FlowDocument(new Paragraph(new Run("Unable to load content.")));
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

    }
}