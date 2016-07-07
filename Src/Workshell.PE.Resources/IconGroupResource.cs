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
using Workshell.PE.Native;

namespace Workshell.PE
{

    public enum IconGroupSaveFormat
    {
        Raw,
        Resource,
        Icon
    }

    public sealed class IconGroupResourceEntry
    {

        internal IconGroupResourceEntry(ICON_RESDIR resDir)
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

    public sealed class IconGroupResource : IEnumerable<IconGroupResourceEntry>
    {

        private Resource resource;
        private uint language_id;
        private IconGroupResourceEntry[] entries;

        internal IconGroupResource(Resource sourceResource, uint languageId, IconGroupResourceEntry[] groupEntries)
        {
            resource = sourceResource;
            language_id = languageId;
            entries = groupEntries;
        }

        #region Static Methods

        public static IconGroupResource Load(Resource resource)
        {
            return Load(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static IconGroupResource Load(Resource resource, uint language)
        {
            if (!resource.Languages.Contains(language))
                return null;

            byte[] data = resource.GetBytes(language);
            MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream(data);

            using (mem)
            {
                NEWHEADER header = Utils.Read<NEWHEADER>(mem);

                if (header.ResType != 1)
                    throw new Exception("Not an icon group resource.");

                IconGroupResourceEntry[] entries = new IconGroupResourceEntry[header.ResCount];

                for (var i = 0; i < header.ResCount; i++)
                {
                    ICON_RESDIR icon = Utils.Read<ICON_RESDIR>(mem);
                    IconGroupResourceEntry entry = new IconGroupResourceEntry(icon);

                    entries[i] = entry;
                }

                IconGroupResource group = new IconGroupResource(resource, language, entries);

                return group;
            }
        }

        #endregion

        #region Methods

        public IEnumerator<IconGroupResourceEntry> GetEnumerator()
        {
            for (var i = 0; i < entries.Length; i++)
                yield return entries[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Save(string fileName)
        {
            Save(fileName, IconGroupSaveFormat.Icon);
        }

        public void Save(Stream stream)
        {
            Save(stream, IconGroupSaveFormat.Icon);
        }

        public void Save(string fileName, IconGroupSaveFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, IconGroupSaveFormat format)
        {
            switch (format)
            {
                case IconGroupSaveFormat.Raw:
                    SaveRaw(stream);
                    break;
                case IconGroupSaveFormat.Resource:
                    SaveResource(stream);
                    break;
                case IconGroupSaveFormat.Icon:
                    SaveIcon(stream);
                    break;
            }
        }

        private void SaveRaw(Stream stream)
        {
            byte[] data = resource.GetBytes(language_id);

            stream.Write(data, 0, data.Length);
        }

        private void SaveResource(Stream stream)
        {
            throw new NotImplementedException();
        }

        private void SaveIcon(Stream stream)
        {
            uint[] offsets = new uint[entries.Length];
            uint offset = Convert.ToUInt32(6 + (16 * offsets.Length));

            for(var i = 0; i < entries.Length; i++)
            {
                IconGroupResourceEntry entry = entries[i];
                offsets[i] = offset;

                offset += entry.BytesInRes;
            }

            Utils.Write(Convert.ToUInt16(0), stream);
            Utils.Write(Convert.ToUInt16(1), stream);
            Utils.Write(Convert.ToUInt16(entries.Length), stream);

            for (var i = 0; i < entries.Length; i++)
            {
                IconGroupResourceEntry entry = entries[i];

                Utils.Write(Convert.ToByte(entry.Width >= 256 ? 0 : entry.Width), stream);
                Utils.Write(Convert.ToByte(entry.Height >= 256 ? 0 : entry.Height), stream);
                Utils.Write(Convert.ToByte(entry.ColorCount), stream);
                Utils.Write(Convert.ToByte(0), stream);
                Utils.Write(Convert.ToUInt16(1), stream);

                ushort colors = Convert.ToUInt16(Math.Log(entry.ColorCount) / Math.Log(2));

                if (colors >= 256)
                    colors = 0;

                Utils.Write(colors, stream);
                Utils.Write(entry.BytesInRes, stream);
                Utils.Write(offsets[i], stream);
            }

            ResourceType icon_type = resource.Type.Resources.First(t => t.Id == ResourceType.RT_ICON);

            for (var i = 0; i < entries.Length; i++)
            {
                IconGroupResourceEntry entry = entries[i];
                Resource resource = icon_type.First(r => r.Id == entry.IconId);
                byte[] data = resource.GetBytes(language_id);

                Utils.Write(data, stream);                
            }
        }

        #endregion

        #region Properties

        public Resource Resource
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

        public IconGroupResourceEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        #endregion

    }

}
