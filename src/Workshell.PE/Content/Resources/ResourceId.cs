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
using System.Text;

namespace Workshell.PE.Resources
{
    public struct ResourceId : IEquatable<ResourceId>
    {
        public ResourceId(uint resourceId)
        {
            Id = resourceId;
            Name = resourceId.ToString();
        }

        public ResourceId(string resourceName)
        {
            Id = 0;
            Name = resourceName;
        }

        public ResourceId(uint resourceId, string resourceName)
        {
            Id = resourceId;
            Name = resourceName;
        }

        #region Operators

        public static implicit operator ResourceId(short resourceId)
        {
            return new ResourceId(Convert.ToUInt32(resourceId));
        }

        public static implicit operator ResourceId(int resourceId)
        {
            return new ResourceId(Convert.ToUInt32(resourceId));
        }

        public static implicit operator ResourceId(long resourceId)
        {
            return new ResourceId(Convert.ToUInt32(resourceId));
        }

        public static implicit operator ResourceId(ushort resourceId)
        {
            return new ResourceId(resourceId);
        }

        public static implicit operator ResourceId(uint resourceId)
        {
            return new ResourceId(resourceId);
        }

        public static implicit operator ResourceId(ulong resourceId)
        {
            return new ResourceId(Convert.ToUInt32(resourceId));
        }

        public static implicit operator ResourceId(string resourceName)
        {
            return new ResourceId(resourceName);
        }

        public static implicit operator short(ResourceId resourceId)
        {
            return Convert.ToInt16(resourceId.Id);
        }

        public static implicit operator int(ResourceId resourceId)
        {
            return Convert.ToInt32(resourceId.Id);
        }

        public static implicit operator long(ResourceId resourceId)
        {
            return resourceId.Id;
        }

        public static implicit operator ushort(ResourceId resourceId)
        {
            return Convert.ToUInt16(resourceId.Id);
        }

        public static implicit operator uint(ResourceId resourceId)
        {
            return resourceId.Id;
        }

        public static implicit operator ulong(ResourceId resourceId)
        {
            return resourceId.Id;
        }

        public static implicit operator string(ResourceId resourceId)
        {
            return (resourceId.Id > 0 ? resourceId.Id.ToString() : resourceId.Name);
        }

        public static bool operator ==(ResourceId firstId, ResourceId secondId)
        {
            return firstId.Equals(secondId);
        }

        public static bool operator !=(ResourceId firstId, ResourceId secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(ResourceId firstId, short secondId)
        {
            return (firstId.Id == Convert.ToUInt32(secondId));
        }

        public static bool operator !=(ResourceId firstId, short secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(ResourceId firstId, ushort secondId)
        {
            return (firstId.Id == Convert.ToUInt32(secondId));
        }

        public static bool operator !=(ResourceId firstId, ushort secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(ResourceId firstId, int secondId)
        {
            return (firstId.Id == Convert.ToUInt32(secondId));
        }

        public static bool operator !=(ResourceId firstId, int secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(ResourceId firstId, uint secondId)
        {
            return (firstId.Id == secondId);
        }

        public static bool operator !=(ResourceId firstId, uint secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(ResourceId firstId, long secondId)
        {
            return (firstId.Id == Convert.ToUInt32(secondId));
        }

        public static bool operator !=(ResourceId firstId, long secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(ResourceId firstId, ulong secondId)
        {
            return (firstId.Id == Convert.ToUInt32(secondId));
        }

        public static bool operator !=(ResourceId firstId, ulong secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(ResourceId firstId, string secondId)
        {
            return (String.Compare(firstId.Name, secondId, true) == 0);
        }

        public static bool operator !=(ResourceId firstId, string secondId)
        {
            return !(firstId == secondId);
        }

        public static bool operator ==(string firstId, ResourceId secondId)
        {
            return (string.Compare(firstId, secondId.Name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public static bool operator !=(string firstId, ResourceId secondId)
        {
            return !(firstId == secondId);
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            if (IsEmpty)
                return "(Empty)";
            
            return Name;
        }

        public override int GetHashCode()
        {
            var hash = 13;

            hash = (hash * 7) + Id.GetHashCode();
            hash = (hash * 7) + Name.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;

            if (!(obj is ResourceId))
                return false;

            if (ReferenceEquals(obj, this))
                return true;

            return Equals((ResourceId)obj);
        }

        public bool Equals(ResourceId resourceId)
        {
            return (Id == resourceId.Id && string.Compare(Name, resourceId.Name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        #endregion

        #region Properties

        public uint Id { get; }
        public string Name { get; }
        public bool IsNumeric => (Id != 0);
        public bool IsEmpty => (Id == 0 && string.IsNullOrWhiteSpace(Name));

        #endregion
    }
}
