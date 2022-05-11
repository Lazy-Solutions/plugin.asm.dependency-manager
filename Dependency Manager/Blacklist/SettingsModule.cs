using System;
using System.Collections.Generic;

namespace plugin.asm.dependency_manager.Blacklist
{

    [Serializable]
    /// <summary>The settings of the blacklist system. A reference to the current settings may be obtained from the current profile.</summary>
    public class SettingsModule
    {

        public bool isWhitelist;
        public List<string> paths = new List<string>();

    }

}
