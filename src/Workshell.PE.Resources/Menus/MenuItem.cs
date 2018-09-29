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

using System;
using System.Collections.Generic;
using System.Text;

namespace Workshell.PE.Resources.Menus
{
    public class MenuItem
    {
        internal MenuItem(ushort id, string text, ushort flags)
        {
            Id = id;

            var parts = text.Split(new [] { '\t' }, 2);

            Text = parts[0];
            Shortcut = (parts.Length > 1 ? parts[1] : string.Empty);
            Flags = (MenuItemFlags)flags;
        }

        #region Methods

        public override string ToString()
        {
            if (IsSeperator)
                return "-";

            var result = Text;

            if (IsPopup)
                result = "+" + result;

            if (Shortcut != string.Empty)
                result += " | " + Shortcut;

            return result;
        }

        #endregion

        #region Properties

        public ushort Id { get; }
        public string Text { get; }
        public string Shortcut { get; }
        public MenuItemFlags Flags { get; }
        public bool IsPopup => (Id == 0);
        public bool IsSeperator => (Id == 0 && Flags == MenuItemFlags.Enabled && Shortcut == string.Empty);

        #endregion
    }
}
