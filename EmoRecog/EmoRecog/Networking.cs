using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace EmoRecog
{
    static class Networking
    {
        static Socket TCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static public async Task SendPhoto(Stream PhotoStream)
        {
            if (!TCPSocket.Connected)
            {
                throw new Exception("Not connected to server");
            }
            byte[] buffer = new byte[1024];
            //Mode: 1 for photo 
            TCPSocket.Send(new byte[] { 1 });
            //Length of file
            TCPSocket.Send(BitConverter.GetBytes((int)PhotoStream.Length));
            //Chunk size
            TCPSocket.Send(BitConverter.GetBytes((int)1024));
            await Task.Run(() =>
            {
                int len = (int)PhotoStream.Length;
                for (int i = 0; i < len; i += 1024)
                {
                    PhotoStream.Read(buffer, 0, (int)Math.Min(1024, PhotoStream.Length - i));
                    TCPSocket.Send(buffer);
                }
            });
        }
        static public async Task<string> Connect()
        {
            Socket UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 6969);
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
                        if (TCPSocket.Connected)
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
