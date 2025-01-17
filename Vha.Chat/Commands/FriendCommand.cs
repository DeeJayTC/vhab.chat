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
using System.Text;
using Vha.Net;
using Vha.Common;

namespace Vha.Chat.Commands
{
    public class FriendCommand : Command
    {
        public override bool Process(Context context, string trigger, string message, string[] args)
        {
            if (!context.Input.CheckArguments(trigger, args.Length, 2, true)) return false;
            string character = Format.UppercaseFirst(args[1]);
            if (args[0].ToLower() == "add")
            {
                if (!context.Input.CheckCharacter(character, true)) return false;
                Friend friend = context.GetFriend(character);
                if (friend != null && !friend.Temporary)
                {
                    context.Write(MessageClass.Error, character + " already is on your friends list");
                    return false;
                }
                context.Chat.SendFriendAdd(character);
                context.Write(MessageClass.Internal, "Adding '" + character + "' to your friends list");
                return true;
            }
            else if (args[0].ToLower() == "remove")
            {
                if (!context.Input.CheckCharacter(character, true)) return false;
                if (!context.HasFriend(character))
                {
                    context.Write(MessageClass.Error, character + " is not on your friends list");
                    return false;
                }
                context.Chat.SendFriendRemove(character);
                context.Write(MessageClass.Internal, "Removing '" + character + "' from your friends list");
                return true;
            }
            else
            {
                context.Write(MessageClass.Error, "Expecting either 'add' or 'remove' as first argument for this command");
                return false;
            }
        }

        public FriendCommand()
            : base(
                "Adding and removing friends", // Name
                new string[] { "friend" }, // Triggers
                new string[] { "friend add [character]", "friend remove [character]" }, // Usage
                new string[] { "friend add Vhab", "friend remove Helpbot" }, // Examples
                // Description
                "The friend commands allows you to add and remove characters from your friendslist.\n" +
                "Adding characters to your friendslist allows you to see whether they're currently online or offline."
            )
        { }
    }
}
