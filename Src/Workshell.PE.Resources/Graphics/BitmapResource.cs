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

namespace Workshell.PE
{

    public enum BitmapSaveFormat
    {
        Raw,
        Resource,
        Bitmap
    }

    public class BitmapResource
    {

        private Resource resource;
        private uint language_id;
        private byte[] dib;

        internal BitmapResource(Resource sourceResource, uint languageId, byte[] dibData)
        {
            resource = sourceResource;
            language_id = languageId;
            dib = dibData;
        }

        #region Static Methods

        public static BitmapResource Load(Resource resource)
        {
            return Load(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static BitmapResource Load(Resource resource, uint language)
        {
            if (!resource.Languages.Contains(language))
                return null;

            byte[] data = resource.GetBytes(language);

            BitmapResource result = new PE.BitmapResource(resource, language, data);

            return result;
        }

        #endregion

        #region Methods

        public Bitmap ToBitmap()
        {
            MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream();

            using (mem)
            {
                MemoryStream dib_mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream(dib);

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

                Bitmap bitmap = (Bitmap)Image.FromStream(mem);

                return bitmap;
            }
        }

        public void Save(string fileName)
        {
            Save(fileName, BitmapSaveFormat.Bitmap);
        }

        public void Save(Stream stream)
        {
            Save(stream, BitmapSaveFormat.Bitmap);
        }

        public void Save(string fileName, BitmapSaveFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, BitmapSaveFormat format)
        {
            switch (format)
            {
                case BitmapSaveFormat.Raw:
                    SaveRaw(stream);
                    break;
                case BitmapSaveFormat.Resource:
                    SaveResource(stream);
                    break;
                case BitmapSaveFormat.Bitmap:
                    SaveBitmap(stream);
                    break;
            }
        }

        private void SaveRaw(Stream stream)
        {
            byte[] data = resource.GetBytes(language_id);

            stream.Write(data, 0, data.Length);
        }

        private void SaveResource(Stream stream)
        {
            throw new NotImplementedException();
        }

        private void SaveBitmap(Stream stream)
        {
            using (Bitmap bitmap = ToBitmap())
                bitmap.Save(stream, ImageFormat.Bmp);
        }

        #endregion

        #region Properties

        public Resource Resource
        {
            get
            {
                return resource;
            }
        }

        public uint Language
        {
            get
            {
                return language_id;
            }
        }

        public byte[] DIB
        {
            get
            {
                return dib;
            }
        }

        #endregion

    }

}
