using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Workshell.PE.Extensions;
using Workshell.PE.Resources.Native;

namespace Workshell.PE.Resources.Messages
{
    public sealed class MessageTableResource : Resource
    {
        public MessageTableResource(PortableExecutableImage image, ResourceType type, ResourceDirectoryEntry entry, ResourceId id) : base(image, type, entry, id)
        {
        }

        #region Static Methods

        public static bool Register()
        {
            var type = new ResourceId(ResourceType.MessageTable);

            return ResourceType.Register<MessageTableResource>(type);
        }

        #endregion

        #region Methods

        public MessageTable GetTable()
        {
            return GetTable(ResourceLanguage.English.UnitedStates);
        }

        public async Task<MessageTable> GetTableAsync()
        {
            return await GetTableAsync(ResourceLanguage.English.UnitedStates).ConfigureAwait(false);
        }

        public MessageTable GetTable(ResourceLanguage language)
        {
            return GetTableAsync(language).GetAwaiter().GetResult();
        }

        public async Task<MessageTable> GetTableAsync(ResourceLanguage language)
        {
            var buffer = await GetBytesAsync(language).ConfigureAwait(false);

            using (var mem = new MemoryStream(buffer))
            {
                var tuples = new List<Tuple<uint, uint, uint>>();
                var resourceData = await mem.ReadStructAsync<MESSAGE_RESOURCE_DATA>().ConfigureAwait(false);

                for(var i = 0; i < resourceData.NumberOfBlocks; i++)
                {
                    var block = await mem.ReadStructAsync<MESSAGE_RESOURCE_BLOCK>().ConfigureAwait(false);
                    var tuple = new Tuple<uint, uint, uint>(block.OffsetToEntries, block.LowId, block.HighId);

                    tuples.Add(tuple);
                }

                var blocks = new MessageTableBlock[resourceData.NumberOfBlocks];

                for(var i = 0; i < tuples.Count; i++)
                {
                    var tuple = tuples[i];

                    mem.Seek(tuple.Item1, SeekOrigin.Begin);

                    var count = (tuple.Item3 - tuple.Item2).ToInt32() + 1;
                    var entries = new MessageTableEntry[count];                   
                    var id = tuple.Item2;

                    for (var j = 0; j < count; j++)
                    {
                        var resourceEntry = await mem.ReadStructAsync<MESSAGE_RESOURCE_ENTRY>().ConfigureAwait(false);
                        var stringBuffer = await mem.ReadBytesAsync(resourceEntry.Length - sizeof(uint)).ConfigureAwait(false);
                        var message = (resourceEntry.Flags == 0 ? Encoding.ASCII.GetString(stringBuffer) : Encoding.Unicode.GetString(stringBuffer));
                        var entry = new MessageTableEntry(id, message, resourceEntry.Flags != 0);

                        entries[j] = entry;
                        id++;
                    }

                    var block = new MessageTableBlock(tuple.Item2, tuple.Item3, tuple.Item1, entries);

                    blocks[i] = block;
                }

                var table = new MessageTable(this, language, blocks);

                return table;
            }
        }

        #endregion
    }
}
