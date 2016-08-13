﻿#region License
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Resources
{

    public sealed class StringTableEntry
    {

        private ushort id;
        private string value;

        internal StringTableEntry(ushort stringId, string stringValue)
        {
            id = stringId;
            value = stringValue;
        }

        #region Methods

        public override string ToString()
        {
            if (id == 0)
            {
                return "(Empty)";
            }
            else
            {
                return String.Format("{0} = {1}", id, value);
            }
        }

        #endregion

        #region Properties

        public ushort Id
        {
            get
            {
                return id;
            }
        }

        public string Value
        {
            get
            {
                return value;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return (id == 0);
            }
        }

        #endregion

    }

    public sealed class StringTable : IEnumerable<StringTableEntry>
    {

        private StringTableResource resource;
        private uint language_id;
        private StringTableEntry[] strings;

        internal StringTable(StringTableResource tableResource, uint languageId, StringTableEntry[] tableEntries)
        {
            resource = tableResource;
            language_id = languageId;
            strings = tableEntries;
        }

        #region Methods

        public IEnumerator<StringTableEntry> GetEnumerator()
        {
            for (var i = 0; i < strings.Length; i++)
                yield return strings[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public StringTableResource Resource
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
                return strings.Length;
            }
        }

        public StringTableEntry this[int index]
        {
            get
            {
                return strings[index];
            }
        }

        #endregion

    }

    public sealed class StringTableResource : Resource
    {

        private Resource resource;
        private uint language_id;
        private StringTableEntry[] strings;

        public StringTableResource(ResourceType owningType, ResourceDirectoryEntry directoryEntry) : base(owningType, directoryEntry)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            ResourceId resource_type = new ResourceId(ResourceType.RT_STRING);

            return ResourceType.Register(resource_type, typeof(StringTableResource));
        }

        #endregion

        #region Methods

        public StringTable ToTable()
        {
            return ToTable(DEFAULT_LANGUAGE);
        }

        public StringTable ToTable(uint languageId)
        {
            byte[] data = GetBytes(languageId);

            List<string> list = new List<string>();

            using (MemoryStream mem = new MemoryStream(data))
            {
                while (mem.Position < mem.Length)
                {
                    ushort count = Utils.ReadUInt16(mem);

                    if (count == 0)
                    {
                        list.Add(null);
                    }
                    else
                    {
                        StringBuilder builder = new StringBuilder(count);

                        for (var i = 0; i < count; i++)
                        {
                            ushort value = Utils.ReadUInt16(mem);

                            builder.Append((char)value);
                        }

                        list.Add(builder.ToString());
                    }
                }
            }

            StringTableEntry[] strings = new StringTableEntry[list.Count];
            ushort base_id = Convert.ToUInt16((resource.Id - 1) << 4);

            for (var i = 0; i < list.Count; i++)
            {
                StringTableEntry entry;
                string value = list[i];

                if (value == null)
                {
                    entry = new StringTableEntry(0, value);
                }
                else
                {
                    ushort id = Convert.ToUInt16(base_id + i);

                    entry = new StringTableEntry(id, value);
                }

                strings[i] = entry;
            }

            StringTable table = new StringTable(this, languageId, strings);

            return table;
        }

        #endregion

    }

}
