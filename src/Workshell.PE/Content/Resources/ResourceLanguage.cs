#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Workshell.PE.Resources
{
    public partial struct ResourceLanguage : IEquatable<ResourceLanguage>
    {
        public ResourceLanguage(int lcid)
        {
            LCID = lcid;
        }

        public ResourceLanguage(uint lcid) : this(Convert.ToInt32(lcid))
        {
        }

        #region Operators

        public static implicit operator ResourceLanguage(int lcid)
        {
            return new ResourceLanguage(Convert.ToUInt32(lcid));
        }

        public static implicit operator ResourceLanguage(long lcid)
        {
            return new ResourceLanguage(Convert.ToUInt32(lcid));
        }

        public static implicit operator ResourceLanguage(uint lcid)
        {
            return new ResourceLanguage(lcid);
        }

        public static implicit operator ResourceLanguage(ulong lcid)
        {
            return new ResourceLanguage(Convert.ToUInt32(lcid));
        }

        public static implicit operator int(ResourceLanguage identifer)
        {
            return identifer.LCID;
        }

        public static implicit operator long(ResourceLanguage identifier)
        {
            return identifier.LCID;
        }

        public static implicit operator uint(ResourceLanguage identifer)
        {
            return Convert.ToUInt32(identifer.LCID);
        }

        public static implicit operator ulong(ResourceLanguage identifier)
        {
            return Convert.ToUInt64(identifier.LCID);
        }

        public static bool operator ==(ResourceLanguage firstId, ResourceLanguage secondId)
        {
            return firstId.Equals(secondId);
        }

        public static bool operator !=(ResourceLanguage firstId, ResourceLanguage secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(ResourceLanguage firstId, int secondId)
        {
            return (firstId.LCID == Convert.ToUInt32(secondId));
        }

        public static bool operator !=(ResourceLanguage firstId, int secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(ResourceLanguage firstId, uint secondId)
        {
            return (firstId.LCID == Convert.ToInt32(secondId));
        }

        public static bool operator !=(ResourceLanguage firstId, uint secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(ResourceLanguage firstId, long secondId)
        {
            return (firstId.LCID == Convert.ToInt32(secondId));
        }

        public static bool operator !=(ResourceLanguage firstId, long secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(ResourceLanguage firstId, ulong secondId)
        {
            return (firstId.LCID == Convert.ToInt32(secondId));
        }

        public static bool operator !=(ResourceLanguage firstId, ulong secondId)
        {
            return !(firstId == secondId);
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"0x{LCID:X4} ({LCID}) | {Culture.EnglishName}";
        }

        public override int GetHashCode()
        {
            var hash = 13;

            hash = (hash * 7) + LCID.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (!(obj is ResourceLanguage))
                return false;

            //if (ReferenceEquals(obj, this))
            //    return true;

            return Equals((ResourceLanguage)obj);
        }

        public bool Equals(ResourceLanguage identifier)
        {
            return (LCID == identifier.LCID);
        }

        #endregion

        #region Properties

        public int LCID { get; }
        public CultureInfo Culture => CultureInfo.GetCultureInfo(LCID);

        #endregion
    }
}
