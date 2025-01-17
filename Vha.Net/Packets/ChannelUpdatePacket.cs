/*
* Vha.Net
* Copyright (C) 2005-2011 Remco van Oosterhout
* See Credits.txt for all aknowledgements.
*
* This program is free software; you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation; version 2 of the License only.
*
* This program is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
*
* You should have received a copy of the GNU General Public License
* along with this program; if not, write to the Free Software
* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307
* USA
*/

using System;
using System.Net;
using Vha.Common;

namespace Vha.Net.Packets
{
    internal class ChannelUpdatePacket : Packet
    {
        internal ChannelUpdatePacket(Packet.Type type, byte[] data) : base(type, data) { }
        internal ChannelUpdatePacket(BigInteger channelID, ChannelFlags flags, string tag)
            : base(Packet.Type.CHANNEL_UPDATE)
        {
            this.AddData(channelID);
            this.AddData(NetConvert.HostToNetworkOrder((uint)flags));
            this.AddData(tag);
        }
    }
}
