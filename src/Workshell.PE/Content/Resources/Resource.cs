using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Content
{
    public class Resource : ISupportsBytes
    {
        public const uint DefaultLanguage = 1033;

        private readonly PortableExecutableImage _image;
        private readonly Dictionary<uint, ResourceDirectoryEntry> _languages;

        protected Resource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id)
        {
            _image = image;
            _languages = BuildLanguages(entry);

            Type = type;
            Entry = entry;
            Id = id;
            Languages = _languages.Keys.OrderBy(k => k).ToArray();
        }

        #region Static Methods

        internal static async Task<Resource> CreateAsync(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, Type resourceType)
        {
            ResourceId id;

            if (entry.NameType == NameType.ID)
            {
                id = entry.GetId();
            }
            else
            {
                id = await entry.GetNameAsync().ConfigureAwait(false);
            }

            var ctors = resourceType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var ctor = ctors.First();
            var resource = (Resource)ctor.Invoke(new object[] { image, type, entry, id });

            return resource;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            if (!Id.IsNumeric)
                return Id.Name;

            return $"#{Id}";
        }

        public ResourceData GetData(uint language = DefaultLanguage)
        {
            return GetDataAsync(language).GetAwaiter().GetResult();
        }

        public async Task<ResourceData> GetDataAsync(uint language = DefaultLanguage)
        {
            if (!_languages.ContainsKey(language))
                return null;

            var languageEntry = _languages[language];
            var entry = await languageEntry.GetDataEntryAsync().ConfigureAwait(false);
            var data = entry.GetData();

            return data;
        }

        public byte[] GetBytes()
        {
            return GetBytes(DefaultLanguage);
        }

        public async Task<byte[]> GetBytesAsync()
        {
            return await GetBytesAsync(DefaultLanguage);
        }

        public byte[] GetBytes(uint language)
        {
            return GetBytesAsync(language).GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync(uint language)
        {
            if (!_languages.ContainsKey(language))
                throw new PortableExecutableImageException(_image, $"Cannot find specified language: {language}");

            var data = await GetDataAsync(language).ConfigureAwait(false);

            if (data == null)
                throw new PortableExecutableImageException(_image, $"Cannot find resource data for language: {language}");

            return await data.GetBytesAsync().ConfigureAwait(false);
        }

        public void CopyTo(Stream stream, uint language = DefaultLanguage)
        {
            CopyToAsync(stream, language).GetAwaiter().GetResult();
        }

        public async Task CopyToAsync(Stream stream, uint language = DefaultLanguage)
        {
            var data = await GetDataAsync(language).ConfigureAwait(false);

            await data.CopyToAsync(stream).ConfigureAwait(false);
        }

        private Dictionary<uint, ResourceDirectoryEntry> BuildLanguages(ResourceDirectoryEntry parentEntry)
        {
            var results = new Dictionary<uint, ResourceDirectoryEntry>();
            var directory = parentEntry.GetDirectory();

            foreach (var entry in directory)
                results.Add(entry.GetId(),entry);

            return results;
        }

        #endregion

        #region Properties

        public ResourceType Type { get; }
        public ResourceDirectoryEntry Entry { get; }
        public ResourceId Id { get; }
        public uint[] Languages { get; }

        #endregion
    }
}
