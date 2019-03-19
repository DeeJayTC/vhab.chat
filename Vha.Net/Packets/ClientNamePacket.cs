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

namespace Vha.Net.Packets
{
    internal class ClientNamePacket : Packet
    {
        internal ClientNamePacket(Packet.Type type, byte[] data) : base(type, data) { }

        override protected void BytesToData(byte[] data)
        {
            if (data == null || data.Length < 6) { return; }

            int offset = 0;
            this.AddData(PopUnsignedInteger(ref data, ref offset));
            this.AddData(PopString(ref data, ref offset).ToString());
            if ((UInt32)this.Data[0] == UInt32.MaxValue)
                this.Data[0] = UInt32.MinValue;
        }

        internal UInt32 CharacterID { get { return (UInt32)this.Data[0]; } }
        internal String CharacterName { get { return (String)this.Data[1]; } }
    }
}
