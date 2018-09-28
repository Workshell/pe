using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Graphics
{
    public sealed class CursorGroupResource : Resource
    {
        public CursorGroupResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var typeId = new ResourceId(ResourceType.GroupCursor);

            return ResourceType.Register<CursorGroupResource>(typeId);
        }

        #endregion

        #region Methods

        public CursorGroup GetGroup()
        {
            return GetGroup(ResourceLanguage.English.UnitedStates);
        }

        public CursorGroup GetGroup(ResourceLanguage language)
        {
            return GetGroupAsync(language).GetAwaiter().GetResult();
        }

        public async Task<CursorGroup> GetGroupAsync()
        {
            return await GetGroupAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public async Task<CursorGroup> GetGroupAsync(ResourceLanguage language)
        {
            var buffer = await GetBytesAsync(language).ConfigureAwait(false);

            using (var mem = new MemoryStream(buffer))
            {
                var header = await mem.ReadStructAsync<NEWHEADER>().ConfigureAwait(false);

                if (header.ResType != 2)
                    throw new Exception("Not a cursor group resource.");

                var entries = new CursorGroupEntry[header.ResCount];

                for (var i = 0; i < header.ResCount; i++)
                {
                    var cursor = await mem.ReadStructAsync<CURSOR_RESDIR>().ConfigureAwait(false);

                    entries[i] = new CursorGroupEntry(cursor);
                }

                var group = new CursorGroup(this, language, entries);

                return group;
            }
        }

        #endregion
    }
}
