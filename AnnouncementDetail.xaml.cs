using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.Web;
using HtmlAgilityPack;
using System.Windows.Media;

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
            this.Height = 650; // Adjust as necessary

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
            string htmlContent = HttpUtility.HtmlDecode(announcement.Text); // Decode HTML entities
                                                                            // Convert Facebook emoji images to Unicode emoji
            htmlContent = ConvertEmojiImagesToUnicode(htmlContent);

            // Load HTML content into HtmlAgilityPack's HtmlDocument
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // Create FlowDocument for the RichTextBox
            FlowDocument flowDoc = new FlowDocument();

            // Handle different HTML tags
            ProcessHtmlTags(doc, flowDoc);

            // Set the FlowDocument to the RichTextBox
            DetailTextBox.Document = flowDoc;
        }

        private void ProcessHtmlTags(HtmlDocument doc, FlowDocument flowDoc)
        {
            // Handle inline styles (bold, italic, underline)
            HandleTextStyles(doc, flowDoc);

            // Handle headers (h1, h2)
            HandleHeaders(doc, flowDoc);

            // Handle paragraphs
            HandleParagraphs(doc, flowDoc);

            // Handle line breaks
            HandleLineBreaks(doc, flowDoc);

            // Handle lists (unordered and ordered)
            HandleLists(doc, flowDoc);

            // Handle images
            HandleImages(doc, flowDoc);

            // Handle links
            HandleLinks(doc, flowDoc);
        }
        private string ConvertEmojiImagesToUnicode(string htmlContent)
        {
            HtmlDocument tempDoc = new HtmlDocument();
            tempDoc.LoadHtml(htmlContent);

            var emojiImages = tempDoc.DocumentNode.SelectNodes("//img[contains(@src, 'emoji.php')]");
            if (emojiImages != null)
            {
                foreach (var img in emojiImages)
                {
                    // Check if the alt attribute contains the emoji
                    string altText = img.GetAttributeValue("alt", "");
                    if (!string.IsNullOrEmpty(altText))
                    {
                        // Replace the entire img tag with the alt text (which contains the Unicode emoji)
                        var newNode = tempDoc.CreateTextNode(altText);
                        img.ParentNode.ReplaceChild(newNode, img);
                    }
                }
                return tempDoc.DocumentNode.InnerHtml;
            }
            return htmlContent;
        }

        private void HandleTextStyles(HtmlDocument doc, FlowDocument flowDoc)
        {
            var tagStyles = new Dictionary<string, Action<HtmlNode>>
            {
                { "//b | //strong", node => AddStyledParagraph(flowDoc, node.InnerText, FontWeights.Bold) },
                { "//i", node => AddStyledParagraph(flowDoc, node.InnerText, null, FontStyles.Italic) },
                { "//u", node => AddStyledParagraph(flowDoc, node.InnerText, null, null, TextDecorations.Underline) }
            };

            foreach (var (xpath, action) in tagStyles)
            {
                foreach (HtmlNode tag in doc.DocumentNode.SelectNodes(xpath) ?? Enumerable.Empty<HtmlNode>())
                {
                    action(tag);
                }
            }
        }

        private void HandleHeaders(HtmlDocument doc, FlowDocument flowDoc)
        {
            foreach (HtmlNode hTag in doc.DocumentNode.SelectNodes("//h1 | //h2") ?? Enumerable.Empty<HtmlNode>())
            {
                double fontSize = hTag.Name == "h1" ? 24 : 20;
                AddStyledParagraph(flowDoc, hTag.InnerText, FontWeights.Bold, null, null, fontSize);
            }
        }

        private void HandleParagraphs(HtmlDocument doc, FlowDocument flowDoc)
        {
            foreach (HtmlNode pTag in doc.DocumentNode.SelectNodes("//p") ?? Enumerable.Empty<HtmlNode>())
            {
                flowDoc.Blocks.Add(new Paragraph(new Run(pTag.InnerText)));
            }
        }

        private void HandleLineBreaks(HtmlDocument doc, FlowDocument flowDoc)
        {
            foreach (HtmlNode brTag in doc.DocumentNode.SelectNodes("//br") ?? Enumerable.Empty<HtmlNode>())
            {
                flowDoc.Blocks.Add(new Paragraph(new LineBreak()));
            }
        }

        private void HandleLists(HtmlDocument doc, FlowDocument flowDoc)
        {
            var listTags = new Dictionary<string, TextMarkerStyle>
            {
                { "//ul", TextMarkerStyle.Disc },
                { "//ol", TextMarkerStyle.Decimal }
            };

            foreach (var (xpath, markerStyle) in listTags)
            {
                foreach (HtmlNode listTag in doc.DocumentNode.SelectNodes(xpath) ?? Enumerable.Empty<HtmlNode>())
                {
                    List list = new List { MarkerStyle = markerStyle };
                    foreach (HtmlNode liTag in listTag.SelectNodes("li") ?? Enumerable.Empty<HtmlNode>())
                    {
                        list.ListItems.Add(new ListItem(new Paragraph(new Run(liTag.InnerText))));
                    }
                    flowDoc.Blocks.Add(list);
                }
            }
        }

        private void HandleImages(HtmlDocument doc, FlowDocument flowDoc)
        {
            foreach (HtmlNode imgTag in doc.DocumentNode.SelectNodes("//img") ?? Enumerable.Empty<HtmlNode>())
            {
                string imgSrc = imgTag.GetAttributeValue("src", string.Empty);

                // Create a border for the image
                Border imageBorder = new Border
                {
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD6E6F2")),
                    BorderThickness = new Thickness(2),
                    CornerRadius = new CornerRadius(5),
                    Padding = new Thickness(5),
                    Margin = new Thickness(0, 10, 0, 10),
                    Background = Brushes.White
                };

                Image img = new Image
                {
                    Stretch = Stretch.Uniform,
                    MaxHeight = 400,
                    MaxWidth = 700
                };

                if (imgSrc.StartsWith("data:image"))
                {
                    LoadBase64Image(img, imgSrc);
                }
                else if (Uri.IsWellFormedUriString(imgSrc, UriKind.Absolute))
                {
                    LoadExternalImage(img, imgSrc);
                }

                imageBorder.Child = img;
                flowDoc.Blocks.Add(new BlockUIContainer(imageBorder));
            }
        }

        private void AddBase64Image(FlowDocument flowDoc, string imgSrc)
        {
            try
            {
                string base64Data = imgSrc.Split(',')[1];
                byte[] binaryData = Convert.FromBase64String(base64Data);
                BitmapImage bitmap = new BitmapImage();
                using (var stream = new MemoryStream(binaryData))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }

                // Add the Image to the FlowDocument
                Image img = new Image { Source = bitmap, Width = 1000, Height = 1000, Margin = new Thickness(5) };
                flowDoc.Blocks.Add(new Paragraph(new InlineUIContainer(img)));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error decoding base64 image: " + ex.Message);
            }
        }
        private void LoadBase64Image(Image img, string base64String)
        {
            try
            {
                string base64Data = base64String.Split(',')[1];
                byte[] binaryData = Convert.FromBase64String(base64Data);

                BitmapImage bitmap = new BitmapImage();
                using (var stream = new MemoryStream(binaryData))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.DecodePixelWidth = 1000; // Optimize loading performance
                    bitmap.EndInit();
                }

                img.Source = bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading base64 image: {ex.Message}");
            }
        }
        private void LoadExternalImage(Image img, string imageUrl)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(imageUrl);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.DecodePixelWidth = 1000; // Optimize loading performance
                bitmap.EndInit();

                img.Source = bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading external image: {ex.Message}");
            }
        }

        private void AddExternalImage(FlowDocument flowDoc, string imgSrc)
        {
            Image img = new Image
            {
                Source = new BitmapImage(new Uri(imgSrc)),
                Width = 1000,
                Height = 1000,
                Margin = new Thickness(5)
            };
            flowDoc.Blocks.Add(new Paragraph(new InlineUIContainer(img)));
        }

        private void HandleLinks(HtmlDocument doc, FlowDocument flowDoc)
        {
            foreach (HtmlNode linkTag in doc.DocumentNode.SelectNodes("//a") ?? Enumerable.Empty<HtmlNode>())
            {
                var hyperlink = new Hyperlink(new Run(linkTag.InnerText))
                {
                    NavigateUri = new Uri(linkTag.GetAttributeValue("href", string.Empty)),
                    Foreground = Brushes.Blue,
                    TextDecorations = TextDecorations.Underline
                };
                flowDoc.Blocks.Add(new Paragraph(hyperlink));
            }
        }

        private void AddStyledParagraph(FlowDocument flowDoc, string text, FontWeight? fontWeight = null, FontStyle? fontStyle = null, TextDecorationCollection textDecorations = null, double fontSize = 12)
        {
            Run run = new Run(text)
            {
                FontWeight = fontWeight ?? FontWeights.Normal,
                FontStyle = fontStyle ?? FontStyles.Normal,
                TextDecorations = textDecorations,
                FontSize = fontSize
            };
            Paragraph paragraph = new Paragraph(run)
            {
                Margin = new Thickness(0, 0, 0, 10) // Add spacing between paragraphs
            };
            flowDoc.Blocks.Add(new Paragraph(run));
        }

        private void LoadImage(string imageUrl)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(imageUrl);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.DecodePixelWidth = 1000; // Optimize loading performance
                bitmapImage.EndInit();
                DetailImage.Source = bitmapImage;
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