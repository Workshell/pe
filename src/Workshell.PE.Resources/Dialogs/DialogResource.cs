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
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Content;
using Workshell.PE.Extensions;

namespace Workshell.PE.Resources.Dialogs
{
    public sealed class DialogResource : Resource
    {
        public DialogResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var typeId = new ResourceId(ResourceType.Dialog);

            return ResourceType.Register<DialogResource>(typeId);
        }

        #endregion

        #region Methods

        public DialogBase GetDialog()
        {
            return GetDialog(ResourceLanguage.English.UnitedStates);
        }

        public async Task<DialogBase> GetDialogAsync()
        {
            return await GetDialogAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public DialogBase GetDialog(ResourceLanguage language)
        {
            return GetDialogAsync(language).GetAwaiter().GetResult();
        }

        public async Task<DialogBase> GetDialogAsync(ResourceLanguage language)
        {
            var buffer = await GetBytesAsync(language).ConfigureAwait(false);

            using (var mem = new MemoryStream(buffer))
            {
                try
                {
                    var version = await mem.ReadUInt16Async().ConfigureAwait(false);
                    var signature = await mem.ReadUInt16Async().ConfigureAwait(false);
                    var isExtended = (version == 1 && signature == 0xFFFF);
                    DialogBase dialog;

                    mem.Seek(0, SeekOrigin.Begin);

                    if (!isExtended)
                    {
                        dialog = new Dialog(this, language);
                    }
                    else
                    {
                        dialog = await DialogEx.CreateAsync(this, language, mem).ConfigureAwait(false);
                    }

                    return dialog;
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(Image, "Could not read dialog from stream.", ex);
                }
            }
        }

        #endregion
    }
}
