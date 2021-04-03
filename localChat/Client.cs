using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace localChat
{
    class Client
    {
        private string name;
        private IPAddress ipAddress;
        private TcpClient connection;

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
    }
}
