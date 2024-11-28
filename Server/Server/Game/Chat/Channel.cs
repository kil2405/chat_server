using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class Channel
    {
        public int ChannelId { get; set; }
        public ChannelType Type { get; set; }
        public string Region { get; set; }
        public int PlayerCount { get { return _players.Count; } }

        Dictionary<int, Player> _players = new Dictionary<int, Player>();
        object _lock = new object();

        public Channel(int channelId, ChannelType type, string region)
        {
            ChannelId = channelId;
            Type = type;
            Region = region;
        }

        public void Add(Player player)
        {
            // 채널에 플레이어 추가
            if(player == null || player.PlayerId <= 0)
                return;

            lock (_lock)
            {
                _players.Add(player.PlayerId, player);
                player.ChatChannel = this;
            }

            //채널 입장 알림
            S_EnterChannel enterPacket = new S_EnterChannel();
            enterPacket.ChannelId = ChannelId;
            foreach(Player p in _players.Values)
            {
                ChannelPlayer cp = new ChannelPlayer()
                {
                    PlayerId = p.PlayerId,
                    Name = p.Name
                };
                enterPacket.Players.Add(cp);
            }

            Broadcast(enterPacket);

            Console.WriteLine($"Enter Channel ({player.PlayerId}) Player");
        }

        public void Remove(int playerId)
        {
            // 채널에서 플레이어 제거
            if (playerId <= 0)
                return;

            Player? player = null;
            lock(_lock)
            {
                _players.TryGetValue(playerId, out player);
                if(player != null)
                {
                    player.ChatChannel = null;
                    _players.Remove(playerId);
                }
            }

            // 채널 퇴장 알림
            if(player != null)
            {
                // 전체 유저에게 채널 퇴장 알림
                S_LeaveChannel leavePacket = new S_LeaveChannel();
                leavePacket.PlayerId = player.PlayerId;
                Broadcast(leavePacket);

                Console.WriteLine($"Leave Channel ({playerId}) Player");
            }
        }

        public void ChatMessage(C_Chat pkt)
        {
            _players.TryGetValue(pkt.PlayerId, out Player? player);
            if(player != null)
            {
                S_Chat chat = new S_Chat()
                {
                    Name = player.Name,
                    Message = pkt.Message
                };
                Broadcast(chat);
            }
        }

        public void Broadcast(IMessage packet)
        {
            //player 전체에게 브로드캐스트
            List<Player> players = new List<Player>();
            lock (_lock)
            {
                players = _players.Values.ToList();
            }

            foreach (Player player in players)
            {
                if(player.Session != null)
                    player.Session.Send(packet);
            }
        }
    }
}
