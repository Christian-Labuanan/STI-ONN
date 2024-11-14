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
    /// Interaction logic for StaticSchedule.xaml
    /// </summary>
    public partial class StaticSchedule : Window
    {
        public StaticSchedule()
        {
            InitializeComponent();
            // Allow clicking outside the window to close it
        }
        // Show dim effect when window is opened
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dimmingOverlay.Visibility = Visibility.Visible;
        }

        // Optionally hide the dim effect when the window is closed or when other actions are taken
        private void Window_Closed(object sender, EventArgs e)
        {
            dimmingOverlay.Visibility = Visibility.Collapsed;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
