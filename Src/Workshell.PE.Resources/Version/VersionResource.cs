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
using Workshell.PE.Native;

namespace Workshell.PE.Resources
{

    public enum VersionSaveFormat
    {
        Raw,
        Resource
    }

    public sealed class VersionResource
    {

        private Resource resource;
        private uint language_id;
        private FixedFileInfo fixed_file_info;
        private StringFileInfo string_file_info;
        private VarFileInfo var_file_info;

        internal VersionResource(Resource sourceResource, uint languageId, byte[] data)
        {
            resource = sourceResource;
            language_id = languageId;

            MemoryStream mem = resource.Type.Resources.Image.MemoryStreamProvider.GetStream(data);

            using (mem)
            {
                ushort len = Utils.ReadUInt16(mem);
                ushort val_len = Utils.ReadUInt16(mem);
                ushort type = Utils.ReadUInt16(mem);
                string key = Utils.ReadUnicodeString(mem);

                if (mem.Position % 4 != 0)
                    Utils.ReadUInt16(mem);

                fixed_file_info = new FixedFileInfo(mem);

                if (mem.Position % 4 != 0)
                    Utils.ReadUInt16(mem);

                string_file_info = new StringFileInfo(this, mem);

                if (mem.Position % 4 != 0)
                    Utils.ReadUInt16(mem);

                var_file_info = new VarFileInfo(this, mem);
            }
        }

        #region Static Methods

        public static VersionResource Load(Resource resource)
        {
            return Load(resource, Resource.DEFAULT_LANGUAGE);
        }

        public static VersionResource Load(Resource resource, uint language)
        {
            if (!resource.Languages.Contains(language))
                return null;

            if (resource.Type.Id != ResourceType.RT_VERSION)
                return null;

            byte[] data = resource.GetBytes(language);
            VersionResource result = new VersionResource(resource, language, data);

            return result;
        }

        #endregion

        #region Methods

        public void Save(string fileName)
        {
            Save(fileName, VersionSaveFormat.Raw);
        }

        public void Save(Stream stream)
        {
            Save(stream, VersionSaveFormat.Raw);
        }

        public void Save(string fileName, VersionSaveFormat format)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, format);
                file.Flush();
            }
        }

        public void Save(Stream stream, VersionSaveFormat format)
        {
            switch (format)
            {
                case VersionSaveFormat.Raw:
                    SaveRaw(stream);
                    break;
                case VersionSaveFormat.Resource:
                    SaveResource(stream);
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

        public FixedFileInfo Fixed
        {
            get
            {
                return fixed_file_info;
            }
        }

        public StringFileInfo Strings
        {
            get
            {
                return string_file_info;
            }
        }

        public VarFileInfo Vars
        {
            get
            {
                return var_file_info;
            }
        }

        #endregion

    }

}
