using System.Windows;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();
        }

        private void OpenAndCloseCurrent(Window newWindow)
        {
            newWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenAndCloseCurrent(new Building());
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            OpenAndCloseCurrent(new Announcement());
        }

        private void Button_Click_ProfessorSchedule(object sender, RoutedEventArgs e)
        {
            OpenAndCloseCurrent(new ProfessorSchedule());
        }

        private void touch_Click_1(object sender, RoutedEventArgs e)
        {
            OpenAndCloseCurrent(new MainWindow());
        }

    }
}
