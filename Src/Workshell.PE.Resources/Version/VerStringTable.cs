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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Extensions;

namespace Workshell.PE.Resources
{

    public sealed class VerStringTable : IEnumerable<VerString>
    {

        private ushort len;
        private ushort val_len;
        private ushort type;
        private string key;
        private List<VerString> strings;

        internal VerStringTable(Stream stream)
        {
            int count = 0;

            len = Utils.ReadUInt16(stream);
            val_len = Utils.ReadUInt16(stream);
            type = Utils.ReadUInt16(stream);

            count += 3 * sizeof(ushort);

            key = Utils.ReadUnicodeString(stream);

            count += (key.Length + 1) * sizeof(ushort);

            if (stream.Position % 4 != 0)
            {
                Utils.ReadUInt16(stream);

                count += sizeof(ushort);
            }

            strings = new List<VerString>();

            byte[] buffer = Utils.ReadBytes(stream, len - count);

            using (MemoryStream mem = new MemoryStream(buffer))
            {
                while (mem.Position < mem.Length)
                {
                    VerString str = new VerString(mem);

                    strings.Add(str);

                    if (mem.Position % 4 != 0)
                        Utils.ReadUInt16(mem);
                }
            }
        }

        #region Methods

        public IEnumerator<VerString> GetEnumerator()
        {
            for (var i = 0; i < strings.Count; i++)
                yield return strings[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return String.Format("Count: {0:n0}", strings.Count);
        }

        #endregion

        #region Properties

        public string Key
        {
            get
            {
                return key;
            }
        }

        public int Count
        {
            get
            {
                return strings.Count;
            }
        }

        public VerString this[int index]
        {
            get
            {
                return strings[index];
            }
        }

        public VerString this[string key]
        {
            get
            {
                VerString str = strings.FirstOrDefault(s => String.Compare(key, s.Key, true) == 0);

                return str;
            }
        }

        #endregion

    }

}
