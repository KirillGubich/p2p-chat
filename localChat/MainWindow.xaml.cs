using System.Windows;

namespace localChat
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnConnectClick(object sender, RoutedEventArgs e)
        {
            ChatWindow chatWindow = ChatWindow.GetInstance();
            chatWindow.init(nameInput.Text);
            chatWindow.Show();
            Close();
        }
    }
}
