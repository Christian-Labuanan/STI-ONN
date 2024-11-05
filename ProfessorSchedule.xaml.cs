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
using Firebase.Database;
using Firebase.Database.Query;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

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
            var instructors = await GetInstructorsFromFirebase();
            PopulateInstructorCards(instructors);
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
                Width = 300,
                Height = 300,
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

            var stackPanel = new StackPanel
            {
                Children =
                {
                    CreateNameTextBlock(instructor.Name),
                    CreateAvatarImage(instructor.AvatarUrl),
                    CreateViewScheduleButton(instructor.ScheduleUrl, instructor.Name, instructor.AvatarUrl)
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
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }

        private Button CreateViewScheduleButton(string scheduleUrl, string instructorName, string instructorAvatarUrl)
        {
            var button = new Button
            {
                Content = "View Schedule",
                Margin = new Thickness(10),
                Padding = new Thickness(10),
                Background = Brushes.LightBlue,
                BorderBrush = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            button.Click += (s, e) => OpenSchedule(scheduleUrl, instructorName, instructorAvatarUrl);
            return button;
        }

        private async void OpenSchedule(string scheduleUrl, string instructorName, string instructorAvatarUrl)
        {
            if (!string.IsNullOrEmpty(scheduleUrl))
            {
                var scheduleViewer = new ScheduleViewer();
                await scheduleViewer.LoadSchedule(scheduleUrl, instructorName, instructorAvatarUrl);
                scheduleViewer.ShowDialog();
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