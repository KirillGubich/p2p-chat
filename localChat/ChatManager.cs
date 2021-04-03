using System;
using System.Collections.Generic;
using System.Net;
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

        public ChatManager(string clientName)
        {
            this.clientName = clientName;
            clients = new List<Client>();
            messageHistory = new List<string>();
            clientsRegistrar = new ClientsRegistrar();
            messenger = new Messenger();
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
            if (!successfulyConected)
            {
                MessageBox.Show("Connection error");
            }
        }

        public void Destroy()
        {

        }

        private void StartReceiving()
        {
            //Thread messengerThread = new Thread(() => { });
            Thread clientsRegistarThread = new Thread(() => { clientsRegistrar.AcceptRequests(clients); });
            //messengerThread.IsBackground = true;
            clientsRegistarThread.IsBackground = true;
            //messengerThread.Start();
            clientsRegistarThread.Start();
        }

    }
}
