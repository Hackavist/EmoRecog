using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;

namespace EmoRecog
{
    static class Networking
    {
        static Socket TCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static public async Task<string> Connect()
        {
            throw new NotImplementedException();
        }
    }
}
