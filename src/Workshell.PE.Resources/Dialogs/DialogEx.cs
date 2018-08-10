using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Content;
using Workshell.PE.Extensions;
using Workshell.PE.Resources.Dialogs.Styles;

namespace Workshell.PE.Resources.Dialogs
{
    public sealed class DialogEx : DialogBase, IEnumerable<DialogItemEx>
    {
        private readonly DialogItemEx[] _items;

        internal DialogEx(DialogResource resource, uint language, DialogItemEx[] items) : base(resource, language, true)
        {
            _items = items;

            Count = items.Length;
        }

        #region Static Methods

        internal static async Task<DialogEx> CreateAsync(DialogResource resource, uint language, Stream stream)
        {
            var version = await stream.ReadUInt16Async().ConfigureAwait(false);
            var signature = await stream.ReadUInt16Async().ConfigureAwait(false);
            var helpId = await stream.ReadUInt32Async().ConfigureAwait(false);
            var exStyles = await stream.ReadUInt32Async().ConfigureAwait(false);
            var styles = await stream.ReadUInt32Async().ConfigureAwait(false);
            var itemCount = await stream.ReadUInt16Async().ConfigureAwait(false);
            var x = await stream.ReadInt16Async().ConfigureAwait(false);
            var y = await stream.ReadInt16Async().ConfigureAwait(false);
            var position = new Point(x, y);
            var cx = await stream.ReadInt16Async().ConfigureAwait(false);
            var cy = await stream.ReadInt16Async().ConfigureAwait(false);
            var size = new Size(cx, cy);
            var menuId = await ResourceUtils.OrdOrSzAsync(stream).ConfigureAwait(false);
            ResourceId menu;

            if (menuId.Item1 > 0)
            {
                menu = menuId.Item1;
            }
            else
            {
                menu = menuId.Item2;
            }

            var classId = await ResourceUtils.OrdOrSzAsync(stream).ConfigureAwait(false);
            ResourceId cls;

            if (classId.Item1 > 0)
            {
                cls = classId.Item1;
            }
            else
            {
                cls = classId.Item2;
            }

            var title = await stream.ReadUnicodeStringAsync().ConfigureAwait(false);
            var dialogStyles = (DialogStyle)styles;
            Font font = null;
            var charSet = CharacterSet.ANSI;

            if ((dialogStyles & DialogStyle.DS_SETFONT) == DialogStyle.DS_SETFONT || (dialogStyles & DialogStyle.DS_SHELLFONT) == DialogStyle.DS_SHELLFONT)
            {

                var pointSize = await stream.ReadUInt16Async().ConfigureAwait(false);
                var weight = await stream.ReadUInt16Async().ConfigureAwait(false);
                var italic = await stream.ReadByteAsync().ConfigureAwait(false);
                var fontStyle = (italic > 0 ? FontStyle.Italic : FontStyle.Regular);
                
                charSet = (CharacterSet)await stream.ReadByteAsync().ConfigureAwait(false);

                var typeFace = await stream.ReadUnicodeStringAsync().ConfigureAwait(false);

                font = new Font(typeFace, pointSize, fontStyle);
            }
            else
            {
                font = null;
            }

            var items = new List<DialogItemEx>(itemCount);

            for (var i = 0; i < itemCount; i++)
            {
                while (stream.Position % 4 != 0)
                    await stream.ReadByteAsync().ConfigureAwait(false);

                var item = await DialogItemEx.CreateAsync(stream).ConfigureAwait(false);

                items.Add(item);
            }

            var dialog = new DialogEx(resource, language, items.ToArray())
            {
                HelpId = helpId,
                ExtendedStyles = exStyles,
                Styles = styles,
                Position = position,
                Size = size,
                Menu = menu,
                Class = cls,
                Title = title,
                Font = font,
                CharSet = charSet
            };

            return dialog;
        }

        #endregion

        #region Methods

        public IEnumerator<DialogItemEx> GetEnumerator()
        {
            foreach (var item in _items)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return Title;
        }

        public WindowStyleEx GetExtendedWindowStyles()
        {
            return (WindowStyleEx)ExtendedStyles;
        }

        public WindowStyle GetWindowStyles()
        {
            return (WindowStyle)Styles;
        }

        public DialogStyle GetDialogStyles()
        {
            return (DialogStyle)Styles;
        }

        #endregion

        #region Properties

        public int Count { get; }
        public DialogItemEx this[int index] => _items[index];

        public uint HelpId { get; private set; }
        public uint ExtendedStyles { get; private set; }
        public uint Styles { get; private set; }
        public Point Position { get; private set; }
        public Size Size { get; private set; }
        public ResourceId Menu { get; private set; }
        public ResourceId Class { get; private set; }
        public string Title { get; private set; }
        public Font Font { get; private set; }
        public CharacterSet CharSet { get; private set; }

        #endregion
    }
}



