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

using System;

namespace Vha.AOML.DOM
{
    /// <summary>
    /// An element that describes the color of its child elements.
    /// Commonly known as FONT.
    /// </summary>
    public class ColorElement : Element
    {
        /// <summary>
        /// Returns the color of this (and child) elements in HTML color format
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Initializes a new instance of ColorElement
        /// </summary>
        /// <param name="color">The color of this element</param>
        public ColorElement(Color color)
            : base(ElementType.Color, true)
        {
            if (color == null) { throw new ArgumentNullException(); }
            this.Color = color;
        }

        /// <summary>
        /// Creates a clone of this ColorElement and its children
        /// </summary>
        /// <returns>A new ColorElement</returns>
        public override Element Clone()
        {
            Element clone = new ColorElement(this.Color);
            foreach (Element child in this.Children)
            {
                clone.Children.Add(child.Clone());
            }
            return clone;
        }
    }
}
