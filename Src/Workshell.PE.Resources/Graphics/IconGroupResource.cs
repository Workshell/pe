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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources
{

    public sealed class IconGroupEntry
    {

        internal IconGroupEntry(ICON_RESDIR resDir)
        {
            Width = resDir.Icon.Width;
            Height = resDir.Icon.Height;
            ColorCount = resDir.Icon.ColorCount;
            Planes = resDir.Planes;
            BitCount = resDir.BitCount;
            BytesInRes = resDir.BytesInRes;
            IconId = resDir.IconId;

            if (Width == 0)
                Width = 256;

            if (Height == 0)
                Height = 256;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("{0}x{1} {2}-bit, ID: {3}", Width, Height, BitCount, IconId);
        }

        #endregion

        #region Properties

        public ushort Width
        {
            get;
            private set;
        }

        public ushort Height
        {
            get;
            private set;
        }

        public byte ColorCount
        {
            get;
            private set;
        }

        public ushort Planes
        {
            get;
            private set;
        }

        public ushort BitCount
        {
            get;
            private set;
        }

        public uint BytesInRes
        {
            get;
            private set;
        }

        public ushort IconId
        {
            get;
            private set;
        }

        #endregion

    }

    public sealed class IconGroup : IEnumerable<IconGroupEntry>
    {

        private IconGroupResource resource;
        private uint language_id;
        private IconGroupEntry[] entries;

        internal IconGroup(IconGroupResource groupResource, uint languageId, IconGroupEntry[] groupEntries)
        {
            resource = groupResource;
            language_id = languageId;
            entries = groupEntries;
        }

        #region Methods

        public IEnumerator<IconGroupEntry> GetEnumerator()
        {
            for (var i = 0; i < entries.Length; i++)
                yield return entries[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public IconGroupResource Resource
        {
            get
            {
                return resource;
            }
        }

        public uint Language
        {
            get
            {
                return language_id;
            }
        }

        public int Count
        {
            get
            {
                return entries.Length;
            }
        }

        public IconGroupEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        #endregion

    }

    public enum IconGroupSaveFormat
    {
        Raw,
        Icon
    }

    public sealed class IconGroupResource : Resource
    {

        public IconGroupResource(ResourceType owningType, ResourceDirectoryEntry directoryEntry) : base(owningType, directoryEntry)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            ResourceId resource_type = new ResourceId(ResourceType.RT_GROUP_ICON);

            return ResourceType.Register(resource_type, typeof(IconGroupResource));
        }

        #endregion

        #region Methods

        public IconGroup ToGroup()
        {
            return ToGroup(DEFAULT_LANGUAGE);
        }

        public IconGroup ToGroup(uint languageId)
        {
            byte[] data = GetBytes(languageId);

            using (MemoryStream mem = new MemoryStream(data))
            {
                NEWHEADER header = Utils.Read<NEWHEADER>(mem);

                if (header.ResType != 1)
                    throw new Exception("Not an icon group resource.");

                IconGroupEntry[] entries = new IconGroupEntry[header.ResCount];

                for (var i = 0; i < header.ResCount; i++)
                {
                    ICON_RESDIR icon = Utils.Read<ICON_RESDIR>(mem);
                    IconGroupEntry entry = new IconGroupEntry(icon);

                    entries[i] = entry;
                }

                IconGroup group = new IconGroup(this, languageId, entries);

                return group;
            }
        }

        public void Save(string fileName, uint languageId, IconGroupSaveFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, languageId, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, uint languageId, IconGroupSaveFormat format)
        {
            if (format == IconGroupSaveFormat.Raw)
            {
                Save(stream, languageId);
            }
            else
            {
                IconGroup group = ToGroup(languageId);
                uint[] offsets = new uint[group.Count];
                uint offset = Convert.ToUInt32(6 + (16 * offsets.Length));

                for (var i = 0; i < group.Count; i++)
                {
                    IconGroupEntry entry = group[i];
                    offsets[i] = offset;

                    offset += entry.BytesInRes;
                }

                Utils.Write(Convert.ToUInt16(0), stream);
                Utils.Write(Convert.ToUInt16(1), stream);
                Utils.Write(Convert.ToUInt16(group.Count), stream);

                for (var i = 0; i < group.Count; i++)
                {
                    IconGroupEntry entry = group[i];

                    ulong color_count = Convert.ToUInt64(Math.Pow(2, entry.BitCount));

                    if (color_count >= 256)
                        color_count = 0;

                    Utils.Write(Convert.ToByte(entry.Width >= 256 ? 0 : entry.Width), stream);
                    Utils.Write(Convert.ToByte(entry.Height >= 256 ? 0 : entry.Height), stream);
                    Utils.Write(Convert.ToByte(color_count), stream);
                    Utils.Write(Convert.ToByte(0), stream);
                    Utils.Write(Convert.ToUInt16(1), stream);
                    Utils.Write(entry.BitCount, stream);
                    Utils.Write(entry.BytesInRes, stream);
                    Utils.Write(offsets[i], stream);
                }

                ResourceType icon_type = Type.Resources.First(t => t.Id == ResourceType.RT_ICON);

                for (var i = 0; i < group.Count; i++)
                {
                    IconGroupEntry entry = group[i];
                    Resource resource = icon_type.First(r => r.Id == entry.IconId);
                    byte[] data = resource.GetBytes(languageId);

                    Utils.Write(data, stream);
                }
            }
        }

        #endregion

    }

}
