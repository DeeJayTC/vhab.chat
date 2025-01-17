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
    [XmlRoot("character")]
    public class Character
    {
        [XmlElement("name")]
        public CharacterName Name;
        [XmlElement("basic_stats")]
        public CharacterStats Stats;
        [XmlElement("pictureurl")]
        public string PictureUrl;
        [XmlElement("smallpictureurl")]
        public string SmallPictureUrl;
        [XmlElement("organization_membership")]
        public CharacterOrganization Organization;
        [XmlElement("last_updated")]
        public string LastUpdated;

        [XmlIgnore]
        public bool Valid
        {
            get
            {
                if (this.Name == null)
                    return false;
                if (String.IsNullOrEmpty(this.Name.Nickname))
                    return false;
                if (this.Stats == null)
                    return false;
                if (this.Stats.Level == 0)
                    return false;
                if (String.IsNullOrEmpty(this.Stats.Profession))
                    return false;
                return true;
            }
        }

        [XmlIgnore]
        public bool InOrganization
        {
            get
            {
                if (this.Organization == null)
                    return false;
                if (String.IsNullOrEmpty(this.Organization.Name))
                    return false;
                return true;
            }
        }

        public override string ToString()
        {
            if (this.Name != null)
                return this.Name.ToString();
            return "";
        }
    }
}
