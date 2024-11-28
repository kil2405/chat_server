using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public class Listener
    {
        Socket? _listenSocket = null;
        Func<Session>? _sessionFactory = null;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            //바인딩
            _listenSocket.Bind(endPoint);

            //backlog : 최대 대기수
            _listenSocket.Listen(backlog);

            for (int i = 0; i < register; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            if(_listenSocket == null)
                return;

            args.AcceptSocket = null;

            try
            {
                bool panding = _listenSocket.AcceptAsync(args);
                if (panding == false)
                    OnAcceptCompleted(null, args);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void OnAcceptCompleted(object? sender, SocketAsyncEventArgs args)
        {
            if(args == null)
                return;

            if (_sessionFactory == null)
                return;

            try
            {
                if(args.SocketError == SocketError.Success && args.AcceptSocket is not null)
                {
                    Session session = _sessionFactory.Invoke();
                    session.Start(args.AcceptSocket);
                    session.OnConnected(args.AcceptSocket.RemoteEndPoint);
                }
                else
                {
                    Console.WriteLine($"OnAcceptCompleted Failed {args.SocketError}");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
