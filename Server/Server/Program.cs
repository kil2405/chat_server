using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System.Net;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();

        static void Main()
        {
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            {
                // 패킷 보내기용 쓰레드
                Thread t = new Thread(NetworkTask);
                t.Name = "Network Task";
                t.Start();
            }
        }

        static void NetworkTask()
        {
            while (true)
            {
                List<ClientSession> sessions = SessionManager.Instance.GetSessions();
                foreach (ClientSession session in sessions)
                {
                    session.FlushSend();
                }

                Thread.Sleep(0);
            }
        }
    }
}