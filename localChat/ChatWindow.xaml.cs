using System.Threading;
using System.Windows;

namespace localChat
{
    public partial class ChatWindow : Window
    {
        private ChatManager chatManager;
        public static object MessageThreadLock = new object();
        public static object OnlineThreadLock = new object();
        private static ChatWindow instance;

        public ChatWindow()
        {
            InitializeComponent();
        }

        public void init(string name)
        {
            chatManager = new ChatManager(name);
            chatManager.Initialize();
        }

        public static ChatWindow GetInstance()
        {
            if (instance == null)
            {
                instance = new ChatWindow();
            }
            return instance;
        }

        private void btnSendClick(object sender, RoutedEventArgs e)
        {
            string message = messageInputField.Text;
            messageInputField.Text = "";
            if (message != "")
            {
                chatManager.SendMessage(message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            chatManager.Disconnect();
            System.Environment.Exit(0);
        }

        public void UpdateMessageBox()
        {
            int i;
            int ListCount = 0;
            int messageCount;
            
            Dispatcher.Invoke(() =>
            {
                lock (MessageThreadLock)
                {
                    messageCount = chatManager.MessageHistory.Count;
                    messagesListBox.Items.Clear();
                    for (i = 0; i < chatManager.MessageHistory.Count; i++)
                    {
                        messagesListBox.Items.Add(chatManager.MessageHistory[i]);
                    }
                    ListCount = messagesListBox.Items.Count;
                    messagesListBox.ScrollIntoView(messagesListBox.Items[messagesListBox.Items.Count - 1]);
                }
            });

            
        }

        public void UpdateOnlineBox()
        {
            int i;
            int listCount = 0;
            int onlineCount;

            Dispatcher.Invoke(() =>
            {
                lock (OnlineThreadLock)
                {
                    onlineCount = chatManager.Clients.Count;                
                    clientsListBox.Items.Clear();
                    for (i = 0; i < chatManager.Clients.Count; i++)
                    {
                        clientsListBox.Items.Add(chatManager.Clients[i].Name + " [" + chatManager.Clients[i].IpAddress + "]");
                    }
                    listCount = clientsListBox.Items.Count;                  
                }
            });
        }
    }
}
