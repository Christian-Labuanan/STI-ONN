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

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for ProfessorSchedule.xaml
    /// </summary>
    public partial class ProfessorSchedule : Window
    {
        public ProfessorSchedule()
        {
            InitializeComponent();
        }
        private void touch_Click_1(object sender, RoutedEventArgs e)
        {
            Home main = new Home();
            main.Show();
            this.Close();
        }
    }
}
