using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Windows;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for room301.xaml
    /// </summary>
    public partial class room301 : System.Windows.Window
    {
        private int sheetNumber;
        private string roomNumber;

        public room301(int sheetNumber, string roomNumber)
        {
            InitializeComponent();
            this.sheetNumber = sheetNumber;
            this.roomNumber = roomNumber;
            DisplayExcelContent();
        }

        private async void DisplayExcelContent()
        {
            // Disable the main window to prevent user interaction
            this.IsEnabled = false;
            // Show loading window
            Loading loadingWindow = new Loading();
            loadingWindow.Topmost = true;
            loadingWindow.LoadingMessage = "Loading 3rd Floor Room Schedule, please wait...";
            loadingWindow.Show();

            // Load Excel content asynchronously
            await Task.Run(() => LoadExcelContent());

            // Update roomLabel on the UI thread
            Dispatcher.Invoke(() =>
            {
                roomLabel.Content = roomNumber;
            });

            // Close loading window
            loadingWindow.Close();
            // Re-enable the main window (Floor2Schedules)
            this.IsEnabled = true;
        }

        private void LoadExcelContent()
        {
            // Specify the path to your Excel file
            string excelFilePath = @"C:\Users\Rodmar\OneDrive\Documents\GitHub\STI-ONN\assets\Schedules\stiSched.xlsx";

            // Create a new Excel Application
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

            // Open the Excel file
            Workbook workbook = excelApp.Workbooks.Open(excelFilePath);

            // Get the specified worksheet
            Worksheet worksheet = (Worksheet)workbook.Sheets[sheetNumber];

            // Get the used range of cells
            Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange;

            // Create a DataTable to hold the Excel data
            System.Data.DataTable dt = new System.Data.DataTable();

            // Add columns to DataTable
            for (int col = 1; col <= 7; col++)
            {
                dt.Columns.Add($"Column{col}");
            }

            // Loop through the cells and populate the DataTable
            for (int row = 1; row <= 25; row++)
            {
                DataRow dr = dt.NewRow();
                for (int col = 1; col <= 7; col++)
                {
                    dr[col - 1] = (range.Cells[row, col] as Microsoft.Office.Interop.Excel.Range)?.Value2?.ToString();
                }
                dt.Rows.Add(dr);
            }

            // Bind DataTable to DataGrid on the UI thread
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                excelDataGrid.ItemsSource = dt.DefaultView;
            });

            // Close Excel
            workbook.Close(false);
            excelApp.Quit();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window
            this.Close();
        }
    }
}