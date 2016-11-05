using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Utility
{
    //C#截获截获截获截获本机数据包数据包数据包数据包方法  发表人：jiamao  发表于：2010-04-06  标签：C# socket 截包 首先实例化一个socket: 用来接收所有的数据 C# 代码 
    //Socket s = new Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Raw,System.Net.Sockets.ProtocolType.IP);  
    //s.Bind(new System.Net.IPEndPoint(IPAddress.Parse(Dns.GetHostAddresses(Dns.GetHostName())[0].ToString()), 0)); 
    //s.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IP, System.Net.Sockets.SocketOptionName.HeaderIncluded, 1); 
    //s.IOControl(unchecked((int)0x98000001), new byte[4] { 1, 0, 0, 0 }, new byte[4]);     
    //s就可以接收到本机的所有数据包数据包数据包数据包   分析数据包数据包数据包数据包类型 ,当接收到数据包数据包数据包数据包后。可以用下面的例来分析 
    //只要new一个ippacket(ref byte[])实例就可以判断它的数据类例等信息   
    //C# 代码 
    public class IPPacket
    // RFC791
    {
        public byte Version;
        public byte HeaderLength;
        public byte TypeOfService;
        public ushort TotalLength;
        public ushort Identification;
        public byte Flags;
        public ushort FragmentOffset;
        public byte TimeToLive;
        public byte Protocol;
        public ushort HeaderChecksum;
        public System.Net.IPAddress SourceAddress;
        public System.Net.IPAddress DestinationAddress;
        public byte[] PacketData;
        public ICMPPacket ICMP;
        public TCPPacket TCP;
        public UDPPacket UDP;
        public IPPacket() : base() { }
        public IPPacket(ref byte[] Packet)
            : base()
        {
            try
            {
                Version = (byte)(Packet[0] >> 4);
                HeaderLength = (byte)((Packet[0] & 0x0F) * 4);
                TypeOfService = Packet[1];
                TotalLength = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 2));
                Identification = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 4));
                Flags = (byte)((Packet[6] & 0xE0) >> 5);
                FragmentOffset = (ushort)(System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 6)) & 0x1FFF);

                TimeToLive = Packet[8];
                Protocol = Packet[9];
                HeaderChecksum = (ushort)(System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 10)));
                SourceAddress = new System.Net.IPAddress(System.BitConverter.ToInt32(Packet, 12) & 0x00000000FFFFFFFF);
                DestinationAddress = new System.Net.IPAddress(System.BitConverter.ToInt32(Packet, 16) & 0x00000000FFFFFFFF);
                PacketData = new byte[TotalLength - HeaderLength];
                System.Buffer.BlockCopy(Packet, HeaderLength, PacketData, 0, PacketData.Length);
            }
            catch { }
            switch (Protocol)
            {
                case 1: ICMP = new ICMPPacket(ref PacketData); break;
                case 6: TCP = new TCPPacket(ref PacketData); break;
                case 17: UDP = new UDPPacket(ref PacketData); break;
            }
        }
        public byte[] GetBytes()
        {
            if (ICMP != null) { Protocol = 1; PacketData = ICMP.GetBytes(); }
            if (TCP != null) { Protocol = 6; PacketData = TCP.GetBytes(ref SourceAddress, ref DestinationAddress); }
            if (UDP != null) { Protocol = 17; PacketData = UDP.GetBytes(ref SourceAddress, ref DestinationAddress); }
            if (PacketData == null) PacketData = new byte[0];
            if (Version == 0) Version = 4;
            if (HeaderLength == 0) HeaderLength = 20;
            TotalLength = (ushort)(HeaderLength + PacketData.Length);
            byte[] Packet = new byte[TotalLength];

            if (TimeToLive == 0) TimeToLive = 128;

            Packet[0] = (byte)(((Version & 0x0F) << 4) | ((HeaderLength / 4) & 0x0F));
            Packet[1] = TypeOfService;

            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)TotalLength)), 0, Packet, 2, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)Identification)), 0, Packet, 4, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)((FragmentOffset & 0x1F) | ((Flags & 0x03) << 13)))), 0, Packet, 6, 2);
            Packet[8] = TimeToLive;

            Packet[9] = Protocol;

            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)0), 0, Packet, 10, 2);

            System.Buffer.BlockCopy(SourceAddress.GetAddressBytes(), 0, Packet, 12, 4);
            System.Buffer.BlockCopy(DestinationAddress.GetAddressBytes(), 0, Packet, 16, 4);
            System.Buffer.BlockCopy(PacketData, 0, Packet, HeaderLength, PacketData.Length);
            HeaderChecksum = GetChecksum(ref Packet, 0, HeaderLength - 1);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)HeaderChecksum)), 0, Packet, 10, 2);
            return Packet;
        }


        public static ushort GetChecksum(ref byte[] Packet, int start, int end)
        {
            uint CheckSum = 0;
            int i;
            for (i = start; i < end; i += 2) CheckSum += (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, i));
            if (i == end) CheckSum += (ushort)System.Net.IPAddress.NetworkToHostOrder((ushort)Packet[end]);
            while (CheckSum >> 16 != 0) CheckSum = (CheckSum & 0xFFFF) + (CheckSum >> 16);
            return (ushort)~CheckSum;
        }

    }

    public class TCPPacket
    //rfc793         
    {
        public ushort SourcePort;
        public ushort DestinationPort;
        public uint SequenceNumber;
        public uint AcknowledgmentNumber;
        public byte DataOffset;
        public byte ControlBits;
        public ushort Window;
        public ushort Checksum;
        public ushort UrgentPointer;
        public byte[] Options;
        public byte[] PacketData;

        public TCPPacket() : base() { }


        public TCPPacket(ref byte[] Packet)
            : base()
        {
            try
            {
                SourcePort = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 0));
                DestinationPort = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 2));
                SequenceNumber = (uint)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 4));
                AcknowledgmentNumber = (uint)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 8));
                DataOffset = (byte)((Packet[12] >> 4) * 4);
                ControlBits = (byte)((Packet[13] & 0x3F));
                Window = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 14));
                Checksum = (ushort)(System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 16)));
                UrgentPointer = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 18));
                Options = new byte[DataOffset - 20];
                System.Buffer.BlockCopy(Packet, 20, Options, 0, Options.Length);
                PacketData = new byte[Packet.Length - DataOffset];
                System.Buffer.BlockCopy(Packet, DataOffset, PacketData, 0, Packet.Length - DataOffset);
            }
            catch { }
        }

        public byte[] GetBytes(ref System.Net.IPAddress SourceAddress, ref System.Net.IPAddress DestinationAddress)
        {
            if (PacketData == null) PacketData = new byte[0];
            if (Options == null) Options = new byte[0];
            int OptionsLength = ((int)((Options.Length + 3) / 4)) * 4;
            DataOffset = (byte)(20 + OptionsLength);
            byte[] Packet = new byte[20 + OptionsLength + PacketData.Length];
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)SourcePort)), 0, Packet, 0, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)DestinationPort)), 0, Packet, 2, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((int)SequenceNumber)), 0, Packet, 4, 4);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((int)AcknowledgmentNumber)), 0, Packet, 8, 4);
            Packet[12] = (byte)(((Packet[12] & 0x0F) | ((DataOffset & 0x0F) << 4)) / 4);
            Packet[13] = (byte)(((Packet[13] & 0xC0) | (ControlBits & 0x3F)));
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)Window)), 0, Packet, 14, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)0), 0, Packet, 16, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)UrgentPointer)), 0, Packet, 18, 2);
            System.Buffer.BlockCopy(Options, 0, Packet, 20, Options.Length);
            if (OptionsLength > Options.Length) System.Buffer.BlockCopy(System.BitConverter.GetBytes((long)0), 0, Packet, 20 + Options.Length, OptionsLength - Options.Length);
            System.Buffer.BlockCopy(PacketData, 0, Packet, DataOffset, PacketData.Length);
            Checksum = GetChecksum(ref Packet, 0, DataOffset - 1, ref SourceAddress, ref DestinationAddress);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)Checksum), 0, Packet, 16, 2);
            return PacketData;
        }


        public ushort GetChecksum(ref byte[] Packet, int start, int end, ref System.Net.IPAddress SourceAddress, ref System.Net.IPAddress DestinationAddress)
        {
            byte[] PseudoPacket;
            PseudoPacket = new byte[12 + Packet.Length];
            System.Buffer.BlockCopy(SourceAddress.GetAddressBytes(), 0, PseudoPacket, 0, 4);
            System.Buffer.BlockCopy(DestinationAddress.GetAddressBytes(), 0, PseudoPacket, 4, 4);
            PseudoPacket[8] = 0;
            PseudoPacket[9] = 6;
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)Packet.Length)), 0, PseudoPacket, 10, 2);
            System.Buffer.BlockCopy(Packet, 0, PseudoPacket, 12, Packet.Length);
            return IPPacket.GetChecksum(ref PseudoPacket, 0, PseudoPacket.Length - 1);
        }
    }
    public class UDPPacket
    // rfc768         
    {
        public ushort SourcePort;
        public ushort DestinationPort;
        public ushort Length;
        public ushort Checksum;
        public byte[] PacketData;

        public UDPPacket() : base() { }

        public UDPPacket(ref byte[] Packet)
            : base()
        {
            try
            {
                SourcePort = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 0));
                DestinationPort = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 2));
                Length = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 4));
                Checksum = (ushort)System.Net.IPAddress.NetworkToHostOrder(System.BitConverter.ToInt16(Packet, 6));
                PacketData = new byte[Packet.Length - 8];
                System.Buffer.BlockCopy(Packet, 8, PacketData, 0, Packet.Length - 8);
            }
            catch { }
        }

        public byte[] GetBytes(ref System.Net.IPAddress SourceAddress, ref System.Net.IPAddress DestinationAddress)
        {

            if (PacketData == null) PacketData = new byte[0];
            byte[] Packet = new byte[8 + PacketData.Length];
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)SourcePort)), 0, Packet, 0, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)DestinationPort)), 0, Packet, 2, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)Length)), 0, Packet, 4, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)0), 0, Packet, 6, 2);
            System.Buffer.BlockCopy(PacketData, 0, Packet, 8, PacketData.Length);
            Checksum = GetChecksum(ref Packet, 0, 8 - 1, ref SourceAddress, ref DestinationAddress);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)Checksum), 0, Packet, 6, 2);
            return PacketData;
        }

        public ushort GetChecksum(ref byte[] Packet, int start, int end, ref System.Net.IPAddress SourceAddress, ref System.Net.IPAddress DestinationAddress)
        {
            byte[] PseudoPacket;
            PseudoPacket = new byte[12 + Packet.Length];
            System.Buffer.BlockCopy(SourceAddress.GetAddressBytes(), 0, PseudoPacket, 0, 4);
            System.Buffer.BlockCopy(DestinationAddress.GetAddressBytes(), 0, PseudoPacket, 4, 4);
            PseudoPacket[8] = 0;
            PseudoPacket[9] = 17;
            System.Buffer.BlockCopy(System.BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder((short)Packet.Length)), 0, PseudoPacket, 10, 2);
            System.Buffer.BlockCopy(Packet, 0, PseudoPacket, 12, Packet.Length);
            return IPPacket.GetChecksum(ref PseudoPacket, 0, PseudoPacket.Length - 1);
        }
    }
    public class ICMPPacket
    // rfc792       
    {
        public byte Type;
        public byte Code;
        public ushort Checksum;
        public byte[] PacketData;
        public ICMPMessage Message;

        public ICMPPacket() : base() { }

        public ICMPPacket(ref byte[] Packet)
            : base()
        {
            try
            {
                Type = (byte)Packet[0];
                Code = (byte)Packet[1];
                Checksum = (ushort)System.BitConverter.ToInt16(Packet, 2);
                PacketData = new byte[Packet.Length - 4];
                System.Buffer.BlockCopy(Packet, 4, PacketData, 0, Packet.Length - 4);
            }
            catch { }

            switch (Type)
            {
                case 0: Message = new ICMPEchoReply(ref PacketData); break;
                case 3: Message = new ICMPDestinationUnreachable(ref PacketData); break;
                case 4: Message = new ICMPSourceQuench(ref PacketData); break;
                case 5: Message = new ICMPRedirect(ref PacketData); break;
                case 8: Message = new ICMPEcho(ref PacketData); break;
                case 11: Message = new ICMPTimeExceeded(ref PacketData); break;
                case 12: Message = new ICMPParameterProblem(ref PacketData); break;
                case 13: Message = new ICMPTimestamp(ref PacketData); break;
                case 14: Message = new ICMPTimestampReply(ref PacketData); break;
                case 15: Message = new ICMPInformationRequest(ref PacketData); break;
                case 16: Message = new ICMPInformationReply(ref PacketData); break;
            }
        }

        public byte[] GetBytes()
        {
            if (Message != null) PacketData = Message.GetBytes();
            if (Message is ICMPEchoReply) Type = 0;
            else
                if (Message is ICMPDestinationUnreachable) Type = 3;
                else
                    if (Message is ICMPSourceQuench) Type = 4;
                    else
                        if (Message is ICMPRedirect) Type = 5;
                        else
                            if (Message is ICMPEcho) Type = 8;
                            else
                                if (Message is ICMPTimeExceeded) Type = 11;
                                else
                                    if (Message is ICMPParameterProblem) Type = 12;
                                    else
                                        if (Message is ICMPTimestamp) Type = 13;
                                        else
                                            if (Message is ICMPTimestampReply) Type = 14;
                                            else
                                                if (Message is ICMPInformationRequest) Type = 15;
                                                else
                                                    if (Message is ICMPInformationReply) Type = 16;

            if (PacketData == null) PacketData = new byte[0];
            byte[] Packet = new byte[4 + PacketData.Length];
            Packet[0] = Type;
            Packet[1] = Code;
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)0), 0, Packet, 2, 2);
            System.Buffer.BlockCopy(PacketData, 0, Packet, 4, PacketData.Length);
            Checksum = GetChecksum(ref Packet, 0, Packet.Length - 1);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)Checksum), 0, Packet, 2, 2);
            return Packet;
        }

        public ushort GetChecksum(ref byte[] Packet, int start, int end)
        {
            uint CheckSum = 0;
            int i;
            for (i = start; i < end; i += 2) CheckSum += (ushort)System.BitConverter.ToInt16(Packet, i);
            if (i == end) CheckSum += (ushort)Packet[end];
            while (CheckSum >> 16 != 0) CheckSum = (CheckSum & 0xFFFF) + (CheckSum >> 16);
            return (ushort)~CheckSum;
        }
    }

    public abstract class ICMPMessage
    {
        public abstract byte[] GetBytes();
    }

    public class ICMPIPHeaderReply : ICMPMessage
    {
        public byte[] Data;
        public IPPacket IP;

        public ICMPIPHeaderReply() : base() { }

        public ICMPIPHeaderReply(ref byte[] Packet)
            : base()
        {
            try
            {
                Data = new byte[Packet.Length - 4];
                System.Buffer.BlockCopy(Packet, 4, Data, 0, Data.Length);
                IP = new IPPacket(ref Data);
            }
            catch { }
        }

        public override byte[] GetBytes()
        {
            if (Data == null) Data = new byte[0];
            byte[] Packet = new byte[4 + Data.Length];
            System.Buffer.BlockCopy(Data, 0, Packet, 4, Data.Length);
            return Packet;
        }
    }

    public class ICMPEcho : ICMPMessage
    {
        public ushort Identifier;
        public ushort SequenceNumber;
        public string Data;

        public ICMPEcho() : base() { }

        public ICMPEcho(ref byte[] Packet)
            : base()
        {
            try
            {
                Identifier = (ushort)System.BitConverter.ToInt16(Packet, 0);
                SequenceNumber = (ushort)System.BitConverter.ToInt16(Packet, 2);
                Data = System.Text.Encoding.ASCII.GetString(Packet, 4, Packet.Length - 4);
            }
            catch { }
        }

        public override byte[] GetBytes()
        {
            if (Data == null) Data = "";
            byte[] Packet = new byte[4 + Data.Length];
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)Identifier), 0, Packet, 0, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)SequenceNumber), 0, Packet, 2, 2);
            System.Buffer.BlockCopy(System.Text.Encoding.ASCII.GetBytes(Data), 0, Packet, 4, Data.Length);
            return Packet;
        }
    }

    public class ICMPEchoReply : ICMPEcho
    {
        public ICMPEchoReply() : base() { }

        public ICMPEchoReply(ref byte[] Packet) : base(ref Packet) { }
    }

    public class ICMPRedirect : ICMPMessage
    {
        public ulong GatewayInternetAddress;
        public byte[] Data;

        public enum CodeEnum
        {
            RedirectDatagramsForTheNetwork = 0,
            RedirectDatagramsForTheHost = 1,
            RedirectDatagramsForTheTypeOfServiceAndNetwork = 2,
            RedirectDatagramsForTheTypeOfServiceAndHost = 3
        }


        public ICMPRedirect() : base() { }


        public ICMPRedirect(ref byte[] Packet)
            : base()
        {
            try
            {

                GatewayInternetAddress = (ulong)System.BitConverter.ToInt32(Packet, 0);
                Data = new byte[Packet.Length - 4];
                System.Buffer.BlockCopy(Packet, 0, Data, 4, Packet.Length);
            }
            catch { }
        }

        public override byte[] GetBytes()
        {
            if (Data == null) Data = new byte[0];
            byte[] Packet = new byte[4 + Data.Length];
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((long)GatewayInternetAddress), 0, Packet, 0, 4);
            System.Buffer.BlockCopy(Data, 0, Packet, 4, Data.Length);
            return Packet;
        }
    }

    public class ICMPDestinationUnreachable : ICMPIPHeaderReply
    {
        public enum CodeEnum
        {
            NetUnreachable = 0,
            HostUnreachable = 1,
            ProtocolUnreachable = 2,
            PortUnreachable = 3,
            FragmentationNeededAndDFSet = 4,
            SourceRouteFailed = 5
        }

        public ICMPDestinationUnreachable() : base() { }
        public ICMPDestinationUnreachable(ref byte[] Packet) : base(ref Packet) { }
    }

    public class ICMPSourceQuench : ICMPIPHeaderReply
    {
        public ICMPSourceQuench() : base() { }
        public ICMPSourceQuench(ref byte[] Packet) : base(ref Packet) { }
    }

    public class ICMPTimeExceeded : ICMPIPHeaderReply
    {
        public enum CodeEnum
        {
            TimeToLiveExceededInTransit = 0,
            FragmentReassemblyTimeExceeded = 1
        }

        public ICMPTimeExceeded() : base() { }
        public ICMPTimeExceeded(ref byte[] Packet) : base(ref Packet) { }
    }

    public class ICMPParameterProblem : ICMPMessage
    {
        public byte Pointer;
        public byte[] Data;

        public ICMPParameterProblem() : base() { }


        public ICMPParameterProblem(ref byte[] Packet)
            : base()
        {
            try
            {
                Pointer = Packet[0];
                Data = new byte[Packet.Length - 4];
                System.Buffer.BlockCopy(Packet, 0, Data, 4, Packet.Length);
            }
            catch { }
        }

        public override byte[] GetBytes()
        {

            if (Data == null) Data = new byte[0];
            byte[] Packet = new byte[4 + Data.Length];
            Packet[0] = Pointer;
            System.Buffer.BlockCopy(Data, 0, Packet, 4, Data.Length);
            return Packet;
        }
    }

    public class ICMPTimestamp : ICMPMessage
    {
        public ushort Identifier;
        public ushort SequenceNumber;
        public ulong OriginateTimestamp;
        public ulong ReceiveTimestamp;
        public ulong TransmitTimestamp;

        public ICMPTimestamp() : base() { }

        public ICMPTimestamp(ref byte[] Packet)
            : base()
        {
            try
            {
                Identifier = (ushort)System.BitConverter.ToInt16(Packet, 0);
                SequenceNumber = (ushort)System.BitConverter.ToInt16(Packet, 2);
                OriginateTimestamp = (ulong)System.BitConverter.ToInt32(Packet, 4);
                ReceiveTimestamp = (ulong)System.BitConverter.ToInt32(Packet, 8);
                TransmitTimestamp = (ulong)System.BitConverter.ToInt32(Packet, 12);
            }
            catch { }
        }

        public override byte[] GetBytes()
        {
            byte[] Packet = new byte[16];
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)Identifier), 0, Packet, 0, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)SequenceNumber), 0, Packet, 2, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((long)OriginateTimestamp), 0, Packet, 4, 4);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((long)ReceiveTimestamp), 0, Packet, 8, 4);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((long)TransmitTimestamp), 0, Packet, 12, 4);
            return Packet;
        }
    }

    public class ICMPTimestampReply : ICMPTimestamp
    {
        public ICMPTimestampReply() : base() { }
        public ICMPTimestampReply(ref byte[] Packet) : base(ref Packet) { }
    }

    public class ICMPInformationRequest : ICMPMessage
    {
        public ushort Identifier;
        public ushort SequenceNumber;

        public ICMPInformationRequest() : base() { }

        public ICMPInformationRequest(ref byte[] Packet)
            : base()
        {
            try
            {
                Identifier = (ushort)System.BitConverter.ToInt16(Packet, 0);
                SequenceNumber = (ushort)System.BitConverter.ToInt16(Packet, 2);
            }
            catch { }
        }

        public override byte[] GetBytes()
        {
            byte[] Packet = new byte[4];
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)Identifier), 0, Packet, 0, 2);
            System.Buffer.BlockCopy(System.BitConverter.GetBytes((short)SequenceNumber), 0, Packet, 2, 2);
            return Packet;
        }
    }

    public class ICMPInformationReply : ICMPInformationRequest
    {
        public ICMPInformationReply() : base() { }

        public ICMPInformationReply(ref byte[] Packet) : base(ref Packet) { }
    }
}
