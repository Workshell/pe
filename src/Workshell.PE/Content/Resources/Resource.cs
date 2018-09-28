#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

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
