using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using SuperSocket.ClientEngine;
using WebSocket4Net;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;

namespace RealDataMonWSS
{
    class Program
    {
        static WebSocket Client;
        static void Main(string[] args)
        {
            //Client = new WebSocket("ws://streamer.cryptocompare.com");
            //Client.Opened += new EventHandler(Client_Opened);
            //Client.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(Client_Error);
            //Client.Closed += new EventHandler(Client_Closed);
            //Client.MessageReceived += new EventHandler<MessageReceivedEventArgs>(Client_MessageReceived);
            //Client.DataReceived += new EventHandler<DataReceivedEventArgs>(Client_DataReceived);
            //Client.Open();
            JObject jo = JObject.Parse("{subs:\"['5~CCCAGG~BTC~USD','5~CCCAGG~ETH~USD']\"}");
            var socket = IO.Socket("wss://streamer.cryptocompare.com");
            socket.On(Socket.EVENT_CONNECT, () =>
            {

                Console.WriteLine("Socket.EVENT_CONNECT" );
                JArray ja = new JArray();
                ja.Add("2~Bitstamp~BTC~USD"); // aggregate BTC
                //ja.Add("2~Poloniex~ETH~USD"); // aggregate ETH
                //ja.Add("2~Poloniex~LTC~USD"); // aggregate ETH
                JObject obj = new JObject();
                try
                {
                    obj.Add("subs", ja);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                socket.Emit("SubAdd", obj);
            });
          
            socket.On("m", (data) =>
            {
                Console.WriteLine(data);
                //Console.WriteLine(System.Text.Encoding.Default.GetString((byte[])data));
                //socket.Disconnect();
            });
            var socket1 = IO.Socket("wss://streamer.cryptocompare.com");
            socket1.On(Socket.EVENT_CONNECT, () =>
            {

                Console.WriteLine("Socket.EVENT_CONNECT");
                JArray ja = new JArray();
                //ja.Add("2~Bitstamp~BTC~USD"); // aggregate BTC
                ja.Add("2~Poloniex~ETH~USD"); // aggregate ETH
                //ja.Add("2~Poloniex~LTC~USD"); // aggregate ETH
                JObject obj = new JObject();
                try
                {
                    obj.Add("subs", ja);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                socket1.Emit("SubAdd", obj);
            });

            socket1.On("m", (data) =>
            {
                Console.WriteLine(data);
                //Console.WriteLine(System.Text.Encoding.Default.GetString((byte[])data));
                //socket.Disconnect();
            });
            Console.ReadKey();
            socket1.Disconnect();
        }
     
        private static void Client_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine("Client_DataReceived"+e.Data.ToString());
        }

        private static void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine("Client_MessageReceived"+e.Message.ToString());
        }

        private static void Client_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Client_Closed");
        }

        private static void Client_Error(object sender, ErrorEventArgs e)
        {
            Console.WriteLine("Client_Error"+e.Exception.Message);
        }

        private static void Client_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Client_Opened");
            Client.Send("['2~Poloniex~BTC~USD']");
        }
    }
}
