using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WW_SYSTEM.UpdateSystem
{
    public class PluginUpdate
    {

        public string LinkToVersionFile;
        public string LinkToDllFile;
        public string DllFileName;

        public PluginUpdate(string LinkToVersion, string LinkToDll, string DllName)
        {
            LinkToVersionFile = LinkToVersion;
            LinkToDllFile = LinkToDll;
            DllFileName = DllName;
        }

        public PluginUpdate(string LinkToVersion, string LinkToDll)
        {
            LinkToVersionFile = LinkToVersion;
            LinkToDllFile = LinkToDll;
            DllFileName = string.Empty;
        }


    }
}
