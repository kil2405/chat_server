using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using ServerCore;

class PacketHandler
{
    public static void C_LoginHandler(PacketSession session, IMessage packet)
    {
        Console.WriteLine("C_LoginHandler");

        C_Login loginPacket = (C_Login)packet;
        ClientSession clientSession = (ClientSession)session;

        clientSession.HandleLogin(loginPacket);
    }

    public static void C_ChatHandler(PacketSession session, IMessage packet)
    {
        C_Chat chatPacket = (C_Chat)packet;
        ClientSession clientSession = (ClientSession)session;

        ChannelManager.Instance.ChatMessage(chatPacket);
    }

    public static void C_EnterChannelHandler(PacketSession session, IMessage packet)
    {
        C_EnterChannel enterChannelPacket = (C_EnterChannel)packet;
        ClientSession clientSession = (ClientSession)session;

        if (clientSession.Player != null)
        {
            if (enterChannelPacket.ChannelType == ChannelType.All)
            {
                Channel channel = ChannelManager.Instance.Add(enterChannelPacket.ChannelId, clientSession.Player);
                
            }
            else if (enterChannelPacket.ChannelType == ChannelType.Region)
            {
            }
            else
            {
            }
        }
    }

    public static void C_RequestChannelHandler(PacketSession session, IMessage packet)
    {
        C_RequestChannel requestChannelPacket = (C_RequestChannel)packet;
        ClientSession clientSession = (ClientSession)session;

        List<Channel> channels = ChannelManager.Instance.GetChannels(requestChannelPacket.Type, requestChannelPacket.Region);

        S_RequestChannel channelPacket = new S_RequestChannel();
        foreach(Channel channel in channels)
        {
            ChannelInfo info = new ChannelInfo();
            info.ChannelId = channel.ChannelId;
            info.ChannelType = channel.Type;
            info.Region = channel.Region;
            info.PlayerCount = channel.PlayerCount;

            channelPacket.Channels.Add(info);
        }

        clientSession.Send(channelPacket);
    }
}