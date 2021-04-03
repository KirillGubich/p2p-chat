using System.Windows;

namespace localChat
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnectClick(object sender, RoutedEventArgs e)
        {
            ChatWindow chatWindow = new ChatWindow(nameInput.Text);
            chatWindow.Show();
            Close();
        }
    }
}
