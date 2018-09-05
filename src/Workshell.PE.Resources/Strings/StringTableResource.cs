using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Resources.Strings
{
    public sealed class StringTableResource : Resource
    {
        public StringTableResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var typeId = new ResourceId(ResourceType.String);

            return ResourceType.Register<StringTableResource>(typeId);
        }

        #endregion

        #region Methods

        public StringTable GetTable()
        {
            return GetTable(ResourceLanguage.English.UnitedStates);
        }

        public async Task<StringTable> GetTableAsync()
        {
            return await GetTableAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public StringTable GetTable(ResourceLanguage language)
        {
            return GetTableAsync(language).GetAwaiter().GetResult();
        }

        public async Task<StringTable> GetTableAsync(ResourceLanguage language)
        {
            var buffer = await GetBytesAsync(language).ConfigureAwait(false);
            var strings = new List<string>();

            using (var mem = new MemoryStream(buffer))
            {
                try
                {
                    while (mem.Position < mem.Length)
                    {
                        var count = await mem.ReadUInt16Async().ConfigureAwait(false);

                        if (count == 0)
                        {
                            strings.Add(null);
                        }
                        else
                        {
                            var builder = new StringBuilder(count);

                            for (var i = 0; i < count; i++)
                            {
                                var value = await mem.ReadUInt16Async().ConfigureAwait(false);

                                builder.Append((char)value);
                            }

                            strings.Add(builder.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(Image, "Could not read string table from stream.", ex);
                }
            }

            var entries = new StringTableEntry[strings.Count];
            var baseId = Convert.ToUInt16((Id - 1) << 4);

            for (var i = 0; i < strings.Count; i++)
            {
                var value = strings[i];

                if (value == null)
                {
                    entries[i] = new StringTableEntry(0, value);
                }
                else
                {
                    var id = Convert.ToUInt16(baseId + i);

                    entries[i] = new StringTableEntry(id, value);
                }
            }

            var table = new StringTable(this, language, entries);

            return table;
        }

        #endregion
    }
}
