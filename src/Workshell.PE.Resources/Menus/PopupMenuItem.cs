using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Menus
{
    public sealed class PopupMenuItem : MenuItem, IEnumerable<MenuItem>
    {
        private readonly MenuItem[] _items;

        internal PopupMenuItem(ushort id, string text, ushort flags, MenuItem[] items) : base(id, text, flags)
        {
            _items = items;

            Count = _items.Length;
        }

        #region Methods

        public IEnumerator<MenuItem> GetEnumerator()
        {
            foreach (var item in _items)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count { get; }
        public MenuItem this[int index] => _items[index];

        #endregion
    }
}
