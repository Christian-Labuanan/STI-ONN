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
        // Static field to store FirebaseApp instance
        private static FirebaseApp firebaseApp;

        public Floor2Schedules(int sheetNumber, string roomNumber)
        {
            InitializeComponent();
            this.sheetNumber = sheetNumber;
            this.roomNumber = roomNumber;
            DisplayExcelContent();
            // Initialize FirebaseApp only once
            InitializeFirebase();
        }
        private void InitializeFirebase()
        {
            // Check if FirebaseApp is already initialized
            if (firebaseApp == null)
            {
                // Create a dynamic file path for ServiceAccountKey.json
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Firebase", "ServiceAccountKey.json");
                firebaseApp = FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(filePath),
                });
            }
        }
        private async void DisplayExcelContent()
        {
            // Disable the main window to prevent user interaction
            this.IsEnabled = false;

            // Show loading window
            Loading loadingWindow = new Loading
            {
                Topmost = true,
                LoadingMessage = "Loading 2nd Floor Room Schedule, please wait..."
            };
            loadingWindow.Show();

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

                // Close loading window
                loadingWindow.Close();

                // Re-enable the main window
                this.IsEnabled = true;
            }
        }

        private async Task LoadExcelContent()
        {
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

                // Create a temporary file to save the memory stream
                string tempFilePath = Path.Combine(Path.GetTempPath(), "tempfile.xlsx");
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
                // Ensure the file is no longer in use before proceeding
                await Task.Delay(1000);  // Small delay to ensure Excel processes have released the file

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
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window
            this.Close();
        }
    }
}
