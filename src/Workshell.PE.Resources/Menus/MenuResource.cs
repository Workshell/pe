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
                    throw new PortableExecutableImageException(Image, "Could not read dialog from stream.", ex);
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
