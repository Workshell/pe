using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Graphics
{
    public sealed class CursorResource : Resource
    {
        public CursorResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }
    }
}
