using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EmoRecogServer
{
    class Program
    {
        static readonly object ConsoleLock = new object();
        static Socket TCPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Socket UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        static Thread AnnoucerThread;
        static List<Thread> WorkerThreads = new List<Thread>();
        static void Announcer()
        {
            byte[] message = Encoding.ASCII.GetBytes("EmoRecog: " + ((IPEndPoint)TCPSocket.LocalEndPoint).Port);
            EndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, 0);
            while(true)
            {
                try
                {
                    UDPSocket.SendTo(message, endPoint);
                }
                catch(Exception e)
                {
                    lock(ConsoleLock)
                    {
                        Console.WriteLine("** Unable to brodcast, reason = " + e.Message);
                        return;
                    }
                }
                Thread.Sleep(1000);
            }
        }
        static void Worker(object o)
        {
            Socket s = (Socket)o;
            //Worker logic here
        }
        static void Main(string[] args)
        {
            Console.WriteLine("## Initializing TCP Socket");
            try
            {
                TCPSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                TCPSocket.Listen(100);
            }
            catch(Exception e)
            {
                Console.WriteLine("** Failed to initalize TCP Socket, Reason : " + e.Message);
                return;
            }
            Console.WriteLine("-> TCP Socket initalized successfully, listening to 100 connections and is bound to port " 
                              + ((IPEndPoint)TCPSocket.LocalEndPoint).Port);
            Console.WriteLine("## Enabling Broadcast in UDP Socket");
            UDPSocket.EnableBroadcast = true;
            Console.WriteLine("## Creating announce thread");
            AnnoucerThread = new Thread(new ThreadStart(Announcer));
            AnnoucerThread.Start();
            Console.WriteLine("## Waiting for connections");
            while(true)
            {
                Socket s = TCPSocket.Accept();
                lock(ConsoleLock)
                {
                    Console.WriteLine("-> " + ((IPEndPoint)s.RemoteEndPoint).Address.ToString() + " is connected");
                }
                Thread t = new Thread(new ParameterizedThreadStart(Worker));
                t.Start(s);
                lock(WorkerThreads)
                {
                    WorkerThreads.Add(t);
                }
            }
        }
    }
}
