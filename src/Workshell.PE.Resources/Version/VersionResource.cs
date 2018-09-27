using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;
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

        internal static async Task<int> AlignWordBoundaryAsync(Stream stream)
        {
            var count = 0;

            while (stream.Position % 4 != 0)
            {
                await stream.ReadUInt16Async().ConfigureAwait(false);

                count += sizeof(ushort);
            }

            return count;
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
                var count = 3 * sizeof(ushort);

                await mem.ReadBytesAsync(count).ConfigureAwait(false);

                var key = await mem.ReadUnicodeStringAsync().ConfigureAwait(false);

                if (key != "VS_VERSION_INFO")
                    throw new Exception("Invalid file version information.");

                await AlignWordBoundaryAsync(mem).ConfigureAwait(false);

                var ffiData = await mem.ReadStructAsync<VS_FIXEDFILEINFO>().ConfigureAwait(false);
                var fixedFileInfo = new FixedFileInfo(ffiData);

                await AlignWordBoundaryAsync(mem).ConfigureAwait(false);

                var stringFileInfo = await StringFileInfo.LoadAsync(mem).ConfigureAwait(false);

                await AlignWordBoundaryAsync(mem).ConfigureAwait(false);

                var varFileInfo = await VarFileInfo.LoadAsync(mem).ConfigureAwait(false);
                var info = new VersionInfo(this, language, fixedFileInfo, stringFileInfo, varFileInfo);

                return info;
            }
        }

        #endregion
    }
}
