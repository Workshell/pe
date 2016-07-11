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

        #region Methods

        #endregion

        #region Properties

        public uint HelpId
        {
            get;
            set;
        } 

        public WindowStyleEx ExtendedStyles
        {
            get;
            set;
        }

        public WindowStyle Styles
        {
            get;
            set;
        }

        public Point Position
        {
            get;
            set;
        }

        public Size Size
        {
            get;
            set;
        }

        public uint Id
        {
            get;
            set;
        }

        public string Class
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        #endregion

    }

    public sealed class DialogEx : IEnumerable<DialogItemEx>
    {

        private uint help_id;
        private uint ex_styles;
        private uint styles;
        private DialogItemEx[] items;
        private Point position;
        private Size size;
        private ResourceId menu;
        private ResourceId cls;
        private string title;
        private Font font;

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
            ushort menu_id = LoadDialogExOrdOrSz(stream, out menu_name);

            if (menu_id > 0)
            {
                menu = menu_id;
            }
            else
            {
                menu = menu_name;
            }

            string class_name = String.Empty;
            ushort class_id = LoadDialogExOrdOrSz(stream, out menu_name);

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
        }

        #region Methods

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

        private ushort LoadDialogExOrdOrSz(Stream stream, out string str)
        {
            ushort value = Utils.ReadUInt16(stream);

            if (value == 0)
            {
                str = String.Empty;

                return 0;
            }
            else if (value == 0xFFFF)
            {
                value = Utils.ReadUInt16(stream);

                str = String.Empty;

                return value;
            }
            else
            {
                StringBuilder builder = new StringBuilder();

                while (true)
                {
                    if (value == 0)
                        break;

                    builder.Append((char)value);

                    value = Utils.ReadUInt16(stream);
                }

                str = builder.ToString();

                return value;
            }
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
