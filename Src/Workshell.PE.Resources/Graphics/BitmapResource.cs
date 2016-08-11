#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources
{

    public enum BitmapSaveFormat
    {
        Raw,
        Bitmap
    }

    public sealed class BitmapResource : Resource
    {

        public BitmapResource(ResourceType owningType, ResourceDirectoryEntry directoryEntry) : base(owningType, directoryEntry)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            ResourceId resource_type = new ResourceId(ResourceType.RT_BITMAP);

            return ResourceType.Register(resource_type, typeof(BitmapResource));
        }

        #endregion

        #region Methods

        public Bitmap ToBitmap()
        {
            return ToBitmap(DEFAULT_LANGUAGE);
        }

        public Bitmap ToBitmap(uint languageId)
        {
            MemoryStream mem = new MemoryStream();

            using (mem)
            {
                byte[] dib = GetBytes(languageId);
                MemoryStream dib_mem = new MemoryStream(dib);

                using (dib_mem)
                {
                    BITMAPINFOHEADER header = Utils.Read<BITMAPINFOHEADER>(dib_mem);
                    BITMAPFILEHEADER file_header = new BITMAPFILEHEADER();

                    file_header.Tag = 19778;
                    file_header.Size = (Utils.SizeOf<BITMAPFILEHEADER>() + dib.Length).ToUInt32();
                    file_header.Reserved1 = 0;
                    file_header.Reserved2 = 0;
                    file_header.BitmapOffset = Utils.SizeOf<BITMAPFILEHEADER>().ToUInt32() + header.biSize;

                    Utils.Write<BITMAPFILEHEADER>(file_header, mem);
                    Utils.Write(dib, mem);
                }

                mem.Seek(0, SeekOrigin.Begin);

                using (Bitmap bitmap = (Bitmap)Image.FromStream(mem))
                {
                    Bitmap result = new Bitmap(bitmap); // Create a copy of the image so we can release the stream

                    return result;
                }
            }
        }

        public void Save(string fileName, uint languageId, BitmapSaveFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, languageId, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, uint languageId, BitmapSaveFormat format)
        {
            if (format == BitmapSaveFormat.Raw)
            {
                Save(stream, languageId);
            }
            else
            {
                using (Bitmap bitmap = ToBitmap(languageId))
                {
                    bitmap.Save(stream, ImageFormat.Bmp);
                }
            }
        }

        #endregion

    }

}
