using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Annotations;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content.Exceptions
{
    [Flags]
    public enum ExceptionUnwindInfoFlags : byte
    {
        ExceptionHandler = 0x01,
        UnwindHandler = 0x02,
        ChainHandler = 0x04
    }

    public sealed class ExceptionUnwindInfo : ISupportsLocation, ISupportsBytes
    {
        private readonly PortableExecutableImage _image;

        internal ExceptionUnwindInfo(PortableExecutableImage image, uint rva, byte versionFlags, byte sizeOfProlog, byte countOfCodes, byte frameRegisterOffset, ushort[] codes, 
            uint handlerAddress, ExceptionChainedUnwindInfo chainedInfo)
        {
            _image = image;

            Version = Convert.ToByte(versionFlags & ((1 << 3) - 1));
            Flags = Convert.ToByte(versionFlags >> 3 & ( (1 << 5) -1));
            SizeOfProlog = sizeOfProlog;
            CountOfCodes = countOfCodes;
            FrameRegister = Convert.ToByte(versionFlags & ((1 << 4) - 1));
            FrameOffset = Convert.ToByte(versionFlags >> 4 & ((1 << 4) - 1));
            UnwindCodes = codes.Select(code => new ExceptionUnwindInfoCode(code)).ToArray();

            var calc = image.GetCalculator();
            var offset = calc.RVAToOffset(rva);
            var va = image.NTHeaders.OptionalHeader.ImageBase + rva;
            var size = sizeof(uint) + (sizeof(ushort) * CountOfCodes);

            if (CountOfCodes % 2 != 0)
                size += sizeof(ushort);

            var flags = GetFlags();

            if ((flags & ExceptionUnwindInfoFlags.ExceptionHandler) == ExceptionUnwindInfoFlags.ExceptionHandler || (flags & ExceptionUnwindInfoFlags.UnwindHandler) == ExceptionUnwindInfoFlags.UnwindHandler)
            {
                AddressOfHandler = handlerAddress;

                size += sizeof(uint);
            }

            if ((flags & ExceptionUnwindInfoFlags.ChainHandler) == ExceptionUnwindInfoFlags.ChainHandler)
            {
                ChainedUnwindInfo = chainedInfo;

                size += (sizeof(uint) * 3);
            }

            Location = new Location(calc, offset, rva, va, size.ToUInt32(), size.ToUInt32());
        }

        #region Methods

        public byte[] GetBytes()
        {
            return GetBytesAsync().GetAwaiter().GetResult();
        }

        public async Task<byte[]> GetBytesAsync()
        {
            var stream = _image.GetStream();
            var buffer = await stream.ReadBytesAsync(Location).ConfigureAwait(false);

            return buffer;
        }

        public ExceptionUnwindInfoFlags GetFlags()
        {
            return (ExceptionUnwindInfoFlags)Flags;
        }

        #endregion

        #region Properties

        public Location Location { get; }

        public byte Version { get; }
        public byte Flags { get; }
        public byte SizeOfProlog { get; }
        public byte CountOfCodes { get; }
        public byte FrameRegister { get; }
        public byte FrameOffset { get; }
        public ExceptionUnwindInfoCode[] UnwindCodes { get; }

        public uint AddressOfHandler { get; }
        public ExceptionChainedUnwindInfo ChainedUnwindInfo { get; }

        #endregion
    }
}
