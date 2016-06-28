using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class Resources : ExecutableImageContent, ISupportsBytes
    {

        private static Dictionary<uint, Tuple<string, string>> type_names;

        private ResourceDirectory root_directory;

        static Resources()
        {
            type_names = new Dictionary<uint, Tuple<string, string>>();

            PopulateTypeNames();
        }

        internal Resources(DataDirectory dataDirectory, Location dataLocation, ulong rootOffset) : base(dataDirectory, dataLocation)
        {
            root_directory = new ResourceDirectory(this, rootOffset, null);
        }

        #region Static Methods

        public static Resources Get(ExecutableImage image)
        {
            if (!image.NTHeaders.DataDirectories.Exists(DataDirectoryType.ResourceTable))
                return null;

            DataDirectory directory = image.NTHeaders.DataDirectories[DataDirectoryType.ResourceTable];

            if (DataDirectory.IsNullOrEmpty(directory))
                return null;

            LocationCalculator calc = directory.Directories.Image.GetCalculator();
            Section section = calc.RVAToSection(directory.VirtualAddress);
            ulong file_offset = calc.RVAToOffset(section, directory.VirtualAddress);
            ulong image_base = directory.Directories.Image.NTHeaders.OptionalHeader.ImageBase;
            Location location = new Location(file_offset, directory.VirtualAddress, image_base + directory.VirtualAddress, directory.Size, directory.Size, section);
            Resources result = new Resources(directory, location, file_offset);

            return result;
        }

        public static string GetTypeName(uint typeId)
        {
            if (!type_names.ContainsKey(typeId))
            {
                return String.Empty;
            }
            else
            {
                return type_names[typeId].Item1;
            }
        }

        public static string GetTypeConstant(uint typeId)
        {
            if (!type_names.ContainsKey(typeId))
            {
                return String.Empty;
            }
            else
            {
                return type_names[typeId].Item2;
            }
        }

        private static void PopulateTypeNames()
        {
            uint[] known_types = new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 17, 19, 20, 21, 22, 23, 24 };

            foreach (uint id in known_types)
                PopulateTypeName(id);
        }

        private static void PopulateTypeName(uint typeId)
        {
            string name;
            string constant_name;

            switch (typeId)
            {
                case 1:
                    name = "Cursor";
                    constant_name = "RT_CURSOR";
                    break;
                case 2:
                    name = "Bitmap";
                    constant_name = "RT_BITMAP";
                    break;
                case 3:
                    name = "Icon";
                    constant_name = "RT_ICON";
                    break;
                case 4:
                    name = "Menu";
                    constant_name = "RT_MENU";
                    break;
                case 5:
                    name = "Dialog";
                    constant_name = "RT_DIALOG";
                    break;
                case 6:
                    name = "String";
                    constant_name = "RT_STRING";
                    break;
                case 7:
                    name = "Font Directory";
                    constant_name = "RT_FONTDIR";
                    break;
                case 8:
                    name = "Font";
                    constant_name = "RT_FONT";
                    break;
                case 9:
                    name = "Accelerator";
                    constant_name = "RT_ACCELERATOR";
                    break;
                case 10:
                    name = "RC Data";
                    constant_name = "RT_RCDATA";
                    break;
                case 11:
                    name = "Message Table";
                    constant_name = "RT_MESSAGETABLE";
                    break;
                case 12:
                    name = "Cursor Group";
                    constant_name = "RT_GROUP_CURSOR";
                    break;
                case 14:
                    name = "Icon Group";
                    constant_name = "RT_GROUP_ICON";
                    break;
                case 16:
                    name = "Version";
                    constant_name = "RT_VERSION";
                    break;
                case 17:
                    name = "Dialog Include";
                    constant_name = "RT_DLGINCLUDE";
                    break;
                case 19:
                    name = "Plug & Play";
                    constant_name = "RT_PLUGPLAY";
                    break;
                case 20:
                    name = "VxD";
                    constant_name = "RT_VXD";
                    break;
                case 21:
                    name = "Animated Cursor";
                    constant_name = "RT_ANICURSOR";
                    break;
                case 22:
                    name = "Animated Icon";
                    constant_name = "RT_ANIICON";
                    break;
                case 23:
                    name = "HTML";
                    constant_name = "RT_HTML";
                    break;
                case 24:
                    name = "Manifest";
                    constant_name = "RT_MANIFEST";
                    break;
                default:
                    name = String.Format("Type {0}", typeId);
                    constant_name = String.Format("MAKEINTRESOUCE({0})", typeId);
                    break;
            }

            Tuple<string, string> tuple = new Tuple<string, string>(name, constant_name);

            type_names.Add(typeId,tuple);
        }

        #endregion

        #region Methods

        public byte[] GetBytes()
        {
            Stream stream = DataDirectory.Directories.Image.GetStream();
            byte[] buffer = Utils.ReadBytes(stream, Location);

            return buffer;
        }

        #endregion

        #region Properties

        public ResourceDirectory Root
        {
            get
            {
                return root_directory;
            }
        }

        #endregion

    }

}
