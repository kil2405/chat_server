using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game
{
    public class ChannelManager
    {
        #region Singleton
        static ChannelManager _channel = new ChannelManager();
        public static ChannelManager Instance { get { return _channel; } }
        #endregion

        Dictionary<int, Channel> _channels = new Dictionary<int, Channel>();
        object _lock = new object();
        int _channelId = 0;

        public Channel Create(ChannelType type, string name = "secret")
        {
            lock (_lock)
            {
                int newId = ++_channelId;
                Channel newChannel = new Channel(newId, type, name);
                _channels.Add(newId, newChannel);
                return newChannel;
            }
        }

        public void Remove(int channelId)
        {
            lock (_lock)
            {
                _channels.Remove(channelId);
            }
        }

        public Channel? Find(int channelId)
        {
            _channels.TryGetValue(channelId, out Channel? channel);
            return channel;
        }

        public List<Channel> GetChannelList()
        {
            List<Channel> channels = new List<Channel>();
            lock (_lock)
            {
                channels = _channels.Values.ToList();
            }
            return channels;
        }

        public List<Channel> GetChannels(ChannelType type, string region)
        {
            List<Channel> channels = new List<Channel>();
            lock (_lock)
            {
                foreach (Channel channel in _channels.Values)
                {
                    if (channel.Type == type || (channel.Type == type && channel.Region.Equals(region)))
                        channels.Add(channel);
                }
            }
            return channels;
        }

        public Channel Add(int channelId, Player player)
        {
            Channel? channel = Find(channelId);
            if(channel != null)
            {
                channel.Add(player);
                return channel;
            }
            else
            {
                channel = Create(ChannelType.All, "");
                channel.Add(player);
                return channel;
            }
        }

        // 지역 공개창 입장
        public Channel? Add(string region, Player player)
        {
            List<Channel> channels = GetChannelList();
            foreach (Channel channel in channels)
            {
                if (channel.Type == ChannelType.Region && channel.Region.Equals(region))
                {
                    channel.Add(player);
                    return channel;
                }
            }
            return null;
        }

        public Channel CreateSceretChannel(Player player, Player target)
        {
            Channel channel = Create(ChannelType.Secret);
            channel.Add(player);
            channel.Add(target);

            return channel;
        }

        public void ChatMessage(C_Chat pkt)
        {
            Channel? channel = Find(pkt.ChannelId);
            if (channel != null)
            {
                channel.ChatMessage(pkt);
            }
        }
    }
}
