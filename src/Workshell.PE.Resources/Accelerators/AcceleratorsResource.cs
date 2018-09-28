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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Content;
using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Accelerators
{
    public sealed class AcceleratorsResource : Resource
    {
        public AcceleratorsResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var typeId = new ResourceId(ResourceType.Accelerator);

            return ResourceType.Register<AcceleratorsResource>(typeId);
        }

        #endregion

        #region Methods

        public AcceleratorsTable Get()
        {
            return Get(ResourceLanguage.English.UnitedStates);
        }

        public async Task<AcceleratorsTable> GetAsync()
        {
            return await GetAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public AcceleratorsTable Get(ResourceLanguage language)
        {
            return GetAsync(language).GetAwaiter().GetResult();
        }

        public async Task<AcceleratorsTable> GetAsync(ResourceLanguage language)
        {
            var stream = Image.GetStream();
            var data = await GetBytesAsync(language).ConfigureAwait(false);

            using (var mem = new MemoryStream(data))
            {
                var size = Marshal.SizeOf<ACCELTABLEENTRY>();
                var count = mem.Length / size;
                var accelerators = new AcceleratorEntry[count];

                for (var i = 0; i < count; i++)
                {
                    try
                    {
                        var entry = await stream.ReadStructAsync<ACCELTABLEENTRY>(size).ConfigureAwait(false);
                        var accelerator = new AcceleratorEntry(entry.fFlags, entry.wAnsi, entry.wId);

                        accelerators[i] = accelerator;
                    }
                    catch (Exception ex)
                    {
                        throw new PortableExecutableImageException(Image, "Could not read accelerator entry from stream.", ex);
                    }
                }

                var table = new AcceleratorsTable(this, language, accelerators);

                return table;
            }
        }

        #endregion
    }
}
