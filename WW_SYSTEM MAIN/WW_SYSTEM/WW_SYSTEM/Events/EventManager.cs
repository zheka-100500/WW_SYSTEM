using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.EventHandlers;
using UnityEngine;

namespace WW_SYSTEM.Events
{
	public class EventManager
	{
		
		public static EventManager Manager
		{
			get
			{
				if (EventManager.singleton == null)
				{
					EventManager.singleton = new EventManager();
				}
				return EventManager.singleton;
			}
		}

	
		public EventManager()
		{
			this.event_meta = new Dictionary<Type, List<EventHandlerWrapper>>();
			this.snapshots = new Dictionary<Plugin, EventManager.Snapshot>();
		}

	
		public void HandleEvent<T>(Event ev) where T : IEventHandler
		{
		
		
			foreach (T t in this.GetEventHandlers<T>())
			{
			
				try
				{
					Logger.Debug("EVENT", "EXECUTE EVENT HANDLER: " + ev.ToString());
					ev.ExecuteHandler(t);
				}
				catch (Exception ex)
				{
					Logger.Error("Event", "Event Handler: " + t.GetType().ToString() + " Failed to handle event:" + ev.GetType().ToString());
					Logger.Error("Event", ex.ToString());
				}
			}
		}

	
		public void AddEventHandlers(Plugin plugin, IEventHandler handler, Priority priority = Priority.Normal)
		{
			foreach (Type type in handler.GetType().GetInterfaces())
			{
				if (typeof(IEventHandler).IsAssignableFrom(type))
				{
					plugin.Debug("Adding event handler for " + type.Name);
					this.AddEventHandler(plugin, type, handler, priority);
				}
			}
		}


		public void AddEventHandler(Plugin plugin, Type eventType, IEventHandler handler, Priority priority = Priority.Normal)
		{
			plugin.Debug(string.Format("Adding event handler from: {0} type: {1} priority: {2} handler: {3}", new object[]
			{
				plugin.Details.name,
				eventType,
				priority,
				handler.GetType()
			}));
			EventHandlerWrapper wrapper = new EventHandlerWrapper(plugin, priority, handler);
			if (PluginManager.Manager.GetEnabledPlugin(plugin.Details.id) == null)
			{
				if (!this.snapshots.ContainsKey(plugin))
				{
					this.snapshots.Add(plugin, new EventManager.Snapshot());
				}
				this.snapshots[plugin].Entries.Add(new EventManager.Snapshot.SnapshotEntry(eventType, wrapper));
			}
			this.AddEventMeta(eventType, wrapper, handler);
		}

		private void AddEventMeta(Type eventType, EventHandlerWrapper wrapper, IEventHandler handler)
		{
			Logger.Debug("EVENT MANAGER","Adding event META for " + eventType.ToString());
			if (!this.event_meta.ContainsKey(eventType))
			{
				this.event_meta.Add(eventType, new List<EventHandlerWrapper>
				{
					wrapper
				});
				Logger.Debug("EVENT MANAGER", "Adding event META for " + eventType.ToString() + " DONE!");
				return;
			}
			List<EventHandlerWrapper> list = this.event_meta[eventType];
			list.Add(wrapper);
			list.Sort(EventManager.priorityCompare);
			list.Reverse();
		}


		public void AddSnapshotEventHandlers(Plugin plugin)
		{
			if (!this.snapshots.ContainsKey(plugin) || !this.snapshots[plugin].Active)
			{
				return;
			}
			this.snapshots[plugin].Active = false;
			foreach (EventManager.Snapshot.SnapshotEntry snapshotEntry in this.snapshots[plugin].Entries)
			{
				this.AddEventMeta(snapshotEntry.Type, snapshotEntry.Wrapper, snapshotEntry.Wrapper.Handler);
			}
		}

		
		public void RemoveEventHandlers(Plugin plugin)
		{
			Dictionary<Type, List<EventHandlerWrapper>> dictionary = new Dictionary<Type, List<EventHandlerWrapper>>();
			foreach (KeyValuePair<Type, List<EventHandlerWrapper>> keyValuePair in this.event_meta)
			{
				List<EventHandlerWrapper> list = new List<EventHandlerWrapper>();
				foreach (EventHandlerWrapper eventHandlerWrapper in keyValuePair.Value)
				{
					if (eventHandlerWrapper.Plugin != plugin)
					{
						list.Add(eventHandlerWrapper);
					}
				}
				if (list.Count > 0)
				{
					dictionary.Add(keyValuePair.Key, list);
				}
			}
			this.event_meta = dictionary;
			if (this.snapshots.ContainsKey(plugin))
			{
				this.snapshots[plugin].Active = true;
			}
		}

		
		public List<T> GetEventHandlers<T>() where T : IEventHandler
		{
		
			List<T> list = new List<T>();
			
			

			if (this.event_meta.ContainsKey(typeof(T)))
			{
			
				foreach (EventHandlerWrapper eventHandlerWrapper in this.event_meta[typeof(T)])
				{
					
					IEventHandler handler = eventHandlerWrapper.Handler;
					if (handler is T)
					{

						
						T item = (T)((object)handler);
						list.Add(item);
					}
				}
			}
			
		
			return list;
		}

	
		private static EventManager singleton;

	
		private static EventManager.PriorityComparator priorityCompare = new EventManager.PriorityComparator();

		
		private Dictionary<Type, List<EventHandlerWrapper>> event_meta;


		private readonly Dictionary<Plugin, EventManager.Snapshot> snapshots;

	
		private class PriorityComparator : IComparer<EventHandlerWrapper>
		{
			
			public int Compare(EventHandlerWrapper x, EventHandlerWrapper y)
			{
				return x.Priority.CompareTo(y.Priority);
			}
		}


		private class Snapshot
		{
		
			
			public List<EventManager.Snapshot.SnapshotEntry> Entries { get; private set; }

	
			public bool Active { get; set; }

			
			public Snapshot()
			{
				this.Entries = new List<EventManager.Snapshot.SnapshotEntry>();
			}

			
			public class SnapshotEntry
			{
			
				public Type Type { get; }

				
				public EventHandlerWrapper Wrapper { get; }

				
				public SnapshotEntry(Type type, EventHandlerWrapper wrapper)
				{
					this.Type = type;
					this.Wrapper = wrapper;
				}
			}
		}
	}
}
