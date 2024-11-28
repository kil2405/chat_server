using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Game;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ClientSession : PacketSession
    {
        public int SessionId { get; set; }
        public Player? Player { get; set; }

        object _lock = new object();
        List<ArraySegment<byte>> _reserveQueue = new List<ArraySegment<byte>>();

        // 패킷 모아보내기용 변수
        int _reservedSendBytes = 0;
        long _lastSendTick = 0;

        // 핑 체크 시간
        long _pingPongTick = 0;

        public void HandleLogin(C_Login pkt)
        {
            if (pkt == null)
                return;

            Player = new Player()
            {
                PlayerId = SessionId,
                Name = pkt.UniqueId,
                Session = this,
            };

            S_Login loginPacket = new S_Login()
            { 
                LoginOk = 1,
                PlayerId = Player.PlayerId,
                Name = pkt.UniqueId,
            };
            Send(loginPacket);
        }

        public void Send(IMessage packet)
        {
            string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
            MsgId msgid = (MsgId)Enum.Parse(typeof(MsgId), msgName);

            ushort size = (ushort)packet.CalculateSize();
            byte[] sendBuffer = new byte[size + 4];
            Array.Copy(BitConverter.GetBytes((ushort)size + 4), 0, sendBuffer, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes((ushort)msgid), 0, sendBuffer, 2, sizeof(ushort));
            Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

            lock (_lock)
            {
                _reserveQueue.Add(sendBuffer);
                _reservedSendBytes += sendBuffer.Length;
            }
        }

        // 실제 Network IO 보내는 부분 (업데이트에서 계속 돌려줘야한다)
        public void FlushSend()
        {
            List<ArraySegment<byte>>? sendList = null;

            lock (_lock)
            {
                // 0.1초가 지났거나, 패킷이 너무 많이 모였을때(1만바이트)
                long delta = (System.Environment.TickCount64 - _lastSendTick);
                if(delta < 100 && _reservedSendBytes < 10000)
                    return;

                _reservedSendBytes = 0;
                _lastSendTick = System.Environment.TickCount64;

                sendList = _reserveQueue;
                _reserveQueue = new List<ArraySegment<byte>>();
            }

            Send(sendList);
        }

        public override void OnConnected(EndPoint? endPoint)
        {
            //Console.WriteLine($"OnConnected : {endPoint}");

            S_Connected connectedPacket = new S_Connected();
            Send(connectedPacket);
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint? endPoint)
        {
            SessionManager.Instance.Remove(this);
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
