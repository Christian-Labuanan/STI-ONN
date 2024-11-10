using System.Windows;

namespace STI_ONN
{
    /// <summary>
    /// Interaction logic for Loading.xaml
    /// </summary>
    public partial class Loading : Window
    {
        public string LoadingMessage
        {
            get => LoadingTextBlock.Text;
            set => LoadingTextBlock.Text = value;
        }
        public Loading()
        {
            InitializeComponent();
        }
    }
}
