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

using Workshell.PE.Annotations;
using Workshell.PE.Content.Exceptions;
using Workshell.PE.Extensions;
using Workshell.PE.Native;

namespace Workshell.PE.Content
{
    public sealed class ExceptionTableEntry64 : ExceptionTableEntry
    {
        private ExceptionUnwindInfo _unwindInfo;

        internal ExceptionTableEntry64(PortableExecutableImage image, Location location, IMAGE_RUNTIME_FUNCTION_64 function) : base(image, location)
        {
            _unwindInfo = null;

            StartAddress = function.StartAddress;
            EndAddress = function.EndAddress;
            UnwindInfoAddress = function.UnwindInfoAddress;
        }

        #region Methods

        public override string ToString()
        {
            return $"Start Address: 0x{StartAddress:X8}, End Address: 0x{EndAddress:X8}, Unwind Info Address: 0x{UnwindInfoAddress:X8}";
        }

        public ExceptionUnwindInfo GetUnwindInfo()
        {
            return GetUnwindInfoAsync().GetAwaiter().GetResult();
        }

        public async Task<ExceptionUnwindInfo> GetUnwindInfoAsync()
        {
            if (_unwindInfo == null && UnwindInfoAddress > 0)
            {
                var calc = Image.GetCalculator();
                var stream = Image.GetStream();

                try
                {
                    var offset = calc.RVAToOffset(UnwindInfoAddress);

                    stream.Seek(offset, SeekOrigin.Begin);

                    var versionFlags = await stream.ReadByteAsync().ConfigureAwait(false);
                    var sizeOfProlog = await stream.ReadByteAsync().ConfigureAwait(false);
                    var countOfCodes = await stream.ReadByteAsync().ConfigureAwait(false);
                    var frameRegisterOffset = await stream.ReadByteAsync().ConfigureAwait(false);
                    var codes = new List<ushort>();

                    for (var i = 0; i < countOfCodes; i++)
                    {
                        var code = await stream.ReadUInt16Async().ConfigureAwait(false);

                        codes.Add(code);
                    }

                    if (countOfCodes % 2 != 0)
                        await stream.SkipBytesAsync(sizeof(ushort)).ConfigureAwait(false);

                    var flags = GetFlags(versionFlags);
                    uint handlerAddress = 0;
                    ExceptionChainedUnwindInfo chainedInfo = null;

                    if ((flags & ExceptionUnwindInfoFlags.ExceptionHandler) == ExceptionUnwindInfoFlags.ExceptionHandler || (flags & ExceptionUnwindInfoFlags.UnwindHandler) == ExceptionUnwindInfoFlags.UnwindHandler)
                        handlerAddress = await stream.ReadUInt32Async().ConfigureAwait(false);

                    if ((flags & ExceptionUnwindInfoFlags.ChainHandler) == ExceptionUnwindInfoFlags.ChainHandler)
                    {
                        var chainedFunction = await stream.ReadStructAsync<IMAGE_RUNTIME_FUNCTION_64>();

                        if (chainedFunction.StartAddress != 0)
                            chainedInfo = new ExceptionChainedUnwindInfo(chainedFunction.StartAddress, chainedFunction.EndAddress, chainedFunction.UnwindInfoAddress);
                    }

                    _unwindInfo = new ExceptionUnwindInfo(Image, UnwindInfoAddress, versionFlags, sizeOfProlog, countOfCodes, frameRegisterOffset, codes.ToArray(), handlerAddress, chainedInfo);
                }
                catch (Exception ex)
                {
                    throw new PortableExecutableImageException(Image, "Could not read exception unwind information from stream.", ex);
                }
            }

            return _unwindInfo;
        }

        private ExceptionUnwindInfoFlags GetFlags(byte versionFlags)
        {
            var flags = Convert.ToByte(versionFlags >> 3 & ( (1 << 5) -1));

            return (ExceptionUnwindInfoFlags)flags;
        }

        #endregion

        #region Properties

        [FieldAnnotation("Start Address", Order = 1)]
        public uint StartAddress { get; }

        [FieldAnnotation("End Address", Order = 2)]
        public uint EndAddress { get; }

        [FieldAnnotation("Unwind Info Address", Order = 3)]
        public uint UnwindInfoAddress { get; }

        #endregion
    }
}
