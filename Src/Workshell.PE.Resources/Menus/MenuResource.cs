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

namespace Workshell.PE
{

    public enum MenuSaveFormat
    {
        Raw,
        Resource
    }

    public sealed class MenuResource : IEnumerable<MenuItem>
    {

        private Resource resource;
        private uint language_id;
        private MenuItem[] items;

        internal MenuResource(Resource sourceResource, uint languageId, byte[] data)
        {
            resource = sourceResource;
            language_id = languageId;

            MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream(data);

            using (mem)
            {
                ushort version = Utils.ReadUInt16(mem);
                ushort header_size = Utils.ReadUInt16(mem);
                List<MenuItem> menu_items = new List<MenuItem>();

                LoadMenu(mem, menu_items);

                items = menu_items.ToArray();
            }
        }

        #region Static Methods

        public static MenuResource Load(Resource resource)
        {
            return Load(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static MenuResource Load(Resource resource, uint language)
        {
            if (!resource.Languages.Contains(language))
                return null;

            if (resource.Type.Id != ResourceType.RT_MENU)
                return null;

            byte[] data = resource.GetBytes(language);
            MenuResource result = new MenuResource(resource, language, data);

            return result;
        }

        #endregion

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

        public void Save(string fileName)
        {
            Save(fileName, MenuSaveFormat.Raw);
        }

        public void Save(Stream stream)
        {
            Save(stream, MenuSaveFormat.Raw);
        }

        public void Save(string fileName, MenuSaveFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, MenuSaveFormat format)
        {
            switch (format)
            {
                case MenuSaveFormat.Raw:
                    SaveRaw(stream);
                    break;
                case MenuSaveFormat.Resource:
                    SaveResource(stream);
                    break;
            }
        }

        private void SaveRaw(Stream stream)
        {
            byte[] data = resource.GetBytes(language_id);

            stream.Write(data, 0, data.Length);
        }

        private void SaveResource(Stream stream)
        {
            throw new NotImplementedException();
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

        #region Properties

        public Resource Resource
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

}
