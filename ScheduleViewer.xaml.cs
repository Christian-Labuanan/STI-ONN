using OfficeOpenXml;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for ScheduleViewer.xaml
    /// </summary>
    public partial class ScheduleViewer : Window
    {
        private const double SCROLL_SPEED = 50.0; // Adjust this value to control scroll speed
        public ScheduleViewer()
        {
            InitializeComponent();   
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (VisualTreeHelper.GetParent(ScheduleDataGrid) is ScrollViewer scrollViewer)
            {
                // Adjust vertical offset
                double newOffset = scrollViewer.VerticalOffset - (e.Delta / Math.Abs(e.Delta) * SCROLL_SPEED);
                newOffset = Math.Max(0, Math.Min(newOffset, scrollViewer.ScrollableHeight));
                scrollViewer.ScrollToVerticalOffset(newOffset);
                e.Handled = true;
            }
        }
        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta * 0.2);
                e.Handled = true;
            }
        }

        private void DataGrid_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (VisualTreeHelper.GetParent(ScheduleDataGrid) is ScrollViewer scrollViewer)
            {
                // Adjust vertical offset for touch scrolling
                double newOffset = scrollViewer.VerticalOffset - e.DeltaManipulation.Translation.Y;
                newOffset = Math.Max(0, Math.Min(newOffset, scrollViewer.ScrollableHeight));
                scrollViewer.ScrollToVerticalOffset(newOffset);
                e.Handled = true;
            }
        }

        private static T GetTemplatedChild<T>(DependencyObject parent) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T typedChild)
                    return typedChild;

                var foundChild = GetTemplatedChild<T>(child);
                if (foundChild != null)
                    return foundChild;
            }
            return null;
        }
        public async Task LoadSchedule(string scheduleUrl, string instructorName, string instructorAvatarUrl)
        {

            try
            {
                // Set the instructor's name and image
                InstructorNameTextBlock.Text = instructorName;
                InstructorImage.Source = new BitmapImage(new Uri(instructorAvatarUrl));

                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

                using var httpClient = new HttpClient();
                var fileBytes = await httpClient.GetByteArrayAsync(scheduleUrl);

                using var package = new ExcelPackage(new MemoryStream(fileBytes));
                var worksheet = package.Workbook.Worksheets[0];

                // Add padding and set font for the first row and column
                worksheet.Cells.Style.Font.Size = 11; // Set a standard font size
                worksheet.Cells.Style.Font.Name = "Arial"; // Set a standard font

                // Apply padding to all cells
                worksheet.Cells.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                worksheet.Cells.Style.WrapText = true; // Wrap text for better readability

                // Apply bold formatting to the first row (header)
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    worksheet.Cells[1, col].Style.Font.Bold = true;
                }

                // Apply bold formatting to the first column
                for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
                {
                    worksheet.Cells[row, 1].Style.Font.Bold = true;
                }


                DataTable table = new DataTable();
                foreach (var headerCell in worksheet.Cells[1, 1, 1, worksheet.Dimension.End.Column])
                {
                    table.Columns.Add(headerCell.Text);
                }

                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    DataRow dataRow = table.NewRow();
                    for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                    {
                        dataRow[col - 1] = worksheet.Cells[row, col].Text;
                    }
                    table.Rows.Add(dataRow);
                }

                if (table.Rows.Count == 0)
                {
                    MessageBox.Show("The schedule is empty.");
                }
                else
                {
                    ScheduleDataGrid.ItemsSource = table.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load schedule: " + ex.Message);
            }

        }
        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle between fullscreen and windowed mode
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}