using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workshell.PE
{

    public interface IResourceProvider
    {

        Resource Create(ResourceType type, ResourceDirectoryEntry entry);

    }

    public sealed class DefaultResourceProvider : IResourceProvider
    {

        #region Methods

        public Resource Create(ResourceType type, ResourceDirectoryEntry entry)
        {
            Resource resource = new Resource(type, entry);

            return resource;
        }

        #endregion

    }

}
