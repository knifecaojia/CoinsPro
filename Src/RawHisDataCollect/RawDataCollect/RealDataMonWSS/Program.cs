using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace RealDataMonWSS
{
    class Program
    {
        static WebSocket Client;
        static void Main(string[] args)
        {
            Client = new WebSocket("https://streamer.cryptocompare.com");
            Client.Opened += new EventHandler(Client_Opened);
            Client.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(Client_Error);
            Client.Closed += new EventHandler(Client_Closed);
            Client.MessageReceived += new EventHandler<MessageReceivedEventArgs>(Client_MessageReceived);
            Client.DataReceived += new EventHandler<DataReceivedEventArgs>(Client_DataReceived);
            Client.Open();
            
            Console.ReadKey();
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
