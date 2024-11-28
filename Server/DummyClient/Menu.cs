using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

public class Menu
{
    #region Singleton
    static Menu _menu = new Menu();
    public static Menu Instance { get { return _menu; } }
    #endregion

    public bool isChat = false;
    public ChannelPlayer? Player { get; set; }

    public void ShowMenu()
    {
        Console.WriteLine("1. Print Channels");
        Console.WriteLine("2. Create Channel");
        Console.WriteLine("3. Enter channel");
        Console.WriteLine("4. Exit");
        Console.Write("Select Menu: ");

        switch(Console.ReadLine())
        {
            case "1":
                SendRequestChannel();
                break;
            case "2":
                break;
            case "3":
                SendEnterChannel();
                break;
            case "4":
                break;
        }
    }

    void SendChat(int channelId)
    {
        if (Player == null)
            return;

        isChat = true;
        while(isChat)
        {
            C_Chat chat = new C_Chat();
            chat.ChannelId = channelId;
            chat.PlayerId = Player.PlayerId;
            chat.Message = Console.ReadLine();

            if(chat.Message == "exit")
            {
                isChat = false;
                break;
            }

            Send(chat);
        }
    }

    void SendRequestChannel()
    {
        C_RequestChannel packet = new C_RequestChannel();
        packet.Type = ChannelType.All;
        packet.Region = "";
        Send(packet);
    }

    void SendEnterChannel()
    {
        C_EnterChannel packet = new C_EnterChannel();
        packet.ChannelId = 0;
        packet.ChannelType = ChannelType.All;
        packet.Region = "";
        Send(packet);
    }

    public void HandleRequestChannel(S_RequestChannel packet)
    {
        Console.WriteLine($"Channel Count : {packet.Channels.Count}");

        foreach (ChannelInfo info in packet.Channels)
        {
            Console.WriteLine($"ChannelId({info.ChannelId}) ChannelType({info.ChannelType}) Region({info.Region}) PlayerCount({info.PlayerCount})");
        }

        ShowMenu();
    }

    public void HandleEnterChannel(S_EnterChannel packet)
    {
        Console.WriteLine($"ChannelId({packet.ChannelId})");

        Console.WriteLine($"PlayerList");
        foreach (ChannelPlayer player in packet.Players)
        {
            Console.WriteLine($"PlayerId({player.PlayerId}) Name({player.Name})");
        }

        SendChat(packet.ChannelId);
    }

    public void HandleChatMessage(S_Chat packet)
    {
        Console.WriteLine($"{packet.Name} : {packet.Message}");
    }
}