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

namespace Vha.Chat.Commands
{
    public class TextCommand : Command
    {
        public override bool Process(Context context, string trigger, string message, string[] args)
        {
            if (!context.Input.CheckArguments(trigger, args.Length, 1, true)) return false;
            context.Write(MessageClass.Internal, message);
            return true;
        }

        public TextCommand()
            : base(
                "Print text", // Name
                new string[] { "text", "echo" }, // Triggers
                new string[] { "text [text]", "echo [text]" }, // Usage
                new string[] { "text Ping? Pong!" }, // Examples
                // Description
                "The text command allows you to print text directly to the output window.\n" +
                "These messages will only appear locally and will not be sent accross the chat server."
            )
        { }
    }
}
