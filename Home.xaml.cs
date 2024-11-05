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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Building bldg = new Building();
            bldg.Show();
            this.Close();
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            Announcement ancmnt = new Announcement();
            ancmnt.Show();
            this.Close();
        }
        private void Button_Click_ProfessorSchedule(object sender, RoutedEventArgs e)
        {
            ProfessorSchedule profSchedule = new ProfessorSchedule();
            profSchedule.Show();
            this.Close();
        }

        private void touch_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }

    }
}
