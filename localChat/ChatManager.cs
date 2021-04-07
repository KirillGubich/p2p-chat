using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace localChat
{
    class ChatManager
    {
        private readonly List<Client> clients;
        private List<string> messageHistory;
        private readonly string clientName;
        private ClientsRegistrar clientsRegistrar;
        private readonly Messenger messenger;
        private const int HISTORY_REQUEST_TIMEOUT = 5000;
        private const int HISTORY_RECEIVING_TIMEOUT = 1000;

        public ChatManager(string clientName)
        {
            this.clientName = clientName;
            clients = new List<Client>();
            messageHistory = new List<string>();
            clientsRegistrar = new ClientsRegistrar();
            messenger = new Messenger();
        }

        public List<Client> Clients
        {
            get
            {
                return clients;
            }
        }

        public List<string> MessageHistory
        {
            get
            {
                return messageHistory;
            }
        }

        public void Initialize()
        {
            bool successfulyConected = clientsRegistrar.SendRequest(clientName);
            StartReceiving();
            if (!successfulyConected)
            {
                MessageBox.Show("Connection error");
            }
            messageHistory.Add("User " + clientName + " joined the chat session");
            ChatManager.UpdateView();
            Thread messageHistoryThread = new Thread(() => { RequestMessageHistory(); })
            {
                IsBackground = true
            };
            messageHistoryThread.Start();
        }

        public void SendMessage(string message)
        {
            messenger.Send(clients, message);
            messageHistory.Add(clientName + " (" + DateTime.Now.ToLongTimeString() + ")" + ": " + message);
            UpdateView();
        }

        public void Disconnect()
        {
            messenger.SendDisconnect(clients);
        }

        public static void UpdateView()
        {
            ChatWindow.GetInstance().UpdateMessageBox();
            ChatWindow.GetInstance().UpdateOnlineBox();
        }

        private void RequestMessageHistory()
        {
            Thread.Sleep(HISTORY_RECEIVING_TIMEOUT);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool historyReceived = false;
            while (stopwatch.ElapsedMilliseconds < HISTORY_REQUEST_TIMEOUT && !historyReceived)
            {
                if (clients.Count != 0)
                {
                    messenger.SendMessageHistoryReauest(clients[0]);
                    historyReceived = true;
                }
            }
        }

        private void StartReceiving()
        {
            Thread messengerThread = new Thread(() => { messenger.ReceiveConnectionRequests(clients, messageHistory); });
            Thread clientsRegistarThread = new Thread(() => { clientsRegistrar.AcceptRequests(clients, MessageHistory, messenger, clientName); });
            messengerThread.IsBackground = true;
            clientsRegistarThread.IsBackground = true;
            messengerThread.Start();
            clientsRegistarThread.Start();
        }
    }
}
