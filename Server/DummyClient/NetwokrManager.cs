using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServerCore;

namespace DummyClient
{
    public class NetwokrManager
    {
        #region Singleton
        static NetwokrManager? _instance;
        static NetwokrManager Instance { 
            get 
            { 
                if(_instance == null)
                    _instance = new NetwokrManager();
                return _instance;
            } 
        }
        #endregion

        ServerSession _session = new ServerSession();

        public void Send(IMessage packet)
        {
            _session.Send(packet);
        }

        public void Update()
        {
            List<PacketMessage> list = PacketQueue.Instance.PopAll();
            foreach(PacketMessage packet in list)
            {
                Action<PacketSession, IMessage>? handler = PacketManager.Instance.GetPacketHandler(packet.Id);
                if(handler != null && packet.Message != null)
                    handler.Invoke(_session, packet.Message);
            }
        }
    }
}
