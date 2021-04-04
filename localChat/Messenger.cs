﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace localChat
{
    class Messenger
    {
        private const int TCP_PORT = 31244;
        public const byte MESSAGE_CODE = 1;
        public const byte NAME_CODE = 2;
        public const byte USER_DISCONNECT_CODE = 3;
        public const byte MESSAGE_HISTORY_REQUEST = 4;
        public const byte MESSAGE_HISTORY_RESPONCE = 5;

        public void ReceiveConnectionRequests(List<Client> clients, List<string> messageHistory)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, TCP_PORT);
            tcpListener.Start();
            try
            {
                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    IPAddress senderIP = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address;
                    Client senderClient = clients.Find(client => client.IpAddress == senderIP);
                    if (senderClient == null)
                    {
                        lock (ChatWindow.OnlineThreadLock)
                        {
                            Client client = new Client(null, senderIP, tcpClient);
                            clients.Add(client);
                            senderClient = client;
                        }
                    }
                    StartMessageReceiving(senderClient, messageHistory, clients);
                }
            }
            catch
            {
                MessageBox.Show("Connection request receiving error");
            }
            finally
            {
                tcpListener.Stop();
            }
        }

        public void Send(List<Client> clients, string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] messageWithCode = new byte[messageBytes.Length + 1];
            messageWithCode[0] = MESSAGE_CODE;
            messageBytes.CopyTo(messageWithCode, 1);
            foreach (Client client in clients)
            {
                client.Connection.GetStream().Write(messageWithCode, 0, messageWithCode.Length);
            }
        }
        public void SendName(Client client, string name)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(name);
            byte[] messageWithCode = new byte[messageBytes.Length + 1];
            messageWithCode[0] = NAME_CODE;
            messageBytes.CopyTo(messageWithCode, 1);
            client.Connection.GetStream().Write(messageWithCode, 0, messageWithCode.Length);
        }

        public void SendDisconnect(List<Client> clients)
        {
            byte[] messageWithCode = new byte[1];
            messageWithCode[0] = USER_DISCONNECT_CODE;
            foreach (Client client in clients)
            {
                client.Connection.GetStream().Write(messageWithCode, 0, messageWithCode.Length);
            }
        }

        public void SendMessageHistoryReauest(Client client)
        {
            byte[] messageWithCode = new byte[1];
            messageWithCode[0] = MESSAGE_HISTORY_REQUEST;
            client.Connection.GetStream().Write(messageWithCode, 0, messageWithCode.Length);
        }

        public void SendMessageHistoryResponce(Client client, string messageHistory)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(messageHistory);
            byte[] messageWithCode = new byte[messageBytes.Length + 1];
            messageWithCode[0] = MESSAGE_HISTORY_RESPONCE;
            messageBytes.CopyTo(messageWithCode, 1);
            client.Connection.GetStream().Write(messageWithCode, 0, messageWithCode.Length);
        }

        private void StartMessageReceiving(Client senderClient, List<string> messageHistory, List<Client> clients)
        {
            Thread messageReceivingThread = new Thread(() => { senderClient.ReceiveMessages(messageHistory, clients); });
            messageReceivingThread.IsBackground = true;
            messageReceivingThread.Start();
        }

    }
}
