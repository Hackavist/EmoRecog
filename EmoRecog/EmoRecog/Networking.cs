using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EmoRecog
{
    static class Networking
    {
        static Socket TCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static public async Task Connect()
        {
            throw new NotImplementedException();
        }
    }
}
