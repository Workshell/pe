using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Messages
{
    public sealed class MessageTable : IEnumerable<MessageTableBlock>
    {
        private readonly MessageTableBlock[] _blocks;

        internal MessageTable(MessageTableResource resource, uint languageId, MessageTableBlock[] blocks)
        {
            _blocks = blocks;

            Resource = resource;
            Language = languageId;
            Count = _blocks.Length;
        }

        #region Methods

        public IEnumerator<MessageTableBlock> GetEnumerator()
        {
            foreach (var block in _blocks)
                yield return block;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public MessageTableResource Resource { get; }
        public ResourceLanguage Language { get; }

        public int Count { get; }
        public MessageTableBlock this[int index] => _blocks[index];

        #endregion
    }
}
