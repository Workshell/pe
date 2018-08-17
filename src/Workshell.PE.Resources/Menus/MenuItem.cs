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
