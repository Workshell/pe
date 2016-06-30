using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE
{

    public sealed class Resource : ISupportsBytes
    {

        private ResourceLanguage language;
        private ResourceDataEntry data_entry;

        internal Resource(ResourceLanguage resourceLanguage, ResourceDataEntry dataEntry)
        {
            language = resourceLanguage;
            data_entry = dataEntry;
        }

        #region Methods

        public byte[] GetBytes()
        {
            ResourceData data = data_entry.GetData();
            byte[] buffer = data.GetBytes();

            return buffer;
        }

        public void CopyToStream(Stream stream)
        {
            byte[] buffer = GetBytes();

            stream.Write(buffer, 0, buffer.Length);
        }

        #endregion

        #region Properties

        public ResourceLanguage Language
        {
            get
            {
                return language;
            }
        }

        public ResourceDataEntry DataEntry
        {
            get
            {
                return data_entry;
            }
        }

        #endregion

    }

    public sealed class ResourceLanguage
    {

        private ResourceName name;
        private ResourceDirectoryEntry directory_entry;
        private uint id;
        private Resource resource;

        internal ResourceLanguage(ResourceName resourceName, ResourceDirectoryEntry directoryEntry)
        {
            name = resourceName;
            directory_entry = directoryEntry;
            id = directory_entry.GetId();
            resource = LoadResource();
        }

        #region Methods

        public override string ToString()
        {
            return id.ToString();
        }

        private Resource LoadResource()
        {
            ResourceDataEntry data_entry = directory_entry.GetDataEntry();
            Resource result = new PE.Resource(this, data_entry);

            return result;
        }

        #endregion

        #region Properties

        public ResourceName Name
        {
            get
            {
                return name;
            }
        }

        public ResourceDirectoryEntry DirectoryEntry
        {
            get
            {
                return directory_entry;
            }
        }

        public uint Id
        {
            get
            {
                return id;
            }
        }

        public Resource Resource
        {
            get
            {
                return resource;
            }
        }

        #endregion

    }

    public sealed class ResourceName : IEnumerable<ResourceLanguage>
    {

        private ResourceType type;
        private ResourceDirectoryEntry directory_entry;
        private uint id;
        private string name;
        private ResourceLanguage[] languages;

        internal ResourceName(ResourceType resourceType, ResourceDirectoryEntry directoryEntry)
        {
            type = resourceType;
            directory_entry = directoryEntry;

            if (directory_entry.NameType == NameType.ID)
            {
                id = directory_entry.GetId();
                name = null;
            }
            else
            {
                id = 0;
                name = directory_entry.GetName();
            }

            languages = LoadLanguages();
        }

        #region Methods

        public IEnumerator<ResourceLanguage> GetEnumerator()
        {
            for(var i = 0; i < languages.Length; i++)
            {
                ResourceLanguage language = languages[i];

                yield return language;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            if (id != 0)
            {
                return id.ToString();
            }
            else
            {
                return name;
            }
        }

        private ResourceLanguage[] LoadLanguages()
        {
            List<ResourceLanguage> results = new List<ResourceLanguage>();
            ResourceDirectory directory = directory_entry.GetDirectory();

            foreach (ResourceDirectoryEntry entry in directory)
            {
                ResourceLanguage language = new PE.ResourceLanguage(this, entry);

                results.Add(language);
            }

            return results.ToArray();
        }

        #endregion

        #region Properties

        public ResourceType Type
        {
            get
            {
                return type;
            }
        }

        public ResourceDirectoryEntry DirectoryEntry
        {
            get
            {
                return directory_entry;
            }
        }

        public uint Id
        {
            get
            {
                return id;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public int LanguageCount
        {
            get
            {
                return languages.Length;
            }
        }

        public ResourceLanguage this[int index]
        {
            get
            {
                return languages[index];
            }
        }

        #endregion

    }

    public sealed class ResourceType : IEnumerable<ResourceName>
    {

        private ResourceDirectoryEntry directory_entry;
        private uint id;
        private string name;
        private ResourceName[] names;

        internal ResourceType(ResourceDirectoryEntry directoryEntry)
        {
            directory_entry = directoryEntry;

            if (directory_entry.NameType == NameType.ID)
            {
                id = directory_entry.GetId();
                name = null;
            }
            else
            {
                id = 0;
                name = directory_entry.GetName();
            }

            names = LoadResources();
        }

        #region Methods

        public IEnumerator<ResourceName> GetEnumerator()
        {
            for(var i = 0; i < names.Length; i++)
            {
                ResourceName resource_name = names[i];

                yield return resource_name;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            string result;

            if (id == 0)
            {
                result = name;
            }
            else
            {
                string constant = PE.Resources.GetTypeConstant(id);

                result = String.Format("{0} ({1})", id, constant);
            }

            return result;
        }

        private ResourceName[] LoadResources()
        {
            List<ResourceName> results = new List<ResourceName>();
            ResourceDirectory directory = directory_entry.GetDirectory();

            foreach(ResourceDirectoryEntry entry in directory)
            {
                ResourceName resource_name = new PE.ResourceName(this, entry);

                results.Add(resource_name);
            }

            return results.ToArray();
        }

        #endregion

        #region Properties

        public ResourceDirectoryEntry DirectoryEntry
        {
            get
            {
                return directory_entry;
            }
        }

        public uint Id
        {
            get
            {
                return id;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public int NameCount
        {
            get
            {
                return names.Length;
            }
        }

        public ResourceName this[int index]
        {
            get
            {
                return names[index];
            }
        }

        #endregion

    }

    public sealed class Resources : IEnumerable<ResourceType>
    {

        private static Dictionary<uint, Tuple<string, string>> type_info;

        private ResourceType[] types;

        static Resources()
        {
            type_info = new Dictionary<uint, Tuple<string, string>>();

            PopulateTypeInfo();
        }

        internal Resources(ResourceDirectory rootDirectory)
        {
            types = LoadTypes(rootDirectory);
        }

        #region Static Methods

        public static Resources Get(ExecutableImage image)
        {
            ResourceDirectory root_directory = ResourceDirectory.GetRootDirectory(image);

            if (root_directory == null)
                return null;

            Resources result = new Resources(root_directory);

            return result;
        }

        public static string GetTypeName(uint typeId)
        {
            if (!type_info.ContainsKey(typeId))
            {
                return String.Empty;
            }
            else
            {
                return type_info[typeId].Item1;
            }
        }

        public static string GetTypeConstant(uint typeId)
        {
            if (!type_info.ContainsKey(typeId))
            {
                return String.Empty;
            }
            else
            {
                return type_info[typeId].Item2;
            }
        }

        private static void PopulateTypeInfo()
        {
            uint[] known_types = new uint[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 17, 19, 20, 21, 22, 23, 24 };

            foreach (uint id in known_types)
                PopulateTypeInfo(id);
        }

        private static void PopulateTypeInfo(uint typeId)
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
                    name = null;
                    constant_name = null;
                    break;
            }

            if (name == null && constant_name == null)
                return;

            Tuple<string, string> tuple = new Tuple<string, string>(name, constant_name);

            type_info.Add(typeId,tuple);
        }

        #endregion

        #region Methods

        public IEnumerator<ResourceType> GetEnumerator()
        {
            for(var i = 0; i < types.Length; i++)
            {
                ResourceType type = types[i];

                yield return type;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private ResourceType[] LoadTypes(ResourceDirectory rootDirectory)
        {
            List<ResourceType> results = new List<ResourceType>();

            foreach(ResourceDirectoryEntry entry in rootDirectory)
            {
                ResourceType type = new ResourceType(entry);

                results.Add(type);
            }

            return results.ToArray();
        }

        #endregion

        #region Properties

        public int TypeCount
        {
            get
            {
                return types.Length;
            }
        }

        public ResourceType this[int index]
        {
            get
            {
                return types[index];
            }
        }

        #endregion

    }

}
