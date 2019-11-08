#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

using System.Reflection;
using System.Runtime.CompilerServices;

#if !SIGNED
[assembly: InternalsVisibleTo("Workshell.PE.Resources")]
[assembly: InternalsVisibleTo("Workshell.PE.Tests")]
#else
[assembly: InternalsVisibleTo("Workshell.PE.Resources, PublicKey=0024000004800000940000000602000000240000525341310004000001000100259ed23116da6a496f873182c31284a428d040b37885524e9b53049cd99d5cc84feb00dbe77278afda8ebc9def14111b20b561f8d958e3f4aea2d492fed946245c528b16cad6ee785995ccfd7e6b7b34fe4be452a651069b2c0bbcf668bfb1dd9b99a7f30ab10d289525d61e82fd45e1ebcc11fc3d286e6096a1ee7edeee6091")]
[assembly: InternalsVisibleTo("Workshell.PE.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100259ed23116da6a496f873182c31284a428d040b37885524e9b53049cd99d5cc84feb00dbe77278afda8ebc9def14111b20b561f8d958e3f4aea2d492fed946245c528b16cad6ee785995ccfd7e6b7b34fe4be452a651069b2c0bbcf668bfb1dd9b99a7f30ab10d289525d61e82fd45e1ebcc11fc3d286e6096a1ee7edeee6091")]
#endif

[assembly: AssemblyTitle("Workshell.PE")]
[assembly: AssemblyDescription("A .NET class library for reading the PE executable format")]