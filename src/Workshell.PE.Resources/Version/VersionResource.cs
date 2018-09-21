using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
using Workshell.PE.Resources.Messages;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Version
{
    public sealed class VersionResource : Resource
    {
        public VersionResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var type = new ResourceId(ResourceType.Version);

            return ResourceType.Register<VersionResource>(type);
        }

        #endregion

        #region Methods

        public VersionInfo GetInfo()
        {
            return GetInfo(ResourceLanguage.English.UnitedStates);
        }

        public async Task<VersionInfo> GetInfoAsync()
        {
            return await GetInfoAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public VersionInfo GetInfo(ResourceLanguage language)
        {
            return GetInfoAsync(language).GetAwaiter().GetResult();
        }

        public async Task<VersionInfo> GetInfoAsync(ResourceLanguage language)
        {
            var buffer = await GetBytesAsync(language).ConfigureAwait(false);

            using (var mem = new MemoryStream(buffer))
            {
                var len = await mem.ReadUInt16Async().ConfigureAwait(false);
                var valLen = await mem.ReadUInt16Async().ConfigureAwait(false);
                var type = await mem.ReadUInt16Async().ConfigureAwait(false);
                var key = await mem.ReadUnicodeStringAsync().ConfigureAwait(false);

                if (mem.Position % 4 != 0)
                    await mem.ReadUInt16Async().ConfigureAwait(false);

                var ffiData = await mem.ReadStructAsync<VS_FIXEDFILEINFO>().ConfigureAwait(false);
                var fixedFileInfo = new FixedFileInfo(ffiData);

                while (mem.Position % 4 != 0)
                    await mem.ReadUInt16Async().ConfigureAwait(false);

                var stringFileInfo = await StringFileInfo.LoadAsync(mem).ConfigureAwait(false);

                //if (mem.Position % 4 != 0)
                //    await mem.ReadUInt16Async().ConfigureAwait(false);

                var info = new VersionInfo(this, language, fixedFileInfo, stringFileInfo, null);

                return info;
            }
        }

        #endregion
    }
}
