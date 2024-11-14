using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Windows;
using Firebase.Storage;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin;
using System.IO;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using SystemDataTable = System.Data.DataTable;
using ExcelDataTable = Microsoft.Office.Interop.Excel.DataTable;
using System;
using System.Runtime.InteropServices;

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
            Loading loadingWindow = new Loading();
            loadingWindow.Topmost = true;
            loadingWindow.LoadingMessage = "Loading 2nd Floor Room Schedule, please wait...";
            loadingWindow.Show();

            try
            {
                // Load Excel content asynchronously
                await Task.Run(() => LoadExcelContent(sheetNumber));

                // Update roomLabel on the UI thread
                Dispatcher.Invoke(() => { roomLabel.Content = roomNumber; });
            }
            catch (Exception ex)
            {
                // Handle errors gracefully
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"Error loading Excel file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
            finally
            {
                // Close loading window
                loadingWindow.Close();
                // Re-enable the main window
                this.IsEnabled = true;
            }
        }

        private async Task LoadExcelContent(int sheetNumber)
        {
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"tempfile_{Guid.NewGuid()}.xls");  // Default to .xls

            // Ensure the temp file is deleted if it already exists
            if (File.Exists(tempFilePath))
            {
                try
                {
                    File.Delete(tempFilePath);
                }
                catch (IOException ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Error deleting temporary file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            }

            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Workbook workbook = null;
            Worksheet worksheet = null;
            Microsoft.Office.Interop.Excel.Range range = null;

            try
            {
                // Initialize Google Cloud Storage client
                var storageClient = StorageClient.Create();
                var bucketName = "sti-onn-d0161.appspot.com";

                // List files in the 'schedules/Floor2/' directory
                var files = storageClient.ListObjects(bucketName, "schedules/Floor3/").ToList();

                if (files == null || !files.Any())
                {
                    throw new Exception("No Excel file found in Firebase Storage for Floor 2 schedules.");
                }

                // Get the most recent file based on the updated time
                var latestFile = files
                    .OrderByDescending(file => file.Updated) // Sort by updated date
                    .FirstOrDefault(); // Select the latest file

                if (latestFile == null)
                {
                    throw new Exception("No file found after checking Firebase Storage.");
                }

                // Check the content type (MIME type) of the file
                var contentType = latestFile.ContentType;

                // Check if the file is an Excel file (either xls or xlsx)
                if (contentType == "application/vnd.ms-excel")
                {
                    // If it's an older Excel file, we handle it as .xls
                    tempFilePath = Path.ChangeExtension(tempFilePath, ".xls");
                }
                else if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    // If it's a modern Excel file, we handle it as .xlsx
                    tempFilePath = Path.ChangeExtension(tempFilePath, ".xlsx");
                }
                else
                {
                    throw new Exception("The file is not a valid Excel file. Content Type: " + contentType);
                }

                // Download the file into a memory stream
                var memoryStream = new MemoryStream();
                await storageClient.DownloadObjectAsync(bucketName, latestFile.Name, memoryStream);

                // Set the position of the stream to the beginning
                memoryStream.Position = 0;

                // Write the memory stream to the temporary file
                File.WriteAllBytes(tempFilePath, memoryStream.ToArray());

                // Check if the file exists and has the proper extension
                if (!File.Exists(tempFilePath) || !tempFilePath.EndsWith(".xls") && !tempFilePath.EndsWith(".xlsx"))
                {
                    throw new Exception("Temporary file is not a valid Excel file or does not have a proper extension.");
                }

                // Open Excel and read data
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                workbook = excelApp.Workbooks.Open(tempFilePath);

                // Check if the sheet number is valid
                if (sheetNumber < 1 || sheetNumber > workbook.Sheets.Count)
                {
                    throw new Exception($"Sheet number {sheetNumber} is out of range. The workbook has {workbook.Sheets.Count} sheets.");
                }

                // Get the specified sheet by sheet number
                worksheet = (Worksheet)workbook.Sheets[sheetNumber]; // Use the specified sheet number
                range = worksheet.UsedRange;

                // Create a DataTable to hold the Excel data
                SystemDataTable dataTable = new SystemDataTable();

                // Add columns to DataTable
                for (int col = 1; col <= 7; col++)
                {
                    dataTable.Columns.Add($"Column{col}");
                }

                // Loop through the cells and populate the DataTable
                for (int row = 1; row <= 25; row++)
                {
                    DataRow dr = dataTable.NewRow();
                    for (int col = 1; col <= 7; col++)
                    {
                        dr[col - 1] = (range.Cells[row, col] as Microsoft.Office.Interop.Excel.Range)?.Value2?.ToString();
                    }
                    dataTable.Rows.Add(dr);
                }

                // Bind DataTable to DataGrid on the UI thread
                Dispatcher.Invoke(() =>
                {
                    excelDataGrid.ItemsSource = dataTable.DefaultView;
                });

                // Close Excel
                workbook.Close(false);
                excelApp.Quit();

                // Release COM objects
                ReleaseExcelObjects(excelApp, workbook, worksheet);

                // Wait a bit and then try deleting the temporary file
                await Task.Delay(500);
                await DeleteTempFile(tempFilePath);
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

        private async Task DeleteTempFile(string filePath)
        {
            const int maxAttempts = 5;
            int attempt = 0;
            bool fileDeleted = false;

            while (!fileDeleted && attempt < maxAttempts)
            {
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        fileDeleted = true;
                    }
                }
                catch (IOException)
                {
                    await Task.Delay(500); // Wait before retrying
                }
                attempt++;
            }

            if (!fileDeleted)
            {
                Dispatcher.Invoke(() => MessageBox.Show("Unable to delete temp file after multiple attempts.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning));
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