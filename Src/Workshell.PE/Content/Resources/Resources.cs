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

    public sealed class Resource
    {

        public const uint DEFAULT_LANGUAGE = 1033;

        private ResourceType type;
        private ResourceDirectoryEntry directory_entry;
        private uint id;
        private string name;
        private Dictionary<uint, ResourceDirectoryEntry> languages;

        internal Resource(ResourceType owningType, ResourceDirectoryEntry directoryEntry)
        {
            type = owningType;
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

        public ResourceData ToData()
        {
            return ToData(DEFAULT_LANGUAGE);
        }

        public ResourceData ToData(uint languageId)
        {
            if (!languages.ContainsKey(languageId))
            {
                return null;
            }
            else
            {
                ResourceDirectoryEntry language_entry = languages[languageId];
                ResourceDataEntry data_entry = language_entry.GetDataEntry();
                ResourceData data = data_entry.GetData();

                return data;
            }
        }

        public byte[] ToBytes()
        {
            return ToBytes(DEFAULT_LANGUAGE);
        }

        public byte[] ToBytes(uint languageId)
        {
            if (!languages.ContainsKey(languageId))
            {
                return new byte[0];
            }
            else
            {
                ResourceDirectoryEntry language_entry = languages[languageId];
                ResourceDataEntry data_entry = language_entry.GetDataEntry();
                ResourceData data = data_entry.GetData();

                return data.GetBytes();
            }
        }

        public void ToStream(Stream stream)
        {
            ToStream(DEFAULT_LANGUAGE, stream);
        }

        public void ToStream(uint languageId, Stream stream)
        {
            byte[] data = ToBytes(languageId);

            stream.Write(data, 0, data.Length);
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

        private Dictionary<uint, ResourceDirectoryEntry> LoadLanguages()
        {
            Dictionary<uint, ResourceDirectoryEntry> results = new Dictionary<uint, ResourceDirectoryEntry>();
            ResourceDirectory directory = directory_entry.GetDirectory();

            foreach (ResourceDirectoryEntry entry in directory)
            {
                results.Add(entry.GetId(),entry);
            }

            return results;
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

        public uint[] Languages
        {
            get
            {
                return languages.Keys.ToArray();
            }
        }

        #endregion

    }

    public sealed class ResourceType : IEnumerable<Resource>
    {

        public const ushort RT_CURSOR = 1;
        public const ushort RT_BITMAP = 2;
        public const ushort RT_ICON = 3;
        public const ushort RT_MENU = 4;
        public const ushort RT_DIALOG = 5;
        public const ushort RT_STRING = 6;
        public const ushort RT_FONTDIR = 7;
        public const ushort RT_FONT = 8;
        public const ushort RT_ACCELERATOR = 9;
        public const ushort RT_RCDATA = 10;
        public const ushort RT_MESSAGETABLE = 11;
        public const ushort RT_GROUP_CURSOR = 12;
        public const ushort RT_GROUP_ICON = 14;
        public const ushort RT_VERSION = 16;
        public const ushort RT_DLGINCLUDE = 17;
        public const ushort RT_PLUGPLAY = 19;
        public const ushort RT_VXD = 20;
        public const ushort RT_ANICURSOR = 21;
        public const ushort RT_ANIICON = 22;
        public const ushort RT_HTML = 23;
        public const ushort RT_MANIFEST = 24;

        private Resources resources;
        private ResourceDirectoryEntry directory_entry;
        private uint id;
        private string name;
        private Resource[] resource_items;

        internal ResourceType(Resources owningResources, ResourceDirectoryEntry directoryEntry)
        {
            resources = owningResources;
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

            resource_items = LoadResources();
        }

        #region Methods

        public IEnumerator<Resource> GetEnumerator()
        {
            for(var i = 0; i < resource_items.Length; i++)
            {
                Resource resource_name = resource_items[i];

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

        private Resource[] LoadResources()
        {
            List<Resource> results = new List<Resource>();
            ResourceDirectory directory = directory_entry.GetDirectory();

            foreach(ResourceDirectoryEntry entry in directory)
            {
                IResourceProvider provider = (id > 0 ? Resources.GetProvider(id) : Resources.GetProvider(name));
                Resource resource = provider.Create(this, entry);

                results.Add(resource);
            }

            return results.ToArray();
        }

        #endregion

        #region Properties

        public Resources Resources
        {
            get
            {
                return resources;
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

        public int Count
        {
            get
            {
                return resource_items.Length;
            }
        }

        public Resource this[int index]
        {
            get
            {
                return resource_items[index];
            }
        }

        #endregion

    }

    public sealed class Resources : IEnumerable<ResourceType>
    {

        private static Dictionary<uint, Tuple<string, string>> type_info;
        private static Dictionary<string, IResourceProvider> providers;
        private static DefaultResourceProvider default_provider;

        private ResourceType[] types;

        static Resources()
        {
            type_info = new Dictionary<uint, Tuple<string, string>>();
            providers = new Dictionary<string, IResourceProvider>(StringComparer.OrdinalIgnoreCase);
            default_provider = new DefaultResourceProvider();

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

        public static bool RegisterProvider(uint typeId, IResourceProvider provider)
        {
            return RegisterProvider(String.Format("#{0}",typeId), provider);
        }

        public static bool RegisterProvider(string typeName, IResourceProvider provider)
        {
            if (providers.ContainsKey(typeName))
                return false;

            providers.Add(typeName, provider);

            return true;
        }

        public static bool UnregisterProvider(uint typeId)
        {
            return UnregisterProvider(String.Format("#{0}", typeId));
        }

        public static bool UnregisterProvider(string typeName)
        {
            if (!providers.ContainsKey(typeName))
                return false;

            providers.Remove(typeName);

            return true;
        }

        public static IResourceProvider GetProvider(uint typeId)
        {
            return GetProvider(String.Format("#{0}", typeId));
        }

        public static IResourceProvider GetProvider(string typeName)
        {
            if (!providers.ContainsKey(typeName))
            {
                return default_provider;
            }
            else
            {
                return providers[typeName];
            }
        }

        private static void PopulateTypeInfo()
        {
            ushort[] known_types = new ushort[] {
                ResourceType.RT_CURSOR,
                ResourceType.RT_BITMAP,
                ResourceType.RT_ICON,
                ResourceType.RT_MENU,
                ResourceType.RT_DIALOG,
                ResourceType.RT_STRING,
                ResourceType.RT_FONTDIR,
                ResourceType.RT_FONT,
                ResourceType.RT_ACCELERATOR,
                ResourceType.RT_RCDATA,
                ResourceType.RT_MESSAGETABLE,
                ResourceType.RT_GROUP_CURSOR,
                ResourceType.RT_GROUP_ICON,
                ResourceType.RT_VERSION,
                ResourceType.RT_DLGINCLUDE,
                ResourceType.RT_PLUGPLAY,
                ResourceType.RT_VXD,
                ResourceType.RT_ANICURSOR,
                ResourceType.RT_ANIICON,
                ResourceType.RT_HTML,
                ResourceType.RT_MANIFEST
            };

            foreach (ushort type_id in known_types)
                PopulateTypeInfo(type_id);
        }

        private static void PopulateTypeInfo(ushort typeId)
        {
            string name;
            string constant_name;

            switch (typeId)
            {
                case ResourceType.RT_CURSOR:
                    name = "Cursor";
                    constant_name = "RT_CURSOR";
                    break;
                case ResourceType.RT_BITMAP:
                    name = "Bitmap";
                    constant_name = "RT_BITMAP";
                    break;
                case ResourceType.RT_ICON:
                    name = "Icon";
                    constant_name = "RT_ICON";
                    break;
                case ResourceType.RT_MENU:
                    name = "Menu";
                    constant_name = "RT_MENU";
                    break;
                case ResourceType.RT_DIALOG:
                    name = "Dialog";
                    constant_name = "RT_DIALOG";
                    break;
                case ResourceType.RT_STRING:
                    name = "String";
                    constant_name = "RT_STRING";
                    break;
                case ResourceType.RT_FONTDIR:
                    name = "Font Directory";
                    constant_name = "RT_FONTDIR";
                    break;
                case ResourceType.RT_FONT:
                    name = "Font";
                    constant_name = "RT_FONT";
                    break;
                case ResourceType.RT_ACCELERATOR:
                    name = "Accelerator";
                    constant_name = "RT_ACCELERATOR";
                    break;
                case ResourceType.RT_RCDATA:
                    name = "RC Data";
                    constant_name = "RT_RCDATA";
                    break;
                case ResourceType.RT_MESSAGETABLE:
                    name = "Message Table";
                    constant_name = "RT_MESSAGETABLE";
                    break;
                case ResourceType.RT_GROUP_CURSOR:
                    name = "Cursor Group";
                    constant_name = "RT_GROUP_CURSOR";
                    break;
                case ResourceType.RT_GROUP_ICON:
                    name = "Icon Group";
                    constant_name = "RT_GROUP_ICON";
                    break;
                case ResourceType.RT_VERSION:
                    name = "Version";
                    constant_name = "RT_VERSION";
                    break;
                case ResourceType.RT_DLGINCLUDE:
                    name = "Dialog Include";
                    constant_name = "RT_DLGINCLUDE";
                    break;
                case ResourceType.RT_PLUGPLAY:
                    name = "Plug & Play";
                    constant_name = "RT_PLUGPLAY";
                    break;
                case ResourceType.RT_VXD:
                    name = "VxD";
                    constant_name = "RT_VXD";
                    break;
                case ResourceType.RT_ANICURSOR:
                    name = "Animated Cursor";
                    constant_name = "RT_ANICURSOR";
                    break;
                case ResourceType.RT_ANIICON:
                    name = "Animated Icon";
                    constant_name = "RT_ANIICON";
                    break;
                case ResourceType.RT_HTML:
                    name = "HTML";
                    constant_name = "RT_HTML";
                    break;
                case ResourceType.RT_MANIFEST:
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
                yield return types[i];
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
                ResourceType type = new ResourceType(this, entry);

                results.Add(type);
            }

            return results.ToArray();
        }

        #endregion

        #region Properties

        public int Count
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
