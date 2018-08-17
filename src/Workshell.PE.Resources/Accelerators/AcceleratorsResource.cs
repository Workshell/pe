using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Content;
using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Accelerators
{
    public sealed class AcceleratorsResource : Resource
    {
        public AcceleratorsResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var typeId = new ResourceId(ResourceType.Accelerator);

            return ResourceType.Register<AcceleratorsResource>(typeId);
        }

        #endregion

        #region Methods

        public AcceleratorsTable Get()
        {
            return Get(ResourceLanguage.English.UnitedStates);
        }

        public async Task<AcceleratorsTable> GetAsync()
        {
            return await GetAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public AcceleratorsTable Get(ResourceLanguage language)
        {
            return GetAsync(language).GetAwaiter().GetResult();
        }

        public async Task<AcceleratorsTable> GetAsync(ResourceLanguage language)
        {
            var stream = Image.GetStream();
            var data = await GetBytesAsync(language).ConfigureAwait(false);

            using (var mem = new MemoryStream(data))
            {
                var size = Marshal.SizeOf<ACCELTABLEENTRY>();
                var count = mem.Length / size;
                var accelerators = new AcceleratorEntry[count];

                for (var i = 0; i < count; i++)
                {
                    try
                    {
                        var entry = await stream.ReadStructAsync<ACCELTABLEENTRY>(size).ConfigureAwait(false);
                        var accelerator = new AcceleratorEntry(entry.fFlags, entry.wAnsi, entry.wId);

                        accelerators[i] = accelerator;
                    }
                    catch (Exception ex)
                    {
                        throw new PortableExecutableImageException(Image, "Could not read accelerator entry from stream.", ex);
                    }
                }

                var table = new AcceleratorsTable(this, language, accelerators);

                return table;
            }
        }

        #endregion
    }
}
