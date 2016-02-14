using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE
{

    public sealed class RelocationBlocks : IEnumerable<RelocationBlock>, IReadOnlyCollection<RelocationBlock>
    {

        private RelocationTableContent content;
        private Location location;
        private RelocationBlock[] blocks;

        internal RelocationBlocks(RelocationTableContent relocContent, Location relocLocation, List<Tuple<ulong,uint,uint,ushort[]>> relocBlocks, ulong imageBase)
        {
            content = relocContent;
            location = relocLocation;
            blocks = new RelocationBlock[0];

            Load(relocBlocks,imageBase);
        }

        #region Methods

        public IEnumerator<RelocationBlock> GetEnumerator()
        {
            return blocks.Cast<RelocationBlock>().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void Load(List<Tuple<ulong,uint,uint,ushort[]>> relocBlocks, ulong imageBase)
        {
            List<RelocationBlock> list = new List<RelocationBlock>();
            LocationCalculator calc = content.DataDirectory.Directories.Reader.GetCalculator();

            foreach(Tuple<ulong,uint,uint,ushort[]> tuple in relocBlocks)
            {
                ulong offset = tuple.Item1;
                uint rva = calc.OffsetToRVA(content.Section,offset);
                ulong va = imageBase + rva;
                uint size = tuple.Item2;
                uint page_rva = tuple.Item3;
                ushort[] relocs = tuple.Item4;
                Location block_location = new Location(offset,rva,va,size,size);
                RelocationBlock block = new RelocationBlock(this,block_location,page_rva,size,relocs);

                list.Add(block);
            }

            blocks = list.ToArray();
        }

        #endregion

        #region Properties

        public RelocationTableContent Content
        {
            get
            {
                return content;
            }
        }

        public Location Location
        {
            get
            {
                return location;
            }
        }

        public int Count
        {
            get
            {
                return blocks.Length;
            }
        }

        public RelocationBlock this[int index]
        {
            get
            {
                return blocks[index];
            }
        }

        #endregion

    }

}
