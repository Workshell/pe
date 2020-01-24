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
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Resources.Dialogs;

namespace Workshell.PE.Resources.Menus
{
    public sealed class MenuResource : Resource
    {
        public MenuResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var typeId = new ResourceId(ResourceType.Menu);

            return ResourceType.Register<MenuResource>(typeId);
        }

        #endregion

        #region Methods

        public Menu GetMenu()
        {
            return GetMenu(ResourceLanguage.English.UnitedStates);
        }

        public async Task<Menu> GetMenuAsync()
        {
            return await GetMenuAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public Menu GetMenu(ResourceLanguage language)
        {
            return GetMenuAsync(language).GetAwaiter().GetResult();
        }

        public async Task<Menu> GetMenuAsync(ResourceLanguage language)
        {
            var buffer = await GetBytesAsync(language).ConfigureAwait(false);

            using (var mem = new MemoryStream(buffer))
            {
                try
                {
                    var version = await mem.ReadUInt16Async().ConfigureAwait(false);
                    var headerSize = await mem.ReadUInt16Async().ConfigureAwait(false);
                    var items = new List<MenuItem>();

                    await BuildMenuAsync(mem, items).ConfigureAwait(false);

                    var result = new Menu(this, language, items.ToArray());

                    return result;
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(Image, "Could not read menu from stream.", ex);
                }
            }
        }

        private async Task BuildMenuAsync(Stream stream, IList<MenuItem> items)
        {
            while (true)
            {
                var flags = await stream.ReadUInt16Async().ConfigureAwait(false);
                var miFlags = (MenuItemFlags)flags;

                if ((miFlags & MenuItemFlags.Popup) == MenuItemFlags.Popup)
                {
                    var builder = new StringBuilder(256);

                    while (true)
                    {
                        var value = await stream.ReadUInt16Async().ConfigureAwait(false);

                        if (value == 0)
                            break;

                        builder.Append((char)value);
                    }

                    var subItems = new List<MenuItem>();

                    await BuildMenuAsync(stream, subItems).ConfigureAwait(false);

                    var item = new PopupMenuItem(0, builder.ToString(), flags, subItems.ToArray());

                    items.Add(item);
                }
                else
                {
                    var id = await stream.ReadUInt16Async().ConfigureAwait(false);
                    var builder = new StringBuilder(256);

                    while (true)
                    {
                        var value = await stream.ReadUInt16Async().ConfigureAwait(false);

                        if (value == 0)
                            break;

                        builder.Append((char)value);
                    }

                    var item = new MenuItem(id, builder.ToString(), flags);

                    items.Add(item);
                }

                if ((miFlags & MenuItemFlags.EndMenu) == MenuItemFlags.EndMenu)
                    break;
            }
        }

        #endregion
    }
}
