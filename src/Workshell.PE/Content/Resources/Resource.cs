using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Resources
{
    public class Resource : ISupportsBytes
    {
        private readonly Dictionary<ResourceLanguage, ResourceDirectoryEntry> _languages;

        protected Resource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id)
        {
            _languages = BuildLanguages(entry);

            Type = type;
            Entry = entry;
            Id = id;
            Languages = _languages.Keys.OrderBy(k => k).ToArray();
            Image = image;
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

        public ResourceData GetData()
        {
            return GetData(ResourceLanguage.English.UnitedStates);
        }

        public async Task<ResourceData> GetDataAsync()
        {
            return await GetDataAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public ResourceData GetData(ResourceLanguage language)
        {
            return GetDataAsync(language).GetAwaiter().GetResult();
        }

        public async Task<ResourceData> GetDataAsync(ResourceLanguage language)
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
            return GetBytes(ResourceLanguage.English.UnitedStates);
        }

        public async Task<byte[]> GetBytesAsync()
        {
            return await GetBytesAsync(ResourceLanguage.English.UnitedStates);
        }

        public byte[] GetBytes(ResourceLanguage language)
        {
            return GetBytesAsync(language).GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync(ResourceLanguage language)
        {
            if (!_languages.ContainsKey(language))
                throw new PortableExecutableImageException(Image, $"Cannot find specified language: {language}");

            var data = await GetDataAsync(language).ConfigureAwait(false);

            if (data == null)
                throw new PortableExecutableImageException(Image, $"Cannot find resource data for language: {language}");

            return await data.GetBytesAsync().ConfigureAwait(false);
        }

        public void CopyTo(Stream stream)
        {
            CopyTo(stream, ResourceLanguage.English.UnitedStates);
        }

        public async Task CopyToAsync(Stream stream)
        {
            await CopyToAsync(stream, ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public void CopyTo(Stream stream, ResourceLanguage language)
        {
            CopyToAsync(stream, language).GetAwaiter().GetResult();
        }

        public async Task CopyToAsync(Stream stream, ResourceLanguage language)
        {
            var data = await GetDataAsync(language).ConfigureAwait(false);

            await data.CopyToAsync(stream).ConfigureAwait(false);
        }

        private Dictionary<ResourceLanguage, ResourceDirectoryEntry> BuildLanguages(ResourceDirectoryEntry parentEntry)
        {
            var results = new Dictionary<ResourceLanguage, ResourceDirectoryEntry>();
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
        public ResourceLanguage[] Languages { get; }
        protected PortableExecutableImage Image { get; }

        #endregion
    }
}
