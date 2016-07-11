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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Annotations;

namespace Workshell.PE.Resources
{

    [Flags]
    public enum MenuItemFlags : ushort
    {
        Enabled = 0,
        Grayed = 0x0001,
        Disabled = 0x0002,
        Bitmap = 0x0004,
        OwnerDraw = 0x0100,
        Checked = 0x0008,
        Popup = 0x0010,
        MenubarBreak = 0x0020,
        MenuBreak = 0x0040,
        EndMenu = 0x0080,
        Seperator = 0x0800
    }

    public class MenuItem
    {

        private ushort id;
        private string text;
        private string shortcut;
        private ushort flags;

        internal MenuItem(ushort itemId, string itemText, ushort itemFlags)
        {
            id = itemId;

            string[] parts = itemText.Split(new char[] { '\t' }, 2);

            text = parts[0];

            if (parts.Length > 1)
            {
                shortcut = parts[1];
            }
            else
            {
                shortcut = String.Empty;
            }

            flags = itemFlags;
        }

        #region Methods

        public override string ToString()
        {
            if (!IsSeperator)
            {
                string result = text;

                if (IsPopup)
                    result = "+" + result;

                if (shortcut != String.Empty)
                    result += " | " + shortcut;

                return result;
            }
            else
            {
                return "-";
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

        public string Text
        {
            get
            {
                return text;
            }
        }

        public string Shortcut
        {
            get
            {
                return shortcut;
            }
        }

        public MenuItemFlags Flags
        {
            get
            {
                return (MenuItemFlags)flags;
            }
        }

        public bool IsPopup
        {
            get
            {
                return (id == 0);
            }
        }

        public bool IsSeperator
        {
            get
            {
                return (id == 0 && flags == 0 && shortcut == String.Empty);
            }
        }

        #endregion

    }

    public sealed class PopupMenuItem : MenuItem, IEnumerable<MenuItem>
    {

        private MenuItem[] items;

        internal PopupMenuItem(ushort itemId, string itemText, ushort itemFlags, MenuItem[] subItems) : base(itemId, itemText, itemFlags)
        {
            items = subItems;
        }

        #region Methods

        public IEnumerator<MenuItem> GetEnumerator()
        {
            for (var i = 0; i < items.Length; i++)
                yield return items[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public int Count
        {
            get
            {
                return items.Length;
            }
        }

        public MenuItem this[int index]
        {
            get
            {
                return items[index];
            }
        }

        #endregion

    }

}
