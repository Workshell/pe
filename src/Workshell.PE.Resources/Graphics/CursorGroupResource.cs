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
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Graphics
{
    public sealed class CursorGroupResource : Resource
    {
        public CursorGroupResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var typeId = new ResourceId(ResourceType.GroupCursor);

            return ResourceType.Register<CursorGroupResource>(typeId);
        }

        #endregion

        #region Methods

        public CursorGroup GetGroup()
        {
            return GetGroup(ResourceLanguage.English.UnitedStates);
        }

        public CursorGroup GetGroup(ResourceLanguage language)
        {
            return GetGroupAsync(language).GetAwaiter().GetResult();
        }

        public async Task<CursorGroup> GetGroupAsync()
        {
            return await GetGroupAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public async Task<CursorGroup> GetGroupAsync(ResourceLanguage language)
        {
            var buffer = await GetBytesAsync(language).ConfigureAwait(false);

            using (var mem = new MemoryStream(buffer))
            {
                var header = await mem.ReadStructAsync<NEWHEADER>().ConfigureAwait(false);

                if (header.ResType != 2)
                    throw new Exception("Not a cursor group resource.");

                var entries = new CursorGroupEntry[header.ResCount];

                for (var i = 0; i < header.ResCount; i++)
                {
                    var cursor = await mem.ReadStructAsync<CURSOR_RESDIR>().ConfigureAwait(false);

                    entries[i] = new CursorGroupEntry(cursor);
                }

                var group = new CursorGroup(this, language, entries);

                return group;
            }
        }

        #endregion
    }
}
