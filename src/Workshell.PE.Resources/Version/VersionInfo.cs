﻿#region License
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

namespace Workshell.PE.Resources.Version
{
    public sealed class VersionInfo
    {
        private readonly IReadOnlyCollection<StringFileInfo> _strings;
        private readonly IReadOnlyCollection<VarFileInfo> _variables;

        internal VersionInfo(VersionResource resource, uint languageId, FixedFileInfo fixedInfo, IReadOnlyCollection<StringFileInfo> stringInfo, IReadOnlyCollection<VarFileInfo> varInfo)
        {
            Resource = resource;
            Language = languageId;
            Fixed = fixedInfo;
            _strings = stringInfo;
            _variables = varInfo;
        }

        #region Properties

        public VersionResource Resource { get; }
        public ResourceLanguage Language { get; }
        public FixedFileInfo Fixed { get; }
        public StringFileInfo Strings => _strings.FirstOrDefault();
        public VarFileInfo Variables => _variables.FirstOrDefault();

        #endregion
    }
}
