using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Workshell.PE.Resources.Dialogs
{
    public sealed class DialogItemEx
    {
        internal DialogItemEx()
        {

        }

        #region Static Methods

        internal static async Task<DialogItemEx> CreateAsync(Stream stream)
        {
            return null;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{Title}, {Id}, {Class}";
        }

        public T GetControlStyle<T>()
        {
            var type = typeof(T);

            if (!type.IsEnum)
                throw new Exception("Not an enum type.");

            return (T)Enum.Parse(typeof(T), Styles.ToString(), true);
        }

        #endregion

        #region Properties

        public uint HelpId { get; private set; }
        public uint ExtendedStyles { get; private set; }
        public uint Styles { get; private set; }
        public Point Position { get; private set; }
        public Size Size { get; private set; }
        public int Id { get; private set; }
        public ResourceId Class { get; private set; }
        public ResourceId Title { get; private set; }
        public byte[] ExtraData { get; private set; }

        #endregion
    }
}
