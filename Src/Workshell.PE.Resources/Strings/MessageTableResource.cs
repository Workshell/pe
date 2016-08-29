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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Workshell.PE.Extensions;
using Workshell.PE.Native;

using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources
{

    public sealed class MessageTableEntry
    {

        internal MessageTableEntry(uint messageId, string message, bool isUnicode)
        {
            Id = messageId;
            Message = message;
            IsUnicode = isUnicode;
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("0x{0:X8}: {1}", Id, Message);
        }

        #endregion

        #region Properties

        public uint Id
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

        public bool IsUnicode
        {
            get;
            private set;
        }

        #endregion

    }

    public sealed class MessageTableBlock : IEnumerable<MessageTableEntry>
    {

        private uint low_id;
        private uint high_id;
        private uint offset;
        private MessageTableEntry[] entries;

        internal MessageTableBlock(uint lowId, uint highId, uint offsetToEntries, MessageTableEntry[] tableEntries)
        {
            low_id = lowId;
            high_id = highId;
            offset = offsetToEntries;
            entries = tableEntries;
        }

        #region Methods

        public IEnumerator<MessageTableEntry> GetEnumerator()
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

        public uint LowId
        {
            get
            {
                return low_id;
            }
        }

        public uint HighId
        {
            get
            {
                return high_id;
            }
        }

        public uint OffsetToEntries
        {
            get
            {
                return offset;
            }
        }

        public int Count
        {
            get
            {
                return entries.Length;
            }
        }

        public MessageTableEntry this[int index]
        {
            get
            {
                return entries[index];
            }
        }

        #endregion

    }

    public sealed class MessageTable : IEnumerable<MessageTableBlock>
    {

        private MessageTableResource resource;
        private uint language_id;
        private MessageTableBlock[] blocks;

        internal MessageTable(MessageTableResource tableResource, uint languageId, MessageTableBlock[] tableBlocks)
        {
            resource = tableResource;
            language_id = languageId;
            blocks = tableBlocks;
        }

        #region Methods

        public IEnumerator<MessageTableBlock> GetEnumerator()
        {
            for (var i = 0; i < blocks.Length; i++)
                yield return blocks[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public MessageTableResource Resource
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
                return blocks.Length;
            }
        }

        public MessageTableBlock this[int index]
        {
            get
            {
                return blocks[index];
            }
        }

        #endregion

    }

    public sealed class MessageTableResource : Resource
    {

        public MessageTableResource(ResourceType owningType, ResourceDirectoryEntry directoryEntry) : base(owningType, directoryEntry)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            ResourceId resource_type = new ResourceId(ResourceType.RT_MESSAGETABLE);

            return ResourceType.Register(resource_type, typeof(MessageTableResource));
        }

        #endregion

        #region Methods

        public MessageTable ToTable()
        {
            return ToTable(DEFAULT_LANGUAGE);
        }

        public MessageTable ToTable(uint languageId)
        {
            byte[] data = GetBytes(languageId);

            using (MemoryStream mem = new MemoryStream(data))
            {
                List<Tuple<uint, uint, uint>> tuples = new List<Tuple<uint, uint, uint>>();
                MESSAGE_RESOURCE_DATA resource_data = Utils.Read<MESSAGE_RESOURCE_DATA>(mem);

                for(var i = 0; i < resource_data.NumberOfBlocks; i++)
                {
                    MESSAGE_RESOURCE_BLOCK block = Utils.Read<MESSAGE_RESOURCE_BLOCK>(mem);
                    Tuple<uint, uint, uint> tuple = new Tuple<uint, uint, uint>(block.OffsetToEntries, block.LowId, block.HighId);

                    tuples.Add(tuple);
                }

                MessageTableBlock[] blocks = new MessageTableBlock[resource_data.NumberOfBlocks];

                for(var i = 0; i < tuples.Count; i++)
                {
                    Tuple<uint, uint, uint> tuple = tuples[i];

                    mem.Seek(tuple.Item1, SeekOrigin.Begin);

                    int count = (tuple.Item3 - tuple.Item2).ToInt32() + 1;
                    MessageTableEntry[] entries = new MessageTableEntry[count];
                    
                    uint id = tuple.Item2;

                    for (var j = 0; j < count; j++)
                    {
                        MESSAGE_RESOURCE_ENTRY resource_entry = Utils.Read<MESSAGE_RESOURCE_ENTRY>(mem);
                        byte[] buffer = Utils.ReadBytes(mem, resource_entry.Length - sizeof(uint));
                        string message = (resource_entry.Flags == 0 ? Encoding.ASCII.GetString(buffer) : Encoding.Unicode.GetString(buffer));
                        MessageTableEntry entry = new MessageTableEntry(id, message, resource_entry.Flags != 0);

                        entries[j] = entry;
                        id++;
                    }

                    MessageTableBlock block = new MessageTableBlock(tuple.Item2, tuple.Item3, tuple.Item1, entries);

                    blocks[i] = block;
                }

                MessageTable table = new MessageTable(this, languageId, blocks);

                return table;
            }
        }

        #endregion

    }

}
