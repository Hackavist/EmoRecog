using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace EmoRecog
{
    static class Networking
    {
        static Socket TCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static public async Task<string> Connect()
        {
            Socket UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            UDPSocket.EnableBroadcast = true;
            UDPSocket.Bind(endPoint);
            byte[] Message = new byte[1024];
            await Task.Run(() =>
            {
                while (true)
                {
                    int n = UDPSocket.ReceiveFrom(Message, ref endPoint);
                    string s = Encoding.ASCII.GetString(Message, 0, n);
                    if (s.StartsWith("EmoRecog:"))
                    {
                        int port = int.Parse(s.Substring(s.IndexOf(':') + 1).Trim());
                        TCPSocket.Connect(new IPEndPoint(((IPEndPoint)endPoint).Address, port));
                        if(TCPSocket.Connected)
                        {
                            break;
                        }
                    }
                }
            });
            return ((IPEndPoint)TCPSocket.RemoteEndPoint).ToString();
        }
    }
}
