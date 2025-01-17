﻿/*
* Vha.Common
* Copyright (C) 2005-2010 Remco van Oosterhout
* See Credits.txt for all aknowledgements.
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using System;
using System.Xml.Serialization;

namespace Vha.Common.Data.AO
{
    public class CharacterOrganization
    {
        [XmlElement("organization_id", Type = typeof(Int32))]
        public Int32 ID = 0;
        [XmlElement("organization_name")]
        public string Name;
        [XmlElement("rank")]
        public string Rank;
        [XmlElement("rank_id", Type = typeof(Int32))]
        public Int32 RankID = 0;

        public override string ToString()
        {
            // Replace values with N/A if they're null or empty.
            return String.Format("{0} of {1}", 
                (String.IsNullOrEmpty(this.Rank) ? "N/A" : this.Rank),
                (String.IsNullOrEmpty(this.Name) ? "N/A" : this.Name));
        }
    }
}
