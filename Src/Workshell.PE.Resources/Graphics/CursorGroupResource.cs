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

    public enum CursorGroupSaveFormat
    {
        Raw,
        Resource,
        Cursor
    }

    public sealed class CursorGroupResourceEntry
    {

        internal CursorGroupResourceEntry(CURSOR_RESDIR resDir)
        {
            Width = resDir.Cursor.Width;
            Height = resDir.Cursor.Height;
            Planes = resDir.Planes;
            BitCount = resDir.BitCount;
            BytesInRes = resDir.BytesInRes;
            CursorId = resDir.CursorId;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("{0}x{1} {2}-bit, ID: {3}", Width, Height, BitCount, CursorId);
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

        public ushort CursorId
        {
            get;
            private set;
        }

        #endregion

    }

    public sealed class CursorGroupResource : IEnumerable<CursorGroupResourceEntry>
    {

        private Resource resource;
        private uint language_id;
        private CursorGroupResourceEntry[] entries;

        internal CursorGroupResource(Resource sourceResource, uint languageId, CursorGroupResourceEntry[] groupEntries)
        {
            resource = sourceResource;
            language_id = languageId;
            entries = groupEntries;
        }

        #region Static Methods

        public static CursorGroupResource Load(Resource resource)
        {
            return Load(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static CursorGroupResource Load(Resource resource, uint language)
        {
            if (!resource.Languages.Contains(language))
                return null;

            if (resource.Type.Id != ResourceType.RT_GROUP_CURSOR)
                return null;

            byte[] data = resource.GetBytes(language);
            MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream(data);

            using (mem)
            {
                NEWHEADER header = Utils.Read<NEWHEADER>(mem);

                if (header.ResType != 2)
                    throw new Exception("Not a cursor group resource.");

                CursorGroupResourceEntry[] entries = new CursorGroupResourceEntry[header.ResCount];

                for (var i = 0; i < header.ResCount; i++)
                {
                    CURSOR_RESDIR cursor = Utils.Read<CURSOR_RESDIR>(mem);
                    CursorGroupResourceEntry entry = new CursorGroupResourceEntry(cursor);

                    entries[i] = entry;
                }

                CursorGroupResource group = new CursorGroupResource(resource, language, entries);

                return group;
            }
        }

        #endregion

        #region Methods

        public IEnumerator<CursorGroupResourceEntry> GetEnumerator()
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
            Save(fileName, CursorSaveFormat.Cursor);
        }

        public void Save(Stream stream)
        {
            Save(stream, CursorSaveFormat.Cursor);
        }

        public void Save(string fileName, CursorSaveFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, CursorSaveFormat format)
        {
            switch (format)
            {
                case CursorSaveFormat.Raw:
                    SaveRaw(stream);
                    break;
                case CursorSaveFormat.Resource:
                    SaveResource(stream);
                    break;
                case CursorSaveFormat.Cursor:
                    SaveCursor(stream);
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

        private void SaveCursor(Stream stream)
        {
            List<Tuple<ushort, ushort, ushort, byte[]>> cursors = new List<Tuple<ushort, ushort, ushort, byte[]>>(entries.Length);

            for (var i = 0; i < entries.Length; i++)
            {
                CursorGroupResourceEntry entry = entries[i];
                ResourceType res_type = resource.Type.Resources.First(t => t.Id == ResourceType.RT_CURSOR);
                Resource res = res_type.First(c => c.Id == entry.CursorId);
                byte[] data = res.GetBytes(language_id);

                ushort hotspot_x = 0;
                ushort hotspot_y = 0;
                byte[] dib = new byte[0];

                using (MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream(data))
                {
                    hotspot_x = Utils.ReadUInt16(mem);
                    hotspot_y = Utils.ReadUInt16(mem);

                    using (MemoryStream dib_mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream())
                    {
                        mem.CopyTo(dib_mem, 4096);

                        dib = dib_mem.ToArray();
                    }
                }

                Tuple<ushort, ushort, ushort, byte[]> tuple = new Tuple<ushort, ushort, ushort, byte[]>(entry.CursorId, hotspot_x, hotspot_y, dib);

                cursors.Add(tuple);
            }

            uint[] offsets = new uint[entries.Length];
            uint offset = Convert.ToUInt32(6 + (16 * offsets.Length));

            for (var i = 0; i < entries.Length; i++)
            {
                CursorGroupResourceEntry entry = entries[i];
                Tuple<ushort, ushort, ushort, byte[]> tuple = cursors[i];

                offsets[i] = offset;
                offset += tuple.Item4.Length.ToUInt32();
            }

            Utils.Write(Convert.ToUInt16(0), stream);
            Utils.Write(Convert.ToUInt16(2), stream);
            Utils.Write(Convert.ToUInt16(entries.Length), stream);

            for (var i = 0; i < entries.Length; i++)
            {
                CursorGroupResourceEntry entry = entries[i];
                Tuple<ushort, ushort, ushort, byte[]> tuple = cursors[i];

                Utils.Write(Convert.ToByte(entry.Width), stream);
                Utils.Write(Convert.ToByte(entry.Height), stream);
                Utils.Write(Convert.ToByte(entry.BitCount), stream);
                Utils.Write(Convert.ToByte(0), stream);
                Utils.Write(tuple.Item2, stream);
                Utils.Write(tuple.Item3, stream);
                Utils.Write(tuple.Item4.Length, stream);
                Utils.Write(offsets[i], stream);
            }

            for (var i = 0; i < entries.Length; i++)
            {
                CursorGroupResourceEntry entry = entries[i];
                Tuple<ushort, ushort, ushort, byte[]> tuple = cursors[i];

                stream.Write(tuple.Item4, 0, tuple.Item4.Length);
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

        public CursorGroupResourceEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        #endregion

    }

}
