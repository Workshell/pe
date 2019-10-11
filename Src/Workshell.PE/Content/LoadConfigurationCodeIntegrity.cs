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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class LoadConfigurationCodeIntegrity
    {
        private readonly LoadConfigurationDirectory _directory;
        private readonly IMAGE_LOAD_CONFIG_CODE_INTEGRITY _codeIntegrity;

        internal LoadConfigurationCodeIntegrity(LoadConfigurationDirectory directory, IMAGE_LOAD_CONFIG_CODE_INTEGRITY codeIntegrity)
        {
            _directory = directory;
            _codeIntegrity = codeIntegrity;
        }

        #region Properties

        public ushort Flags => _codeIntegrity.Flags;
        public ushort Catalog => _codeIntegrity.Catalog;
        public ulong CatalogOffset => _codeIntegrity.CatalogOffset;
        public ulong Reserved => _codeIntegrity.Reserved;

        #endregion
    }
}
