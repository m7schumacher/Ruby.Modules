using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Swiss;

namespace Ruby.Movements
{
    internal class TCPListener : Reflex
    {
        public TCPListener()
        {
            Name = "TCPListener";
        }

        public override void Initialize()
        {
            //Thread TCPrunner = new Thread(new ThreadStart(StartTCPNetwork));
            //TCPrunner.Start();
        }

        public override string Execute() { return null; }

        private void StartTCPNetwork()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 5004);
            listener.Start();

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();

                Thread TCPHandlerThread = new Thread(new ParameterizedThreadStart(TCPHandler));
                TCPHandlerThread.Start(client);
            }
        }

        private void TCPHandler(object client)
        {
            TcpClient mClient = (TcpClient)client;
            NetworkStream stream = mClient.GetStream();

            byte[] message = new byte[1024];
            stream.Read(message, 0, message.Length);

            string mess = Encoding.ASCII.GetString(message);

            int index = mess.IndexOf('\0');
            string actual = mess.Substring(0, index);

            string input = actual.SplitOnWhiteSpace()[1].Split('=')[1].Trim();
            string result = input.Replace("|", " ");

            
            stream.Close();
            mClient.Close();
        }
    }
}
