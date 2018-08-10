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

        public DialogBase GetDialog(uint language = DefaultLanguage)
        {
            return GetDialogAsync(language).GetAwaiter().GetResult();
        }

        public async Task<DialogBase> GetDialogAsync(uint language = DefaultLanguage)
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
