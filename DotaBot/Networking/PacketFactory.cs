﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteamKit2;

namespace DotaBot
{
    static class PacketFactory
    {
        public static Packet GetPacket( byte[] data )
        {
            Packet packet = null;

            using ( var ms = new MemoryStream( data ) )
            using ( var br = new BinaryReader( ms, Encoding.UTF8 ) )
            {
                int channel = br.ReadInt32();

                switch ( channel )
                {
                    case -1:
                        packet = GetOOBPacket( data );
                        break;

                    case -2:
                        break; // todo: split packet

                    default:
                        packet = new NetMessagePacket();
                        break;
                }

                if ( packet == null )
                {
                    DebugLog.WriteLine( "PacketFactory", "Recieved unknown packet!" );
                    Debugger.Break();

                    return null;
                }

                ms.Seek( 0, SeekOrigin.Begin );
                packet.Deserialize( ms );
            }

            return packet;
        }

        static OutOfBandPacket GetOOBPacket( byte[] data )
        {
            using ( var ms = new MemoryStream( data ) )
            using ( var br = new BinaryReader( ms, Encoding.UTF8 ) )
            {
                br.ReadInt32(); // skip channel
                OutOfBandPacketType type = ( OutOfBandPacketType )br.ReadByte();

                switch ( type )
                {
                    case OutOfBandPacketType.ServerChallenge:
                        return new ServerChallengePacket();

                    case OutOfBandPacketType.ServerReject:
                        return new ServerRejectPacket();

                    case OutOfBandPacketType.ServerAccept:
                        return new ServerAcceptPacket();
                }
            }

            return null;
        }
    }
}
