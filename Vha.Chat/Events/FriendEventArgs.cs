﻿/*
* Vha.Chat
* Copyright (C) 2009-2010 Remco van Oosterhout
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
using System.Collections.Generic;
using System.Text;
using Vha.Chat;
using Vha.Net;

namespace Vha.Chat.Events
{
    public class FriendEventArgs
    {
        /// <summary>
        /// The character who triggered this event
        /// </summary>
        public readonly Friend Friend;
        /// <summary>
        /// The character's state before this event occured
        /// </summary>
        public readonly Friend PreviousFriend;
        /// <summary>
        /// Whether this friend has just been added
        /// </summary>
        public readonly bool Added;

        public FriendEventArgs(Friend friend, Friend previousFriend, bool added)
        {
            this.Friend = friend;
            this.PreviousFriend = previousFriend;
            this.Added = added;
        }
    }
}
