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

namespace Workshell.PE.Resources
{

    public sealed class Menu : IEnumerable<MenuItem>
    {

        private MenuResource resource;
        private uint language_id;
        private MenuItem[] items;

        internal Menu(MenuResource menuResource, uint languageId, MenuItem[] menuItems)
        {
            resource = menuResource;
            language_id = languageId;
            items = menuItems;
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

        public MenuResource Resource
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

    public sealed class MenuResource : Resource
    {

        public MenuResource(ResourceType owningType, ResourceDirectoryEntry directoryEntry) : base(owningType, directoryEntry)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            ResourceId resource_type = new ResourceId(ResourceType.RT_MENU);

            return ResourceType.Register(resource_type, typeof(MenuResource));
        }

        #endregion

        #region Methods

        public Menu ToMenu()
        {
            return ToMenu(DEFAULT_LANGUAGE);
        }

        public Menu ToMenu(uint languageId)
        {
            byte[] data = GetBytes(languageId);

            using (MemoryStream mem = new MemoryStream(data))
            {
                ushort version = Utils.ReadUInt16(mem);
                ushort header_size = Utils.ReadUInt16(mem);
                List<MenuItem> menu_items = new List<MenuItem>();

                LoadMenu(mem, menu_items);

                MenuItem[] items = menu_items.ToArray();
                Menu menu = new Resources.Menu(this, languageId, items);

                return menu;
            }
        }

        private void LoadMenu(Stream stream, List<MenuItem> items)
        {
            while (true)
            {
                ushort flags = Utils.ReadUInt16(stream);
                MenuItemFlags menu_item_flags = (MenuItemFlags)flags;

                if ((menu_item_flags & MenuItemFlags.Popup) == MenuItemFlags.Popup)
                {
                    StringBuilder builder = new StringBuilder();

                    while (true)
                    {
                        ushort value = Utils.ReadUInt16(stream);

                        if (value == 0)
                            break;

                        builder.Append((char)value);
                    }

                    List<MenuItem> sub_menu_items = new List<MenuItem>();

                    LoadMenu(stream, sub_menu_items);

                    PopupMenuItem popup_menu_item = new PopupMenuItem(0, builder.ToString(), flags, sub_menu_items.ToArray());

                    items.Add(popup_menu_item);
                }
                else
                {
                    ushort id = Utils.ReadUInt16(stream);
                    StringBuilder builder = new StringBuilder();

                    while (true)
                    {
                        ushort value = Utils.ReadUInt16(stream);

                        if (value == 0)
                            break;

                        builder.Append((char)value);
                    }

                    MenuItem menu_item = new MenuItem(id, builder.ToString(), flags);

                    items.Add(menu_item);
                }

                if ((menu_item_flags & MenuItemFlags.EndMenu) == MenuItemFlags.EndMenu)
                    break;
            }
        }

        #endregion

    }

}
