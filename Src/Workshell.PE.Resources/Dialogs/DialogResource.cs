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
using System.Collections;
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

    public sealed class DialogResource : Resource
    {

        public DialogResource(ResourceType owningType, ResourceDirectoryEntry directoryEntry) : base(owningType, directoryEntry)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            ResourceId resource_type = new ResourceId(ResourceType.RT_DIALOG);

            return ResourceType.Register(resource_type, typeof(DialogResource));
        }

        #endregion

        #region Methods

        public Dialog GetDialog(uint languageId)
        {
            byte[] data = GetBytes(languageId);

            using (MemoryStream mem = new MemoryStream(data))
            {
                ushort ver = Utils.ReadUInt16(mem);
                ushort sig = Utils.ReadUInt16(mem);

                bool is_extended = (ver == 1 && sig == 0xFFFF);

                mem.Seek(0, SeekOrigin.Begin);

                if (!is_extended)
                {
                    Dialog dialog = new Dialog(this, languageId);

                    return dialog;
                }
                else
                {
                    return null;
                }
            }
        }

        public DialogEx GetDialogEx(uint languageId)
        {
            byte[] data = GetBytes(languageId);

            using (MemoryStream mem = new MemoryStream(data))
            {
                ushort ver = Utils.ReadUInt16(mem);
                ushort sig = Utils.ReadUInt16(mem);

                bool is_extended = (ver == 1 && sig == 0xFFFF);

                mem.Seek(0, SeekOrigin.Begin);

                if (!is_extended)
                {
                    return null;
                }
                else
                {
                    DialogEx dialog = new DialogEx(this, languageId, mem);

                    return dialog;
                }
            }
        }

        #endregion

    }

}
