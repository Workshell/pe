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

        private CursorGroupResourceEntry[] entries;

        internal CursorGroupResource(CursorGroupResourceEntry[] groupEntries)
        {
            entries = groupEntries;
        }

        #region Static Methods

        public static CursorGroupResource FromBytes(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                return FromStream(mem);
            }
        }

        public static CursorGroupResource FromStream(Stream stream)
        {
            NEWHEADER header = Utils.Read<NEWHEADER>(stream);

            if (header.ResType != 2)
                throw new Exception("Not a cursor group resource.");

            CursorGroupResourceEntry[] entries = new CursorGroupResourceEntry[header.ResCount];

            for(var i = 0; i < header.ResCount; i++)
            {
                CURSOR_RESDIR cursor = Utils.Read<CURSOR_RESDIR>(stream);
                CursorGroupResourceEntry entry = new CursorGroupResourceEntry(cursor);

                entries[i] = entry;
            }

            CursorGroupResource group = new CursorGroupResource(entries);

            return group;
        }

        public static CursorGroupResource FromResource(Resource resource)
        {
            return FromResource(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static CursorGroupResource FromResource(Resource resource, uint language)
        {
            byte[] data = resource.ToBytes(language);

            return FromBytes(data);
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

        #endregion

        #region Properties

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
