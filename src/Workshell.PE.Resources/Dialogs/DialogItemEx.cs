#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Resources.Dialogs
{
    public sealed class DialogItemEx
    {
        public const ushort Button = 0x0080;
        public const ushort Edit = 0x0081;
        public const ushort Static = 0x0082;
        public const ushort ListBox = 0x0083;
        public const ushort ScrollBar = 0x0084;
        public const ushort ComboBox = 0x0085;

        private DialogItemEx()
        {
        }

        #region Static Methods

        internal static async Task<DialogItemEx> CreateAsync(Stream stream)
        {
            var helpId = await stream.ReadUInt32Async().ConfigureAwait(false);
            var exStyles = await stream.ReadUInt32Async().ConfigureAwait(false);
            var styles = await stream.ReadUInt32Async().ConfigureAwait(false);
            var x = await stream.ReadInt16Async().ConfigureAwait(false);
            var y = await stream.ReadInt16Async().ConfigureAwait(false);
            var position = new Point(x, y);
            var cx = await stream.ReadInt16Async().ConfigureAwait(false);
            var cy = await stream.ReadInt16Async().ConfigureAwait(false);
            var size = new Size(cx, cy);
            var id = await stream.ReadInt32Async().ConfigureAwait(false);
            var classId = await ResourceUtils.OrdOrSzAsync(stream).ConfigureAwait(false);
            var className = string.Empty;
            ResourceId cls;

            if (classId.Item1 > 0)
            {
                switch (classId.Item1)
                {
                    case Button:
                        className = "BUTTON";
                        break;
                    case Edit:
                        className = "EDIT";
                        break;
                    case Static:
                        className = "STATIC";
                        break;
                    case ListBox:
                        className = "LISTBOX";
                        break;
                    case ScrollBar:
                        className = "SCROLLBAR";
                        break;
                    case ComboBox:
                        className = "COMBOBOX";
                        break;
                }

                cls = new ResourceId(classId.Item1, className);
            }
            else
            {
                cls = new ResourceId(classId.Item2);
            }

            var titleId = await ResourceUtils.OrdOrSzAsync(stream).ConfigureAwait(false);
            ResourceId title;

            if (titleId.Item1 > 0)
            {
                title = new ResourceId(titleId.Item1);
            }
            else
            {
                title = new ResourceId(titleId.Item2);
            }

            var extraCount = await stream.ReadUInt16Async().ConfigureAwait(false);

            if (stream.Position % 2 != 0)
                await stream.ReadByteAsync().ConfigureAwait(false);

            var extraData = new byte[extraCount];

            if (extraData.Length > 0)
                extraData = await stream.ReadBytesAsync(extraCount).ConfigureAwait(false);       
            
            var result = new DialogItemEx()
            {
                HelpId = helpId,
                ExtendedStyles = exStyles,
                Styles = styles,
                Position = position,
                Size = size,
                Id = id,
                Class = cls,
                Title = title,
                ExtraData = extraData
            };

            return result;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{Title}, {Id}, {Class}";
        }

        public T GetControlStyle<T>()
        {
            var type = typeof(T);

            if (!type.IsEnum)
                throw new Exception("Not an enum type.");

            return (T)Enum.Parse(typeof(T), Styles.ToString(), true);
        }

        #endregion

        #region Properties

        public uint HelpId { get; private set; }
        public uint ExtendedStyles { get; private set; }
        public uint Styles { get; private set; }
        public Point Position { get; private set; }
        public Size Size { get; private set; }
        public int Id { get; private set; }
        public ResourceId Class { get; private set; }
        public ResourceId Title { get; private set; }
        public byte[] ExtraData { get; private set; }

        #endregion
    }
}
