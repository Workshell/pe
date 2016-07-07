using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Workshell.PE")]
[assembly: AssemblyDescription("A .NET class library for reading the PE executable format")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Workshell Ltd")]
[assembly: AssemblyProduct(".NET PE Class Library")]
[assembly: AssemblyCopyright("Copyright ©2016 Workshell Ltd")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5ef8a44d-2548-4cda-b507-bc3a7907ee1e")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
[assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyInformationalVersion("1.0.0")]

#if !SIGNED
[assembly: InternalsVisibleTo("peres")]
#else
[assembly: InternalsVisibleTo("peres, PublicKey=0024000004800000940000000602000000240000525341310004000001000100259ed23116da6a496f873182c31284a428d040b37885524e9b53049cd99d5cc84feb00dbe77278afda8ebc9def14111b20b561f8d958e3f4aea2d492fed946245c528b16cad6ee785995ccfd7e6b7b34fe4be452a651069b2c0bbcf668bfb1dd9b99a7f30ab10d289525d61e82fd45e1ebcc11fc3d286e6096a1ee7edeee6091")]
#endif