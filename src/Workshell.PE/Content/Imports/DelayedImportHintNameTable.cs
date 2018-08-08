using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;

namespace Workshell.PE.Content
{
    public sealed class DelayedImportHintNameTable : ImportHintNameTableBase<DelayedImportHintNameEntry>
    {
        internal DelayedImportHintNameTable(PortableExecutableImage image, DataDirectory dataDirectory, Location location, Tuple<ulong, uint, ushort, string, bool>[] entries) : base(image, dataDirectory, location, entries, true)
        {
        }

        #region Static Methods

        public static async Task<DelayedImportHintNameTable> GetAsync(PortableExecutableImage image, DelayedImportDirectory directory = null)
        {
            if (directory == null)
                directory = await DelayedImportDirectory.GetAsync(image).ConfigureAwait(false);

            var entries = new Dictionary<uint, Tuple<ulong, uint, ushort, string, bool>>();
            var ilt = await DelayedImportAddressTables.GetLookupTableAsync(image, directory).ConfigureAwait(false);
            var calc = image.GetCalculator();
            var stream = image.GetStream();

            foreach (var table in ilt)
            {
                foreach (var entry in table)
                {
                    if (entry.Address == 0)
                        continue;

                    if (entries.ContainsKey(entry.Address))
                        continue;

                    if (!entry.IsOrdinal)
                    {
                        var offset = calc.RVAToOffset(entry.Address);
                        var size = 0u;
                        var isPadded = false;
                        ushort hint = 0;
                        var name = new StringBuilder(256);

                        stream.Seek(offset.ToInt64(), SeekOrigin.Begin);

                        hint = await stream.ReadUInt16Async().ConfigureAwait(false);
                        size += sizeof(ushort);

                        while (true)
                        {
                            var b = await stream.ReadByteAsync().ConfigureAwait(false);

                            size++;

                            if (b <= 0)
                                break;

                            name.Append((char)b);
                        }

                        if (size % 2 != 0)
                        {
                            isPadded = true;
                            size++;
                        }

                        var tuple = new Tuple<ulong, uint, ushort, string, bool>(offset, size, hint, name.ToString(), isPadded);

                        entries.Add(entry.Address, tuple);
                    }
                }
            }

            Location location;

            if (entries.Count > 0)
            {
                var firstEntry = entries.Values.MinBy(tuple => tuple.Item1);
                var lastEntry = entries.Values.MaxBy(tuple => tuple.Item1);
                var tableOffset = firstEntry.Item1;
                var tableSize = ((lastEntry.Item1 + lastEntry.Item2) - tableOffset).ToUInt32();
                var tableRVA = calc.OffsetToRVA(tableOffset);
                var tableVA = image.NTHeaders.OptionalHeader.ImageBase + tableRVA;
                var tableSection = calc.RVAToSection(tableRVA);

                location = new Location(tableOffset, tableRVA, tableVA, tableSize, tableSize, tableSection);
            }
            else
            {
                location = new Location(0, 0, 0, 0, 0, null);
            }

            var result = new DelayedImportHintNameTable(image, directory.DataDirectory, location, entries.Values.ToArray());

            return result;
        }

        #endregion
    }
}
