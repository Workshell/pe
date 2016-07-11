#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workshell.PE.Resources
{

    public struct ResourceId : IEquatable<ResourceId>
    {

        private readonly uint id;
        private readonly string name;

        public ResourceId(uint resourceId)
        {
            id = resourceId;
            name = id.ToString();
        }

        public ResourceId(string resourceName)
        {
            id = 0;
            name = resourceName;
        }

        public ResourceId(uint resourceId, string resourceName)
        {
            id = resourceId;
            name = resourceName;
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
            if (resourceId.Id > 0)
            {
                return resourceId.Id.ToString();
            }
            else
            {
                return resourceId.Name;
            }
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
            return (String.Compare(firstId, secondId.Name, true) == 0);
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
            {
                return "(Empty)";
            }
            else
            {
                return name;
            }
        }

        public override int GetHashCode()
        {
            int hash = 13;

            hash = (hash * 7) + id.GetHashCode();
            hash = (hash * 7) + name.GetHashCode();

            return hash;
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null))
                return false;

            if (!(obj is ResourceId))
                return false;

            if (Object.ReferenceEquals(obj, this))
                return true;

            return Equals((ResourceId)obj);
        }

        public bool Equals(ResourceId resourceId)
        {
            return (id == resourceId.Id && String.Compare(name, resourceId.Name, true) == 0);
        }

        #endregion

        #region Properties

        public uint Id
        {
            get
            {
                return id;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public bool IsNumeric
        {
            get
            {
                return (id != 0);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (id == 0 && String.IsNullOrWhiteSpace(name));
            }
        }

        #endregion

    }

}
