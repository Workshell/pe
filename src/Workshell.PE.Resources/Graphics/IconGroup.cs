using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Resources.Graphics
{
    public enum IconGroupSaveFormat
    {
        Raw,
        Icon
    }

    public sealed class IconGroup : IEnumerable<IconGroupEntry>
    {
        private readonly IconGroupResource _resource;
        private readonly IconGroupEntry[] _entries;

        internal IconGroup(IconGroupResource resource, ResourceLanguage language, IconGroupEntry[] entries)
        {
            _resource = resource;
            _entries = entries;

            Language = language;
            Count = entries.Length;
        }

        #region Methods

        public void Save(string fileName)
        {
            Save(fileName, ResourceLanguage.English.UnitedStates);
        }

        public void Save(string fileName, ResourceLanguage language)
        {
            Save(fileName, language, IconGroupSaveFormat.Icon);
        }

        public void Save(string fileName, ResourceLanguage language, IconGroupSaveFormat saveFormat)
        {
            SaveAsync(fileName, language, saveFormat).GetAwaiter().GetResult();
        }

        public async Task SaveAsync(string fileName)
        {
            await SaveAsync(fileName, ResourceLanguage.English.UnitedStates);
        }

        public async Task SaveAsync(string fileName, ResourceLanguage language)
        {
            await SaveAsync(fileName, language, IconGroupSaveFormat.Icon);
        }

        public async Task SaveAsync(string fileName, ResourceLanguage language, IconGroupSaveFormat saveFormat)
        {
            using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await SaveAsync(file, language, saveFormat).ConfigureAwait(false);
                await file.FlushAsync().ConfigureAwait(false);
            }
        }

        public void Save(Stream stream)
        {
            Save(stream, ResourceLanguage.English.UnitedStates);
        }

        public void Save(Stream stream, ResourceLanguage language)
        {
            Save(stream, language, IconGroupSaveFormat.Icon);
        }

        public void Save(Stream stream, ResourceLanguage language, IconGroupSaveFormat saveFormat)
        {
            SaveAsync(stream, language, saveFormat).GetAwaiter().GetResult();
        }

        public async Task SaveAsync(Stream stream)
        {
            await SaveAsync(stream, ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public async Task SaveAsync(Stream stream, ResourceLanguage language)
        {
            await SaveAsync(stream, language, IconGroupSaveFormat.Icon).ConfigureAwait(false);
        }

        public async Task SaveAsync(Stream stream, ResourceLanguage language, IconGroupSaveFormat saveFormat)
        {
            if (saveFormat == IconGroupSaveFormat.Raw)
            {
                var buffer = await _resource.GetBytesAsync(language).ConfigureAwait(false);

                await stream.WriteBytesAsync(buffer).ConfigureAwait(false);
            }
            else
            {
                var group = await _resource.GetGroupAsync(language).ConfigureAwait(false);
                var offsets = new uint[group.Count];
                var offset = (6 + (16 * offsets.Length)).ToUInt32();

                for (var i = 0; i < group.Count; i++)
                {
                    var entry = group[i];

                    offsets[i] = offset;
                    offset += entry.BytesInRes;
                }

                await stream.WriteUInt16Async(0).ConfigureAwait(false);
                await stream.WriteUInt16Async(1).ConfigureAwait(false);
                await stream.WriteUInt16Async(group.Count.ToUInt16()).ConfigureAwait(false);

                for (var i = 0; i < group.Count; i++)
                {
                    var entry = group[i];
                    var colorCount = Convert.ToUInt64(Math.Pow(2, entry.BitCount));

                    if (colorCount >= 256)
                        colorCount = 0;

                    await stream.WriteByteAsync(Convert.ToByte(entry.Width >= 256 ? 0 : entry.Width)).ConfigureAwait(false);
                    await stream.WriteByteAsync(Convert.ToByte(entry.Height >= 256 ? 0 : entry.Height)).ConfigureAwait(false);
                    await stream.WriteByteAsync(Convert.ToByte(colorCount)).ConfigureAwait(false);
                    await stream.WriteByteAsync(0).ConfigureAwait(false);
                    await stream.WriteUInt16Async(1).ConfigureAwait(false);
                    await stream.WriteUInt16Async(entry.BitCount).ConfigureAwait(false);
                    await stream.WriteUInt32Async(entry.BytesInRes).ConfigureAwait(false);
                    await stream.WriteUInt32Async(offsets[i]).ConfigureAwait(false);
                }

                var icons = _resource.Type.Resources.First(type => type.Id == ResourceType.Icon);

                for (var i = 0; i < group.Count; i++)
                {
                    var entry = group[i];
                    var resource = icons.First(r => r.Id == entry.IconId);
                    var buffer = await resource.GetBytesAsync(language).ConfigureAwait(false);

                    await stream.WriteBytesAsync(buffer).ConfigureAwait(false);
                }
            }
        }

        public IEnumerator<IconGroupEntry> GetEnumerator()
        {
            foreach (var entry in _entries)
                yield return entry;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Properties

        public ResourceLanguage Language { get; }
        public int Count { get; }
        public IconGroupEntry this[int index] => _entries[index];

        #endregion
    }
}
