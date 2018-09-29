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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Resources
{
    public sealed class ResourceCollection : IEnumerable<ResourceType>
    {
        private ResourceType[] _types;

        private ResourceCollection(PortableExecutableImage image)
        {
            _types = new ResourceType[0];

            Count = 0;
        }

        #region Static Methods

        public static ResourceCollection Get(PortableExecutableImage image)
        {
            return GetAsync(image).GetAwaiter().GetResult();
        }

        public static async Task<ResourceCollection> GetAsync(PortableExecutableImage image)
        {
            var rootDirectory = await ResourceDirectory.GetAsync(image).ConfigureAwait(false);

            if (rootDirectory == null)
                return null;

            var resources = new ResourceCollection(image);

            await resources.LoadAsync(image, rootDirectory).ConfigureAwait(false);

            return resources;
        }

        #endregion

        #region Methods

        public IEnumerator<ResourceType> GetEnumerator()
        {
            foreach (var type in _types)
                yield return type;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal async Task LoadAsync(PortableExecutableImage image, ResourceDirectory rootDirectory)
        {
            var types = new List<ResourceType>();

            foreach (var entry in rootDirectory)
            {
                var type = await ResourceType.CreateAsync(image, this, entry).ConfigureAwait(false);

                await type.LoadResourcesAsync(image).ConfigureAwait(false);

                types.Add(type);
            }

            _types = types.ToArray();
            Count = types.Count;
        }

        #endregion
        
        #region Properties

        public int Count { get; private set; }
        public ResourceType this[int index] => _types[index];

        #endregion
    }
}
