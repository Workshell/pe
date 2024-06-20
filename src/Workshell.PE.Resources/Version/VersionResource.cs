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

using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Version
{
    public sealed class VersionResource : Resource
    {
        public VersionResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var type = new ResourceId(ResourceType.Version);

            return ResourceType.Register<VersionResource>(type);
        }

        internal static async Task<int> AlignWordBoundaryAsync(Stream stream)
        {
            var count = 0;

            while (stream.Position % 4 != 0)
            {
                await stream.ReadUInt16Async()
                    .ConfigureAwait(false);

                count += sizeof(ushort);
            }

            return count;
        }

        internal static async Task<FileInfo> LoadBaseInfoAsync(Stream stream)
        {
            var len = await stream.ReadUInt16Async()
                .ConfigureAwait(false);
            var valLen = await stream.ReadUInt16Async()
                .ConfigureAwait(false);
            var type = await stream.ReadUInt16Async()
                .ConfigureAwait(false);
            var key = await stream.ReadUnicodeStringAsync()
                .ConfigureAwait(false);

            return new FileInfo()
            {
                Length = len,
                ValueLength = valLen,
                Type = type,
                Key = key
            };
        }

        #endregion

        #region Methods

        public VersionInfo GetInfo()
        {
            return GetInfo(ResourceLanguage.English.UnitedStates);
        }

        public async Task<VersionInfo> GetInfoAsync()
        {
            return await GetInfoAsync(ResourceLanguage.English.UnitedStates)
                .ConfigureAwait(false);
        }

        public VersionInfo GetInfo(ResourceLanguage language)
        {
            return GetInfoAsync(language)
                .GetAwaiter()
                .GetResult();
        }

        public async Task<VersionInfo> GetInfoAsync(ResourceLanguage language)
        {
            var buffer = await GetBytesAsync(language)
                .ConfigureAwait(false);

            using (var mem = new MemoryStream(buffer))
            {
                var count = 3 * sizeof(ushort);

                await mem.ReadBytesAsync(count)
                    .ConfigureAwait(false);

                var key = await mem.ReadUnicodeStringAsync()
                    .ConfigureAwait(false);

                if (key != "VS_VERSION_INFO")
                {
                    throw new Exception("Invalid file version information.");
                }

                await AlignWordBoundaryAsync(mem).ConfigureAwait(false);

                var ffiData = await mem.ReadStructAsync<VS_FIXEDFILEINFO>()
                    .ConfigureAwait(false);
                var fixedFileInfo = new FixedFileInfo(ffiData);

                await AlignWordBoundaryAsync(mem)
                    .ConfigureAwait(false);

                var stringFileInfos = new List<StringFileInfo>();
                var varFileInfos = new List<VarFileInfo>();

                while ((mem.Position + count) < mem.Length)
                {
                    var baseInfo = await LoadBaseInfoAsync(mem)
                        .ConfigureAwait(false);

                    if (baseInfo.Key == "StringFileInfo")
                    {
                        var stringFileInfo = await StringFileInfo.LoadAsync(mem, baseInfo.Length, baseInfo.ValueLength, baseInfo.Type, baseInfo.Key)
                            .ConfigureAwait(false);

                        stringFileInfos.Add(stringFileInfo);
                    }
                    else if (baseInfo.Key == "VarFileInfo")
                    {
                        var varFileInfo = await VarFileInfo.LoadAsync(mem, baseInfo.Length, baseInfo.ValueLength, baseInfo.Type, baseInfo.Key)
                            .ConfigureAwait(false);

                        varFileInfos.Add(varFileInfo);
                    }
                    else
                    {
                        throw new Exception("Unknown type - " + baseInfo.Key);
                    }

                    await AlignWordBoundaryAsync(mem)
                        .ConfigureAwait(false);
                }

                var info = new VersionInfo(this, language, fixedFileInfo, stringFileInfos, varFileInfos);

                return info;
            }
        }

        #endregion
    }
}
