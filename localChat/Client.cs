using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace localChat
{
    class Client
    {
        private string name;
        private IPAddress ipAddress;
        private TcpClient connection;
        private const int BUFFER_SIZE = 128;

        public Client(string name, IPAddress ipAddress, TcpClient connection)
        {
            this.name = name;
            this.ipAddress = ipAddress;
            this.connection = connection;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public IPAddress IpAddress
        {
            get
            {
                return ipAddress;
            }
        }

        public TcpClient Connection
        {
            get
            {
                return connection;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Client client &&
                   name == client.name &&
                   EqualityComparer<IPAddress>.Default.Equals(ipAddress, client.ipAddress);
        }

        public override int GetHashCode()
        {
            int hashCode = 862851038;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + EqualityComparer<IPAddress>.Default.GetHashCode(ipAddress);
            hashCode = hashCode * -1521134295 + EqualityComparer<TcpClient>.Default.GetHashCode(connection);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<IPAddress>.Default.GetHashCode(IpAddress);
            hashCode = hashCode * -1521134295 + EqualityComparer<TcpClient>.Default.GetHashCode(Connection);
            return hashCode;
        }

        public void ReceiveMessages(List<string> messageHistory, List<Client> clients)
        {
            NetworkStream OneUserStream = Connection.GetStream();
            try
            {
                while (true)
                {
                    byte[] byteMessage = new byte[BUFFER_SIZE];
                    StringBuilder MessageBuilder = new StringBuilder();
                    string message;
                    int RecBytes = 0;
                    do
                    {
                        RecBytes = OneUserStream.Read(byteMessage, 0, byteMessage.Length);
                        MessageBuilder.Append(Encoding.UTF8.GetString(byteMessage, 0, RecBytes));
                    }
                    while (OneUserStream.DataAvailable);

                    message = MessageBuilder.ToString();
                    int messageCode = message[0];
                    switch (messageCode)
                    {
                        case Messenger.MESSAGE_CODE:
                            {
                                string resultMessage;
                                resultMessage = name + " (" + DateTime.Now.ToLongTimeString() + "): " + message.Substring(1);
                                messageHistory.Add(resultMessage);
                                break;
                            }
                        case Messenger.NAME_CODE:
                            {
                                name = message.Substring(1);
                                break;
                            }
                        case Messenger.USER_DISCONNECT_CODE:
                            {
                                DisconnectClient(messageHistory, clients);
                                break;
                            }
                        case Messenger.MESSAGE_HISTORY_REQUEST:
                            {
                                ProccessMessageHistoryRequest(messageHistory);
                                break;
                            }
                        case Messenger.MESSAGE_HISTORY_RESPONCE:
                            {
                                ProccessMessageHistoryResponce(messageHistory, message);
                                break;
                            }
                    }
                }
            }
            catch
            {
                DisconnectClient(messageHistory, clients);
            }
            finally
            {
                if (OneUserStream != null)
                    OneUserStream.Close();
                if (Connection != null)
                    Connection.Close();

            }
        }

        private void DisconnectClient(List<string> messageHistory, List<Client> clients)
        {
            messageHistory.Add("User " + name + " left the chat session");
            clients.Remove(this);
        }

        private void ProccessMessageHistoryRequest(List<string> messageHistory)
        {
            string history = "";
            foreach (string historyItem in messageHistory)
            {
                history += historyItem + ((char)1).ToString();
            }
            Messenger messenger = new Messenger();
            messenger.SendMessageHistoryResponce(this, history);
        }

        private void ProccessMessageHistoryResponce(List<string> messageHistory, string message)
        {
            string fullHistory = message.Substring(1);
            List<string> history = new List<string>();
            while (fullHistory != "")
            {
                history.Add(fullHistory.Substring(0, fullHistory.IndexOf((char)1)));
                fullHistory = fullHistory.Substring(fullHistory.IndexOf((char)1) + 1);
            }
            lock (ChatWindow.MessageThreadLock)
            {
                messageHistory.Clear();
                foreach (string items in history)
                {
                    messageHistory.Add(items);
                }
            }
        }
    }
}
