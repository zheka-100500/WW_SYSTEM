using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.Attributes;
using WW_SYSTEM.Config;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using WW_SYSTEM.Translation;
using WW_SYSTEM.UpdateSystem;

namespace WW_SYSTEM
{
	public abstract class Plugin
	{
	
		public PluginDetails Details { get; internal set; }

		

	
		public EventManager EventManager
		{
			get
			{
				return EventManager.Manager;
			}
		}

	
		public PluginManager PluginManager
		{
			get
			{
				return PluginManager.Manager;
			}
		}


		public TranslateData Translate
		{
			get
			{
				return Translator.GetTranslationForPlugin(this.Details.name);
			}
		}

		public PluginUpdate AutoUpdate = null;



		
		public abstract void Register();

	
		public abstract void OnEnable();

		
		public abstract void OnDisable();

		
		
		public void AddEventHandlers(IEventHandler handler, Priority priority = Priority.Normal)
		{
			this.EventManager.AddEventHandlers(this, handler, priority);
		}

	

		public void AddEventHandler(Type eventType, IEventHandler handler, Priority priority = Priority.Normal)
		{
			this.EventManager.AddEventHandler(this, eventType, handler, priority);
		}





		
		public void Debug(string message)
		{
			Logger.Debug(this.Details.id, message);
		}

		public PluginConfig Config {

			get
			{
				if (!Directory.Exists(FileManager.GetAppFolder(true, true, "") + "Plugins_Configs"))
					Directory.CreateDirectory(FileManager.GetAppFolder(true, true, "") + "Plugins_Configs");
				return new PluginConfig(FileManager.GetAppFolder(true, true, "") + "Plugins_Configs/" + this.Details.name + ".txt");
			}
		
		}
	
		public void Info(string message)
		{
			Logger.Info(this.Details.id, message);
		}

		
		public void Warn(string message)
		{
			Logger.Warn(this.Details.id, message);
		}

		
		public void Error(string message)
		{
			Logger.Error(this.Details.id, message);
		}

		
		public override string ToString()
		{
			if (this.Details == null)
			{
				return base.ToString();
			}
			return this.Details.name + "(" + this.Details.id + ")";
		}

	
		[Obsolete("Use EventManager instead.")]
		public readonly EventManager eventManager = EventManager.Manager;


		[Obsolete("Use PluginManager instead.")]
		public readonly PluginManager pluginManager = PluginManager.Manager;
	}
}
