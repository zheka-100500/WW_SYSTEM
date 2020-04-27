using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM;
using WW_SYSTEM.Attributes;

namespace TestPlugin
{
    [PluginDetails(
        author = "zheka_100500",
        name = "TestPlugin",
        description = "TestPlugin",
        id = "zheka_100500.test.plugin",
        configPrefix = "TestPlugin",
        version = "1.0",
        WWSYSTEMMajor = 6,
        WWSYSTEMMinor = 3,
        WWSYSTEMRevision = 1
        )]
    public class TestPlugin : Plugin
    {
        public override void OnDisable()
        {
            this.Info(this.Details.name + " was disabled ):");
        }

        public override void OnEnable()
        {
            this.Info(this.Details.name + " has loaded :)");
        }

        public override void Register()
        {
            AddEventHandlers(new EventHandler(this));
        }
    }
}
