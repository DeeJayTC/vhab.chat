/*
* Vha.AOML
* Copyright (C) 2010-2011 Remco van Oosterhout
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

namespace Vha.AOML.DOM
{
    /// <summary>
    /// Identifies the type of a link
    /// </summary>
    public enum LinkType
    {
        Command,
        Window,
        Item,
        Entity,
        User,
        Other,
        Invalid
    }

    /// <summary>
    /// Base class for all links
    /// </summary>
    public abstract class Link
    {
        /// <summary>
        /// Returns the type of this link
        /// </summary>
        public LinkType Type { get; private set; }

        public abstract Link Clone();

        #region internal
        internal Link(LinkType type)
        {
            this.Type = type;
        }
        #endregion
    }
}
