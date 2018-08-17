using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Menus
{
    public sealed class Menu : IEnumerable<MenuItem>
    {
        private readonly MenuItem[] _items;

        internal Menu(MenuResource resource, uint languageId, MenuItem[] items)
        {
            _items = items;

            Count = _items.Length;
            Resource = resource;
            Language = languageId;
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

        public MenuResource Resource { get; }
        public ResourceLanguage Language { get; }

        public int Count { get; }
        public MenuItem this[int index] => _items[index];

        #endregion
    }
}
