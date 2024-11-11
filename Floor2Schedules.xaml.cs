using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Windows;
using Firebase.Storage;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin;
using System.IO;
using System.Threading.Tasks;
using Firebase.Storage.Client;
using Google.Cloud.Storage.V1;
using SystemDataTable = System.Data.DataTable;
using ExcelDataTable = Microsoft.Office.Interop.Excel.DataTable;
using System;
using Firebase.Storage.Options;
using System.Runtime.InteropServices;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for Floor2Schedules.xaml
    /// </summary>
    public partial class Floor2Schedules : System.Windows.Window
    {
        private int sheetNumber;
        private string roomNumber;

        public Floor2Schedules(int sheetNumber, string roomNumber)
        {
            InitializeComponent();
            this.sheetNumber = sheetNumber;
            this.roomNumber = roomNumber;
            DisplayExcelContent();
            // Call the centralized Firebase initialization
            FirebaseManager.InitializeFirebase();
        }
        
        private async void DisplayExcelContent()
        {
            // Disable the main window to prevent user interaction
            this.IsEnabled = false;


            try
            {
                // Load Excel content asynchronously
                await Task.Run(() => LoadExcelContent());

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

                // Re-enable the main window
                this.IsEnabled = true;
            }
        }

        private async Task LoadExcelContent()
        {
            // Unique temp file to avoid conflicts
            string tempFilePath = Path.Combine(Path.GetTempPath(), $"tempfile_{Guid.NewGuid()}.xlsx");
            try
            {
                
                // Initialize Google Cloud Storage client
                var storageClient = StorageClient.Create();

                // Initialize the FirebaseStorage instance with your bucket name
                var bucketName = "sti-onn-d0161.appspot.com";

                // List all objects in the 'schedules/Floor2/' directory
                var files = storageClient.ListObjects(bucketName, "schedules/Floor2/");

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

                // Write the memory stream to the temporary file
                File.WriteAllBytes(tempFilePath, memoryStream.ToArray());

                // Load Excel data from the stream
                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                Workbook workbook = excelApp.Workbooks.Open(tempFilePath); // You can save the stream temporarily to disk and open it
                Worksheet worksheet = (Worksheet)workbook.Sheets[sheetNumber];
                Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange;

                // Create a DataTable to hold the Excel data
                System.Data.DataTable dataTable = new System.Data.DataTable();

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
                await Task.Delay(500);  // Wait for 1 second
                                        // Try deleting the temp file
                await DeleteTempFile(tempFilePath);


                if (File.Exists(tempFilePath))
                {
                    try
                    {
                        // Try opening the file exclusively to check if it is still in use
                        using (FileStream fs = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            // If it is not in use, delete it
                            File.Delete(tempFilePath);
                        }
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window
            this.Close();
        }
    }
}
