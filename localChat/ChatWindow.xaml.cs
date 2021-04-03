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

namespace localChat
{
    /// <summary>
    /// Логика взаимодействия для ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        private ChatManager chatManager;
        public ChatWindow()
        {
            InitializeComponent();
        }

        public ChatWindow(string name)
        {
            InitializeComponent();
            chatManager = new ChatManager(name);
            chatManager.Initialize();
        }

        private void btnSendClick(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            chatManager.Destroy();
            System.Environment.Exit(0);
        }
    }
}
