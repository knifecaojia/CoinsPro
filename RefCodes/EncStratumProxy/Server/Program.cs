using CoreLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static TcpListener ProxyListener = new TcpListener(IPAddress.Any, 4502);
        static int serverid = 1;
        static string startnotify;
        static string extranonce1;
        static int extranonce2 = 0;
        static int extranonce2_size;
        static ulong difficulty = 0;
        static string miningnotify = null;
        static StreamWriter serverWriter = null;
        static StreamReader serverReader = null;
        static StreamWriter proxyWriter = null;
        static StreamReader proxyReader = null;
        static List<MinerClientData> miners = new List<MinerClientData>();
        static Dictionary<int, MinerSubmit> submitids = new Dictionary<int, MinerSubmit>();

        const int BUFFER_SIZE = 4096;
        static CoreLib.Encrypt enc = new Encrypt();
        static void Main(string[] args)
        {
            Console.WriteLine("Press anykey to start RemoteServer on port 4502!");
            Console.ReadKey();
            ConnectToPool();

            new Task(() =>
            {
                // Accept clients.
                string serverString;
                Thread.CurrentThread.Name = "Server";
                while (true)
                {
                    try
                    {
                        serverString = serverReader.ReadLine();
                        //ECN send to pp side
                        //

                      
                        if (serverString == null)
                        {
                            // server bezarta a kapcsolatot
                            break;
                        }
                        Console.WriteLine("PS>: " + serverString);
                        serverString = enc.EncryptString(serverString);
                        ICollection myminers = miners;
                        lock (myminers.SyncRoot)
                        {
                            foreach (MinerClientData m in myminers)
                            {
                                Console.WriteLine("PS>:RemoteServer write encedstr to proxy");
                                m.clientWriter.WriteLine(serverString);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        // Server socket error or disconnect - exit loop.  Client will have to reconnect.
                        Console.WriteLine("ERROR: " + e.Message);
                        break;
                    }
                }
            }).Start();
            ProxyListener.Start();
            new Task(() =>
            {
                // Accept clients.
                while (true)
                {
                    TcpClient client = ProxyListener.AcceptTcpClient();
                    new Task(() => MinerClient(client)).Start();
                }
            }).Start();
            Console.WriteLine("Server listening on port 4502.  Press enter to exit.");
            Console.ReadLine();
            ProxyListener.Stop();
        }
        static void MinerClient(TcpClient client)
        {
            MinerClientData miner = null;
            try
            {
                NetworkStream clientStream = client.GetStream();
                Thread.CurrentThread.Name = "Client - " + client.Client.RemoteEndPoint;
                Console.WriteLine("Client - " + client.Client.RemoteEndPoint + " connected!");
                miner = new MinerClientData(client, new StreamReader(clientStream), new StreamWriter(clientStream));
                int userid = -1;

                string clientString;
                while (true)
                {
                    try
                    {
                        clientString = miner.clientReader.ReadLine();
                        //Decode

                        clientString = enc.DecryptString(clientString);
                        if (clientString.IndexOf("fuck gfw") >= 0)
                        {
                            miners.Add(miner);
                            miner.extranonce = string.Format("{0,2:X2}", extranonce2++);
                            miner.clientWriter.WriteLine(enc.EncryptString(string.Format("{{\"id\": 1, \"result\": [{0},  \"{1}{2}\", {3}], \"error\": null}}", startnotify, extranonce1, miner.extranonce, extranonce2_size - 1)));
                            if (difficulty > 0)
                            {
                                miner.clientWriter.WriteLine(enc.EncryptString(string.Format("{{\"params\": [{0}], \"id\": null, \"method\": \"mining.set_difficulty\"}}", difficulty)));
                            }
                            continue;
                           
                        }

                        if (clientString == null)
                        {
                            // client bezarta a kapcsolatot
                            break;
                        }
                        Console.WriteLine("CL>: " + clientString);

                        serverWriter.WriteLine(clientString);
                    }
                    catch
                    {
                        // Socket error or client disconnected - exit loop.  Client will have to reconnect.
                        break;
                    }
                }
                if (miners.Contains(miner))
                {
                    miners.Remove(miner);
                }
                client.Close();
            }
            catch
            {
                // nagy global mindent elkapo kivetel
                if (miners.Contains(miner))
                {
                    miners.Remove(miner);
                }
            }
        }
        static void ConnectToPool()
        {
            NetworkStream serverStream;
            TcpClient server = new TcpClient("stratum.antpool.com", 3333);
            serverStream = server.GetStream();
            serverWriter = new StreamWriter(serverStream);
            serverReader = new StreamReader(serverStream);
            serverWriter.AutoFlush = true;
            serverWriter.WriteLine(string.Format("{{\"id\": {0}, \"method\": \"mining.subscribe\", \"params\": []}}", serverid = 1));
        }

    }
    class MinerSubmit
    {
        public MinerClientData miner { get; set; }
        public int id { get; set; }
    }
    class MinerClientData
    {
        public TcpClient client { get; private set; }
        public StreamReader clientReader { get; private set; }
        public StreamWriter clientWriter { get; private set; }
        public string extranonce { get; set; }
        public MinerClientData(TcpClient client, StreamReader clientReader, StreamWriter clientWriter)
        {
            this.client = client;
            this.clientReader = clientReader;
            this.clientWriter = clientWriter;
            this.clientWriter.AutoFlush = true;
        }
    }
}
