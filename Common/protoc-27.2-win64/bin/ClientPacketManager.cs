using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

#nullable enable

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();
	
	public Action<PacketSession, IMessage, ushort>? CustomHandler { get; set; } = null;

	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.SConnected, MakePacket<S_Connected>);
		_handler.Add((ushort)MsgId.SConnected, PacketHandler.S_ConnectedHandler);		
		_onRecv.Add((ushort)MsgId.SLogin, MakePacket<S_Login>);
		_handler.Add((ushort)MsgId.SLogin, PacketHandler.S_LoginHandler);		
		_onRecv.Add((ushort)MsgId.SChat, MakePacket<S_Chat>);
		_handler.Add((ushort)MsgId.SChat, PacketHandler.S_ChatHandler);		
		_onRecv.Add((ushort)MsgId.SRequestChannel, MakePacket<S_RequestChannel>);
		_handler.Add((ushort)MsgId.SRequestChannel, PacketHandler.S_RequestChannelHandler);		
		_onRecv.Add((ushort)MsgId.SEnterChannel, MakePacket<S_EnterChannel>);
		_handler.Add((ushort)MsgId.SEnterChannel, PacketHandler.S_EnterChannelHandler);		
		_onRecv.Add((ushort)MsgId.SLeaveChannel, MakePacket<S_LeaveChannel>);
		_handler.Add((ushort)MsgId.SLeaveChannel, PacketHandler.S_LeaveChannelHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		if (buffer.Array == null)
			return;

		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		if (_onRecv.TryGetValue(id, out Action<PacketSession, ArraySegment<byte>, ushort>? action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);
		
		if(CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			if (_handler.TryGetValue(id, out Action<PacketSession, IMessage>? action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage>? GetPacketHandler(ushort id)
	{
		if (_handler.TryGetValue(id, out Action<PacketSession, IMessage>? action))
			return action;
		return null;
	}
}