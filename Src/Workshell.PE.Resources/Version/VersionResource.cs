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

    public sealed class VersionInfo
    {

        private VersionResource resource;
        private uint language_id;
        private FixedFileInfo fixed_file_info;
        private StringFileInfo string_file_info;
        private VarFileInfo var_file_info;

        internal VersionInfo(VersionResource versionResource, uint languageId, FixedFileInfo fixedInfo, StringFileInfo stringInfo, VarFileInfo varInfo)
        {
            resource = versionResource;
            language_id = languageId;
            fixed_file_info = fixedInfo;
            string_file_info = stringInfo;
            var_file_info = varInfo;
        }

        #region Properties

        public VersionResource Resource
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

        public VarFileInfo Var
        {
            get
            {
                return var_file_info;
            }
        }

        #endregion

    }

    public sealed class VersionResource : Resource
    {

        public VersionResource(ResourceType owningType, ResourceDirectoryEntry directoryEntry) : base(owningType, directoryEntry)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            ResourceId resource_type = new ResourceId(ResourceType.RT_VERSION);

            return ResourceType.Register(resource_type, typeof(VersionResource));
        }

        #endregion

        #region Methods

        public VersionInfo GetInfo()
        {
            return GetInfo(DEFAULT_LANGUAGE);
        }

        public VersionInfo GetInfo(uint languageId)
        {
            byte[] data = GetBytes(languageId);

            using (MemoryStream mem = new MemoryStream(data))
            {
                ushort len = Utils.ReadUInt16(mem);
                ushort val_len = Utils.ReadUInt16(mem);
                ushort type = Utils.ReadUInt16(mem);
                string key = Utils.ReadUnicodeString(mem);

                if (mem.Position % 4 != 0)
                    Utils.ReadUInt16(mem);

                FixedFileInfo fixed_file_info = new FixedFileInfo(mem);

                if (mem.Position % 4 != 0)
                    Utils.ReadUInt16(mem);

                StringFileInfo string_file_info = new StringFileInfo(this, mem);

                if (mem.Position % 4 != 0)
                    Utils.ReadUInt16(mem);

                VarFileInfo var_file_info = new VarFileInfo(this, mem);

                VersionInfo info = new VersionInfo(this, languageId, fixed_file_info, string_file_info, var_file_info);

                return info;
            }
        }

        #endregion

    }

}
