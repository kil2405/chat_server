using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        int _sessionId = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        object _lock = new object();

        public List<ClientSession> GetSessions()
        {
            List<ClientSession> sessions = new List<ClientSession>();
            lock (_lock)
            {
                sessions = _sessions.Values.ToList();
            }
            return sessions;
        }

        public ClientSession Generate()
        {
            lock(_lock)
            {
                int sessionId = ++_sessionId;
                ClientSession session = new ClientSession()
                {
                    SessionId = sessionId
                };
                _sessions.Add(sessionId, session);

                Console.WriteLine($"Connected({_sessions.Count}) Players");
                return session;
            }
        }

        // Find 함수 호출 후 null 체크 필수
        public ClientSession? Find(int sessionId)
        {
            if (sessionId <= 0)
                return null;

            _sessions.TryGetValue(sessionId, out ClientSession? session);
            return session;
        }

        public void Remove(ClientSession session)
        {
            if (session == null)
                return;

            if (session.SessionId <= 0)
                return;

            lock(_lock)
            {
                _sessions.Remove(session.SessionId);
                Console.WriteLine($"Disconnect ({_sessions.Count}) Players");
            }
        }
    }
}
