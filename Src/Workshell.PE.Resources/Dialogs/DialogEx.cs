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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Resources
{

    public sealed class DialogItemEx
    {

        public const ushort BUTTON = 0x0080;
        public const ushort EDIT = 0x0081;
        public const ushort STATIC = 0x0082;
        public const ushort LISTBOX = 0x0083;
        public const ushort SCROLLBAR = 0x0084;
        public const ushort COMBOBOX = 0x0085;

        private uint help_id;
        private uint ex_styles;
        private uint styles;
        private Point position;
        private Size size;
        private int id;
        private ResourceId cls;
        private ResourceId title;
        private byte[] extra_data;

        internal DialogItemEx(Stream stream)
        {
            help_id = Utils.ReadUInt32(stream);
            ex_styles = Utils.ReadUInt32(stream);
            styles = Utils.ReadUInt32(stream);

            short x = Utils.ReadInt16(stream);
            short y = Utils.ReadInt16(stream);

            position = new Point(x, y);

            short cx = Utils.ReadInt16(stream);
            short cy = Utils.ReadInt16(stream);

            size = new Size(cx, cy);
            id = Utils.ReadInt32(stream);

            string class_name = String.Empty;
            ushort class_id = ResourceUtils.OrdOrSz(stream, out class_name);

            if (class_id > 0)
            {
                switch (class_id)
                {
                    case BUTTON:
                        class_name = "BUTTON";
                        break;
                    case EDIT:
                        class_name = "EDIT";
                        break;
                    case STATIC:
                        class_name = "STATIC";
                        break;
                    case LISTBOX:
                        class_name = "LISTBOX";
                        break;
                    case SCROLLBAR:
                        class_name = "SCROLLBAR";
                        break;
                    case COMBOBOX:
                        class_name = "COMBOBOX";
                        break;
                }

                cls = new ResourceId(class_id, class_name);
            }
            else
            {
                cls = new ResourceId(class_name);
            }

            string title_name = String.Empty;
            ushort title_id = ResourceUtils.OrdOrSz(stream, out title_name);

            if (title_id > 0)
            {
                title = new ResourceId(title_id);
            }
            else
            {
                title = new ResourceId(title_name);
            }

            ushort extra_count = Utils.ReadUInt16(stream);

            if (stream.Position % 2 != 0)
                Utils.ReadByte(stream);

            extra_data = new byte[extra_count];

            if (extra_data.Length > 0)
                extra_data = Utils.ReadBytes(stream, extra_count);
        }

        #region Methods

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", title, id, cls);
        }

        public T GetControlStyle<T>()
        {
            Type type = typeof(T);

            if (!type.IsEnum)
                throw new Exception("Not an enum type.");

            return (T)Enum.Parse(typeof(T), styles.ToString(), true);
        }

        #endregion

        #region Properties

        public uint HelpId
        {
            get
            {
                return help_id;
            }
        } 

        public uint ExtendedStyles
        {
            get
            {
                return ex_styles;
            }
        }

        public uint Styles
        {
            get
            {
                return styles;
            }
        }

        public Point Position
        {
            get
            {
                return position;
            }
        }

        public Size Size
        {
            get
            {
                return size;
            }
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        public ResourceId Class
        {
            get
            {
                return cls;
            }
        }

        public ResourceId Title
        {
            get
            {
                return title;
            }
        }

        public byte[] ExtraData
        {
            get
            {
                return extra_data;
            }
        }

        #endregion

    }

    public sealed class DialogEx : IEnumerable<DialogItemEx>
    {

        private uint help_id;
        private uint ex_styles;
        private uint styles;
        private Point position;
        private Size size;
        private ResourceId menu;
        private ResourceId cls;
        private string title;
        private Font font;
        private DialogItemEx[] items;

        internal DialogEx(Stream stream)
        {
            ushort ver = Utils.ReadUInt16(stream);
            ushort sig = Utils.ReadUInt16(stream);
       
            help_id = Utils.ReadUInt32(stream);
            ex_styles = Utils.ReadUInt32(stream);
            styles = Utils.ReadUInt32(stream);

            ushort item_count = Utils.ReadUInt16(stream);

            short x = Utils.ReadInt16(stream);
            short y = Utils.ReadInt16(stream);

            position = new Point(x, y);

            short cx = Utils.ReadInt16(stream);
            short cy = Utils.ReadInt16(stream);

            size = new Size(cx, cy);

            string menu_name = String.Empty;
            ushort menu_id = ResourceUtils.OrdOrSz(stream, out menu_name);

            if (menu_id > 0)
            {
                menu = menu_id;
            }
            else
            {
                menu = menu_name;
            }

            string class_name = String.Empty;
            ushort class_id = ResourceUtils.OrdOrSz(stream, out menu_name);

            if (class_id > 0)
            {
                cls = class_id;
            }
            else
            {
                cls = class_name;
            }

            title = Utils.ReadUnicodeString(stream);

            DialogStyle dialog_styles = GetDialogStyles();

            if ((dialog_styles & DialogStyle.DS_SETFONT) == DialogStyle.DS_SETFONT)
            {
                ushort point_size = Utils.ReadUInt16(stream);
                ushort weight = Utils.ReadUInt16(stream);
                byte italic = Utils.ReadByte(stream);
                byte charset = Utils.ReadByte(stream);
                string type_face = Utils.ReadUnicodeString(stream);

                font = new Font(type_face, point_size);
            }
            else
            {
                font = null;
            }

            items = new DialogItemEx[item_count];

            for (var i = 0; i < item_count; i++)
            {
                while (stream.Position % 4 != 0)
                    Utils.ReadByte(stream);

                DialogItemEx item = new Resources.DialogItemEx(stream);

                items[i] = item;
            }
        }

        #region Methods

        public override string ToString()
        {
            return title;
        }

        public IEnumerator<DialogItemEx> GetEnumerator()
        {
            for (var i = 0; i < items.Length; i++)
                yield return items[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public WindowStyleEx GetExtendedWindowStyles()
        {
            return (WindowStyleEx)ex_styles;
        }

        public WindowStyle GetWindowStyles()
        {
            return (WindowStyle)styles;
        }

        public DialogStyle GetDialogStyles()
        {
            return (DialogStyle)styles;
        }

        #endregion

        #region Properties

        public uint HelpId
        {
            get
            {
                return help_id;
            }
        }

        public uint ExtendedStyles
        {
            get
            {
                return ex_styles;
            }
        }

        public uint Styles
        {
            get
            {
                return styles;
            }
        }

        public Point Position
        {
            get
            {
                return position;
            }
        }

        public Size Size
        {
            get
            {
                return size;
            }
        }

        public ResourceId Menu
        {
            get
            {
                return menu;
            }
        }

        public ResourceId Class
        {
            get
            {
                return cls;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public Font Font
        {
            get
            {
                return font;
            }
        }

        public int ItemCount
        {
            get
            {
                return items.Length;
            }
        }

        public DialogItemEx this[int index]
        {
            get
            {
                return items[index];
            }
        }

        #endregion

    }

}
