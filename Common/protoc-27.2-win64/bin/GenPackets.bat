protoc.exe -I=./ --csharp_out=./ ./Protocol.proto
IF ERRORLEVEL 1 PAUSE

START ../../../Server/PacketGenerator/bin/Debug/net8.0/PacketGenerator.exe ./Protocol.proto
XCOPY /Y Protocol.cs "../../../Server/Server/Packet"
XCOPY /Y Protocol.cs "Client"
XCOPY /Y ServerPacketManager.cs "../../../Server/Server/Packet"
XCOPY /Y ClientPacketManager.cs "Client"
