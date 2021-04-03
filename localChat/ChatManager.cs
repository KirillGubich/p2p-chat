using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace localChat
{
    class ChatManager
    {
        private List<Client> clients;
        private List<string> messageHistory;
        private string clientName;
        private ClientsRegistrar clientsRegistrar;
        private Messenger messenger;
        private const int HISTORY_RECEIVING_TIMEOUT = 5000;

        public ChatManager(string clientName)
        {
            this.clientName = clientName;
            clients = new List<Client>();
            messageHistory = new List<string>();
            clientsRegistrar = new ClientsRegistrar();
            messenger = new Messenger();
        }

        public string ClientName
        {
            get
            {
                return clientName;
            }
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
            bool successfulyConected =  clientsRegistrar.SendRequest(clientName);
            StartReceiving();
            if (successfulyConected)
            {
                messageHistory.Add(clientName +" (" + DateTime.Now.ToLongTimeString() + "):" + " joined chat");
            }
            else
            {
                MessageBox.Show("Connection error");
            }
            Thread messageHistoryThread = new Thread(() => { RequestMessageHistory(); });
            messageHistoryThread.IsBackground = true;
            messageHistoryThread.Start();
        }

        public void SendMessage(string message)
        {
            messenger.Send(clients, message);
            messageHistory.Add(clientName + " (" + DateTime.Now.ToLongTimeString() + ")" + ": " + message);
        }

        public void Disconnect()
        {
            messenger.SendDisconnect(clients);
        }

        private void RequestMessageHistory()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            bool historyReceived = false;
            while (stopwatch.ElapsedMilliseconds < HISTORY_RECEIVING_TIMEOUT && !historyReceived)
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
