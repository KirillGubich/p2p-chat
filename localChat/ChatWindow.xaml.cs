using System.Threading;
using System.Windows;

namespace localChat
{
    public partial class ChatWindow : Window
    {
        private ChatManager chatManager;
        public static object MessageThreadLock = new object();
        public static object OnlineThreadLock = new object();

        public ChatWindow(string name)
        {
            InitializeComponent();
            chatManager = new ChatManager(name);
            InitChat();
        }

        private void InitChat()
        {
            Thread messageUpdater = new Thread(() => { UpdateMessageBox(); });
            Thread onlineUpdater = new Thread(() => { UpdateOnlineBox(); });
            messageUpdater.IsBackground = true;
            onlineUpdater.IsBackground = true;
            messageUpdater.Start();
            onlineUpdater.Start();
            chatManager.Initialize();
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

        private void UpdateMessageBox()
        {
            int i;
            int ListCount = 0;
            int messageCount;
            while (true)
            {
                Dispatcher.Invoke(() =>
                {
                    lock (MessageThreadLock)
                    {
                        messageCount = chatManager.MessageHistory.Count;
                        if (messageCount > ListCount)
                        {
                            messagesListBox.Items.Clear();
                            for (i = 0; i < chatManager.MessageHistory.Count; i++)
                            {
                                messagesListBox.Items.Add(chatManager.MessageHistory[i]);
                            }
                            ListCount = messagesListBox.Items.Count;

                            messagesListBox.ScrollIntoView(messagesListBox.Items[messagesListBox.Items.Count - 1]);
                        }

                    }

                });
                Thread.Sleep(200);
            }
        }

        private void UpdateOnlineBox()
        {
            Thread.Sleep(1000);
            int i;
            int listCount = 0;
            int onlineCount;
            while (true)
            {
                Dispatcher.Invoke(() =>
                {
                    lock (OnlineThreadLock)
                    {
                        onlineCount = chatManager.Clients.Count;
                        if (onlineCount != listCount)
                        {
                            clientsListBox.Items.Clear();
                            for (i = 0; i < chatManager.Clients.Count; i++)
                            {
                                clientsListBox.Items.Add(chatManager.Clients[i].Name + " [" + chatManager.Clients[i].IpAddress + "]");
                            }
                            listCount = clientsListBox.Items.Count;
                        }
                    }
                });
            }
        }
    }
}
