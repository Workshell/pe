#region License
//  Copyright(c) 2016, Workshell Ltd
//  All rights reserved.
//  
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//     * Redistributions of source code must retain the above copyright
//       notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of Workshell Ltd nor the names of its contributors
//  may be used to endorse or promote products
//  derived from this software without specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED.IN NO EVENT SHALL WORKSHELL BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Workshell.PE.Native;

namespace Workshell.PE.Resources
{

    [Serializable]
    public class ResourceException : Exception
    {

        public ResourceException() : base()
        {
        }

        public ResourceException(string message) : base(message)
        {
        }

        public ResourceException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ResourceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

    }

    public class Resource : ISupportsBytes
    {

        public const uint DEFAULT_LANGUAGE = 1033;

        private ResourceType type;
        private ResourceDirectoryEntry directory_entry;
        private ResourceId id;
        private Dictionary<uint, ResourceDirectoryEntry> languages;

        internal Resource(ResourceType owningType, ResourceDirectoryEntry directoryEntry)
        {
            type = owningType;
            directory_entry = directoryEntry;

            if (directory_entry.NameType == NameType.ID)
            {
                id = directory_entry.GetId();
            }
            else
            {
                id = directory_entry.GetName();
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

        public byte[] GetBytes()
        {
            return GetBytes(DEFAULT_LANGUAGE);
        }

        public byte[] GetBytes(uint languageId)
        {
            if (!languages.ContainsKey(languageId))
            {
                throw new ResourceException(String.Format("Cannot find specified language: {0}", languageId));
            }
            else
            {
                ResourceDirectoryEntry language_entry = languages[languageId];
                ResourceDataEntry data_entry = language_entry.GetDataEntry();
                ResourceData data = data_entry.GetData();

                return data.GetBytes();
            }
        }

        public void Save(string fileName)
        {
            Save(fileName, DEFAULT_LANGUAGE);
        }

        public void Save(string fileName, uint language)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Save(file, language);
                file.Flush();
            }
        }

        public void Save(Stream stream)
        {
            Save(stream, DEFAULT_LANGUAGE);
        }

        public virtual void Save(Stream stream, uint language)
        {
            byte[] data = GetBytes(language);

            stream.Write(data, 0, data.Length);
        }

        public override string ToString()
        {
            return id.ToString();
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

        public ResourceId Id
        {
            get
            {
                return id;
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

        private static Dictionary<ResourceId, Type> resource_types;

        private ResourceCollection resources;
        private ResourceDirectoryEntry directory_entry;
        private ResourceId id;
        private Resource[] resource_items;

        static ResourceType()
        {
            resource_types = new Dictionary<ResourceId, Type>();    
        }

        internal ResourceType(ResourceCollection owningResources, ResourceDirectoryEntry directoryEntry)
        {
            resources = owningResources;
            directory_entry = directoryEntry;

            if (directory_entry.NameType == NameType.ID)
            {
                id = directory_entry.GetId();
            }
            else
            {
                id = directory_entry.GetName();
            }

            resource_items = LoadResources();
        }

        #region Static Methods

        public static bool Register(ResourceId typeId, Type dataType)
        {
            if (resource_types.ContainsKey(typeId))
                return false;

            resource_types.Add(typeId, dataType);

            return true;
        }

        public static bool Unregister(ResourceId typeId)
        {
            if (!resource_types.ContainsKey(typeId))
                return false;

            resource_types.Remove(typeId);

            return true;
        }

        private static Type Get(ResourceId typeId)
        {
            if (resource_types.ContainsKey(typeId))
            {
                return resource_types[typeId];
            }
            else
            {
                return null;
            }
        }

        #endregion

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

            if (!id.IsNumeric)
            {
                result = id.ToString();
            }
            else
            {
                string constant = ResourceCollection.GetTypeConstant(id);

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
                Type resource_type = Get(id);
                Resource resource = null;

                if (resource_type != null)
                {
                    if (!resource_type.IsSubclassOf(typeof(Resource)))
                        throw new Exception("Type must be a subclass of Resource.");

                    resource = (Resource)Activator.CreateInstance(resource_type, this, entry);
                }
                else
                {
                    resource = new Resource(this, entry);
                }

                results.Add(resource);
            }

            return results.ToArray();
        }

        #endregion

        #region Properties

        public ResourceCollection Resources
        {
            get
            {
                return resources;
            }
        }

        public ResourceId Id
        {
            get
            {
                return id;
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

    public sealed class ResourceCollection : IEnumerable<ResourceType>
    {

        private static Dictionary<uint, Tuple<string, string>> type_info;

        private ExecutableImage image;
        private ResourceType[] types;

        static ResourceCollection()
        {
            type_info = new Dictionary<uint, Tuple<string, string>>();

            PopulateTypeInfo();
        }

        internal ResourceCollection(ExecutableImage exeImage, ResourceDirectory rootDirectory)
        {
            image = exeImage;
            types = LoadTypes(rootDirectory);
        }

        #region Static Methods

        public static ResourceCollection Get(ExecutableImage image)
        {
            ResourceDirectory root_directory = ResourceDirectory.GetRootDirectory(image);

            if (root_directory == null)
                return null;

            ResourceCollection result = new ResourceCollection(image, root_directory);

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

        public Resource Find(ResourceDirectoryEntry typeEntry, ResourceDirectoryEntry nameEntry, ResourceDirectoryEntry langEntry)
        {
            foreach(ResourceType type in types)
            {
                ResourceId type_id;

                if (typeEntry.NameType == NameType.ID)
                {
                    type_id = new ResourceId(typeEntry.GetId());
                }
                else
                {
                    type_id = new ResourceId(typeEntry.GetName());
                }

                if (type.Id == type_id)
                {
                    foreach(Resource res in type)
                    {
                        ResourceId res_id;

                        if (nameEntry.NameType == NameType.ID)
                        {
                            res_id = new ResourceId(nameEntry.GetId());
                        }
                        else
                        {
                            res_id = new ResourceId(nameEntry.GetName());
                        }

                        if (res.Id == res_id)
                        {
                            uint lang_id = langEntry.GetId();

                            if (res.Languages.Contains(lang_id))
                                return res;
                        }
                    }
                }
            }

            return null;
        }

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

        public ExecutableImage Image
        {
            get
            {
                return image;
            }
        }

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
