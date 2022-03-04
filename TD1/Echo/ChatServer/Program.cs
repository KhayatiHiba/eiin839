using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace Echo
{
    class EchoServer
    {
        [Obsolete]
        static void Main(string[] args)
        {

            Console.CancelKeyPress += delegate
            {
                System.Environment.Exit(0);
            };

            TcpListener ServerSocket = new TcpListener(5000);
            ServerSocket.Start();

            Console.WriteLine("Server started.");
            while (true)
            {
                TcpClient clientSocket = ServerSocket.AcceptTcpClient();
                handleClient client = new handleClient();
                client.startClient(clientSocket);
            }


        }
    }

    public class handleClient
    {
        static string HTTP_ROOT = @"D:\Documents\Study\02- Second Year\PS8\SOC\TDS\eiin839\TD1\Echo\www\pub";
        TcpClient clientSocket;
        public void startClient(TcpClient inClientSocket)
        {
            this.clientSocket = inClientSocket;
            Thread ctThread = new Thread(Echo);
            ctThread.Start();
        }

        private void Echo()
        {
            NetworkStream stream = clientSocket.GetStream();
            BinaryReader reader = new BinaryReader(stream);
            BinaryWriter writer = new BinaryWriter(stream);

            string header = "HTTP/1.0 200 OK";
            string file = File.ReadAllText(HTTP_ROOT + "\\index.html");

            while (true)
            {
                string response = "";

                //Reads what was written on the client console.
                string str = reader.ReadString();

                if (str.Equals("GET /index.html"))
                {
                    //Writes the information on the console.
                    Console.WriteLine("index.html retrieved!");
                    writer.Write(header + "\n" + file);
                }
                else
                {
                    //Writes the a bad request message on the console.
                    response = "403 Bad Request";
                    Console.WriteLine("Response :\n" + response);
                    writer.Write(response);
                }
            }
        }



    }

}