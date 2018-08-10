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
