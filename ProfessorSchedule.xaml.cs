using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for ProfessorSchedule.xaml
    /// </summary>
    public partial class ProfessorSchedule : Window
    {

        public ObservableCollection<Instructor> Instructors { get; set; }

        public ProfessorSchedule()
        {
            InitializeComponent();
            LoadInstructors();

        }


        private async Task LoadInstructors()
        {
            var loadingWindow = new Loading { LoadingMessage = "Loading Instructor Schedule, please wait..." };
            loadingWindow.Show();
            // Disable the main window to prevent interaction
            this.IsEnabled = false;

            var instructors = await GetInstructorsFromFirebase();

            // Assign the retrieved instructors to the Instructors collection for filtering
            Instructors = new ObservableCollection<Instructor>(instructors);

            // Populate the department ComboBox with distinct and sorted departments
            var departments = Instructors
                .Select(i => i.Department)
                .Distinct()
                .OrderBy(d => d)
                .ToList();
            departments.Insert(0, "All"); // Add "All" as the first item
            DepartmentComboBox.ItemsSource = departments;
            DepartmentComboBox.SelectedIndex = 0; // Default to "All"

            PopulateInstructorCards(instructors);
            // Add a small delay to ensure the UI has finished processing
            await Task.Delay(100);
            loadingWindow.Close();

            // Re-enable the main window
            this.IsEnabled = true;
        }

        private async Task<List<Instructor>> GetInstructorsFromFirebase()
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://sti-onn-d0161-default-rtdb.asia-southeast1.firebasedatabase.app/");
                var instructorList = await firebaseClient.Child("instructors").OnceAsync<Instructor>();

                return instructorList.Select(item => new Instructor
                {
                    Name = item.Object.Name,
                    AvatarUrl = item.Object.AvatarUrl,
                    Department = item.Object.Department, // Ensure this field exists in your data
                    ScheduleUrl = item.Object.ScheduleUrl,
                    Timestamp = item.Object.Timestamp
                }).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching instructors: " + ex.Message);
                return new List<Instructor>();
            }
        }

        private void DepartmentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if instructors are loaded
            if (Instructors == null) return;

            // Get the selected department
            var selectedDepartment = DepartmentComboBox.SelectedItem as string;

            // Filter instructors based on the selected department
            List<Instructor> filteredInstructors;
            if (selectedDepartment == "All")
            {
                // Show all instructors if "All" is selected
                filteredInstructors = Instructors.ToList();
                DepartmentDescriptionTextBlock.Text = "All Departments";  // Display description for 'All'
            }
            else
            {
                // Filter for the selected department only
                filteredInstructors = Instructors
                    .Where(i => i.Department == selectedDepartment)
                    .ToList();

                // Display department description based on selection
                string departmentDescription = GetDepartmentDescription(selectedDepartment);
                DepartmentDescriptionTextBlock.Text = departmentDescription;
            }

            // Update the instructor cards to show only filtered instructors
            PopulateInstructorCards(filteredInstructors);
        }

        private string GetDepartmentDescription(string department)
        {
            // Define descriptions for each department
            switch (department)
            {
                case "IT":
                    return "Information Technology Department";
                case "SHS":
                    return "Senior High School Department";
                case "GE":
                    return "General Education Department";
                case "THM":
                    return "Tourism and Hospitality Management Department";
                case "AMT":
                    return "Academic Management Team (Head) Department";
                default:
                    return "";
            }
        }

        private void PopulateInstructorCards(List<Instructor> instructors)
        {
            InstructorWrapPanel.Children.Clear();

            if (instructors.Count == 0)
            {
                DisplayNoInstructorsMessage();
                return;
            }

            foreach (var instructor in instructors)
            {
                var card = CreateInstructorCard(instructor);
                InstructorWrapPanel.Children.Add(card);
            }
        }

        private void DisplayNoInstructorsMessage()
        {
            var noInstructorsText = new TextBlock
            {
                Text = "No instructors available.",
                FontSize = 20,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(20),
                Foreground = Brushes.Gray
            };
            InstructorWrapPanel.Children.Add(noInstructorsText);
        }

        private Border CreateInstructorCard(Instructor instructor)
        {
            var cardBorder = new Border
            {
                Width = 250,
                Height = 270,
                Margin = new Thickness(15),
                Background = Brushes.White,
                CornerRadius = new CornerRadius(15),
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Gray,
                    BlurRadius = 8,
                    ShadowDepth = 2
                }
            };
            // Attach click event to the card border
            cardBorder.MouseLeftButtonUp += (s, e) => OpenSchedule(instructor.ScheduleUrl, instructor.Name, instructor.AvatarUrl);

            // Add hover effect
            cardBorder.MouseEnter += (s, e) =>
            {
                cardBorder.Background = new SolidColorBrush(Color.FromRgb(230, 240, 255)); // Light blue on hover
                cardBorder.BorderBrush = Brushes.SkyBlue;
            };
            cardBorder.MouseLeave += (s, e) =>
            {
                cardBorder.Background = Brushes.White; // Reset to original color
                cardBorder.BorderBrush = Brushes.LightGray;
            };

            var stackPanel = new StackPanel
            {
                Children =
                {
                    CreateNameTextBlock(instructor.Name),
                    CreateAvatarImage(instructor.AvatarUrl),
                    CreateDepartmentTextBlock(instructor.Department)
                }
            };

            cardBorder.Child = stackPanel;
            return cardBorder;
        }

        private TextBlock CreateNameTextBlock(string name)
        {
            return new TextBlock
            {
                Text = name,
                FontWeight = FontWeights.Bold,
                FontSize = 20,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            };
        }

        private Image CreateAvatarImage(string avatarUrl)
        {
            return new Image
            {
                Source = new BitmapImage(new Uri(avatarUrl)),
                Height = 150,
                Width = 150,
                Stretch = Stretch.UniformToFill,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                Clip = new EllipseGeometry(new Rect(0, 0, 150, 150))  // Makes the image circula
            };
        }
        private TextBlock CreateDepartmentTextBlock(string department)
        {
            return new TextBlock
            {
                Text = "Department: " + department,
                FontWeight = FontWeights.Bold,
                FontSize = 20,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(5)
            };
        }


        private async void OpenSchedule(string scheduleUrl, string instructorName, string instructorAvatarUrl)
        {
            if (!string.IsNullOrEmpty(scheduleUrl))
            {
                // Show the dim overlay
                DimOverlay.Visibility = Visibility.Visible;

                // Show loading screen while the schedule is loading
                var loadingWindow = new Loading { LoadingMessage = "Loading Schedule, please wait..." };
                loadingWindow.Show();

                var scheduleViewer = new ScheduleViewer();
                await scheduleViewer.LoadSchedule(scheduleUrl, instructorName, instructorAvatarUrl);

                // Hide the loading window when ScheduleViewer closes
                loadingWindow.Close();

                scheduleViewer.ShowDialog();
                // Hide the dim overlay when ScheduleViewer closes
                DimOverlay.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("No schedule available for this instructor.");
            }
        }
        public class Instructor
        {
            public string Name { get; set; }
            public string AvatarUrl { get; set; }
            public string Department { get; set; } // Department field
            public string ScheduleUrl { get; set; }
            public long Timestamp { get; set; }
        }

        private void touch_Click_1(object sender, RoutedEventArgs e)
        {
            Home main = new Home();
            main.Show();
            this.Close();
        }
    }
}