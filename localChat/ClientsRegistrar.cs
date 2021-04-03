using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace localChat
{
    class ClientsRegistrar
    {
        private const int UDP_PORT = 31234;
        private const int TCP_PORT = 31244;

        public void AcceptRequests(List<Client> clients)
        { 
            UdpClient udpCLient = null;
            try
            {              
                udpCLient = new UdpClient();
                IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Any, UDP_PORT);
                udpCLient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                udpCLient.ExclusiveAddressUse = false;
                udpCLient.Client.Bind(iPEndPoint);
                while (true)
                {
                    byte[] receivedData = udpCLient.Receive(ref iPEndPoint);
                    string clientName = Encoding.UTF8.GetString(receivedData);
                    if (clients.Find(x => x.IpAddress.ToString() == iPEndPoint.Address.ToString()) == null)
                    {
                        TcpClient tcpClient = EstablishConnection(iPEndPoint.Address);
                        Client client = new Client(clientName, iPEndPoint.Address, tcpClient);
                        clients.Add(client);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                if (udpCLient != null)
                {
                    udpCLient.Close();
                }
            }    
        }

        public bool SendRequest(string name)
        {  
            UdpClient udpCLient = null;
            try
            {
                IPAddress ipAddress = GetLocalIPAddress();
                if (ipAddress == null)
                {
                    return false;
                }
                IPAddress broadcastIP = CountBroadcastIP(ipAddress);
                IPEndPoint iPEndPoint = new IPEndPoint(broadcastIP, UDP_PORT);
                udpCLient = new UdpClient(UDP_PORT, AddressFamily.InterNetwork);
                udpCLient.Connect(iPEndPoint);
                byte[] registrationMessage = Encoding.UTF8.GetBytes(name);
                int bytesSent = udpCLient.Send(registrationMessage, registrationMessage.Length);
                return bytesSent == registrationMessage.Length;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (udpCLient != null)
                {
                    udpCLient.Close();
                }
            } 
        }

        private TcpClient EstablishConnection(IPAddress iPAddress)
        {
            try
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(new IPEndPoint(iPAddress, TCP_PORT));
                return tcpClient;
            }
            catch
            {
                MessageBox.Show("Connection error");
                return null;
            }
        }

        private IPAddress CountBroadcastIP(IPAddress ipAddress)
        {
            const string BROADCAST_MASK = "0.0.0.255";
            IPAddress broadcastIP = IPAddress.Parse(BROADCAST_MASK);
            broadcastIP.Address = broadcastIP.Address | ipAddress.Address;
            return broadcastIP;
        }

        private IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }
    }
}
