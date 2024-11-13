using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Windows;
using System.IO;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using System.Runtime.InteropServices;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

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
            // Initialize Firebase once in the application
            FirebaseManager.InitializeFirebase();
        }

        private async void DisplayExcelContent()
        {
            // Disable the main window to prevent user interaction
            this.IsEnabled = false;
            // Show loading window
            Loading loadingWindow = new Loading
            {
                Topmost = true,
                LoadingMessage = "Loading Room 301 Schedule, please wait..."
            };
            loadingWindow.Show();

            try
            {
                // Await the LoadExcelContent task to ensure it completes before closing the loading window
                await LoadExcelContent();
            }
            finally
            {
                // Close loading window
                loadingWindow.Close();
            // Re-enable the main window (Floor2Schedules)
            this.IsEnabled = true;
            }
        }

        private async Task LoadExcelContent()
        {
            try
            {
                // Initialize Google Cloud Storage client
                var storageClient = StorageClient.Create();

                // Specify the bucket name
                var bucketName = "sti-onn-d0161.appspot.com";

                // List all objects in the 'schedules/Floor3/' directory
                var files = storageClient.ListObjects(bucketName, "schedules/Floor3/");

                // Find the file that matches the Excel file criteria (e.g., ending with .xlsx)
                string filePath = null;
                foreach (var file in files)
                {
                    if (file.Name.EndsWith(".xlsx"))
                    {
                        filePath = file.Name;
                        break; // Exit loop once the file is found
                    }
                }

                if (filePath == null)
                {
                    throw new Exception("Excel file not found in Firebase Storage.");
                }

                // Create a memory stream to download the file
                var memoryStream = new MemoryStream();

                // Download the file from Firebase Storage
                await storageClient.DownloadObjectAsync(bucketName, filePath, memoryStream);

                // Set the position of the stream to the beginning
                memoryStream.Position = 0;

                // Create a unique temporary file path for each instance
                string tempFilePath = Path.Combine(Path.GetTempPath(), $"tempfile_{Guid.NewGuid()}.xlsx");

                // Write the memory stream to the temporary file
                File.WriteAllBytes(tempFilePath, memoryStream.ToArray());

                // Create a new Excel Application
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();

                // Open the Excel file
                Workbook workbook = excelApp.Workbooks.Open(tempFilePath);

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

                // Release Excel COM objects
                ReleaseExcelObjects(excelApp, workbook, worksheet);

                // Ensure the file is no longer in use before proceeding
                await Task.Delay(2000);  // Small delay to ensure Excel processes have released the file

                // Optional: Delete the temporary file after use
                if (File.Exists(tempFilePath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch (IOException ex)
                    {
                        // Handle the case where the file is still in use
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Error deleting file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors gracefully
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error loading Excel file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }
        private void ReleaseExcelObjects(Microsoft.Office.Interop.Excel.Application excelApp, Workbook workbook, Worksheet worksheet)
        {
            // Properly release Excel COM objects
            if (worksheet != null) Marshal.ReleaseComObject(worksheet);
            if (workbook != null) Marshal.ReleaseComObject(workbook);
            if (excelApp != null) Marshal.ReleaseComObject(excelApp);

            // Force garbage collection
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window
            this.Close();
        }
    }
}