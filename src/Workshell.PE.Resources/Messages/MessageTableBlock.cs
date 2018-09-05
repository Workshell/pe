using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Messages
{
    public sealed class MessageTableBlock : IEnumerable<MessageTableEntry>
    {
        private readonly MessageTableEntry[] _entries;

        internal MessageTableBlock(uint lowId, uint highId, uint offsetToEntries, MessageTableEntry[] entries)
        {
            _entries = entries;

            LowId = lowId;
            HighId = highId;
            OffsetToEntries = offsetToEntries;
            Count = _entries.Length;
        }

        #region Methods

        public IEnumerator<MessageTableEntry> GetEnumerator()
        {
            foreach (var entry in _entries)
                yield return entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public uint LowId { get; }
        public uint HighId { get; }
        public uint OffsetToEntries { get; }

        public int Count { get; }
        public MessageTableEntry this[int index] => _entries[index];

        #endregion
    }
}
