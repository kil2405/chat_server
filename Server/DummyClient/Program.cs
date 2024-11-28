using Server;
using SeverCore;
using System.Net;

namespace DummyClient
{
   class Program
    {
        static int DummyClientCount = 1;
        static void Main(string[] args)
        {
            Thread.Sleep(5000);

            // DNS
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate(); }, DummyClientCount);

            while (true) { ; } 
        }
    }
}