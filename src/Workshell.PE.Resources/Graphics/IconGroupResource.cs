using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Graphics
{
    public sealed class IconGroupResource : Resource
    {
        public IconGroupResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var typeId = new ResourceId(ResourceType.GroupIcon);

            return ResourceType.Register<IconGroupResource>(typeId);
        }

        #endregion

        #region Methods

        public IconGroup GetGroup()
        {
            return GetGroup(ResourceLanguage.English.UnitedStates);
        }

        public async Task<IconGroup> GetGroupAsync()
        {
            return await GetGroupAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public IconGroup GetGroup(ResourceLanguage language)
        {
            return GetGroupAsync(language).GetAwaiter().GetResult();
        }

        public async Task<IconGroup> GetGroupAsync(ResourceLanguage language)
        {
            var buffer = await GetBytesAsync(language).ConfigureAwait(false);

            using (var mem = new MemoryStream(buffer))
            {
                var header = await mem.ReadStructAsync<NEWHEADER>().ConfigureAwait(false);

                if (header.ResType != 1)
                    throw new Exception("Not an icon group resource.");

                var entries = new IconGroupEntry[header.ResCount];

                for (var i = 0; i < entries.Length; i++)
                {
                    var icon = await mem.ReadStructAsync<ICON_RESDIR>().ConfigureAwait(false);

                    entries[i] = new IconGroupEntry(icon);
                }

                var group = new IconGroup(language, entries);

                return group;
            }
        }

        #endregion
    }
}
