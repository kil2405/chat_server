using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using ServerCore;

class PacketHandler
{
    public static void S_ConnectedHandler(PacketSession session, IMessage packet)
    {
        Console.WriteLine("S_ConnectedHandler");
        
        C_Login loginPakcet = new C_Login();
        loginPakcet.UniqueId = Guid.NewGuid().ToString("N");

        ServerSession serverSession = (ServerSession)session;
        serverSession.Send(loginPakcet);
    }
    public static void S_LoginHandler(PacketSession session, IMessage packet)
    {
        Console.WriteLine("S_LoginHandler");
        S_Login loginPacket = (S_Login)packet;

        if(loginPacket.LoginOk == 1)
        {
            ChannelPlayer player = new ChannelPlayer()
            {
                PlayerId = loginPacket.PlayerId,
                Name = loginPacket.Name
            };

            ServerSession serverSession = (ServerSession)session;
            Menu.Instance.Player = player;
            Menu.Instance.ShowMenu();
        }
    }

    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        S_Chat chatPacket = (S_Chat)packet;
        Menu.Instance.HandleChatMessage(chatPacket);
    }

    public static void S_EnterChannelHandler(PacketSession session, IMessage packet)
    {
        S_EnterChannel enterChannelPacket = (S_EnterChannel)packet;
        ServerSession serverSession = (ServerSession)session;
        Menu.Instance.HandleEnterChannel( enterChannelPacket);
    }

    public static void S_LeaveChannelHandler(PacketSession session, IMessage packet)
    {
        S_LeaveChannel enterChannelPacket = (S_LeaveChannel)packet;
    }

    public static void S_RequestChannelHandler(PacketSession session, IMessage packet)
    {
        S_RequestChannel requestChannel = (S_RequestChannel)packet;
        ServerSession serverSession = (ServerSession)session;

        Menu.Instance.HandleRequestChannel(requestChannel);
    }
}