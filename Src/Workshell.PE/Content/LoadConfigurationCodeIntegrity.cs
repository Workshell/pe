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
