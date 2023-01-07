using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using WW_SYSTEM.API;

namespace WW_SYSTEM.Triggers
{
    public class Trigger : MonoBehaviour 
    {
        public Action<Player> OnPlayerEnter = null;
        public Action<Player> OnPlayerLeave = null;

        public List<Player> PlayersInTrigger = new List<Player>();


        public int ID { get; private set; }

        public string Name { get; private set; }

        bool InitCompleted = false;

        public Vector3 TriggerScale = new Vector3();

        public void Init(int ID, string Name, Vector3 TriggerScale)
        {
            if (InitCompleted) return;

            this.ID = ID;
            this.Name = Name;
            this.TriggerScale = TriggerScale;

            Trigger_Manager.Triggers.Add(this);
            InitCompleted = true;
        }

        public void Delete()
        {
            if (Trigger_Manager.Triggers.Contains(this))
            {
                Trigger_Manager.Triggers.Remove(this);
            }
            DestroyImmediate(gameObject);
        }


        void OnDestroy()
        {
            if (Trigger_Manager.Triggers.Contains(this))
            {
                Trigger_Manager.Triggers.Remove(this);
            }
        }

        void FixedUpdate()
        {
            if (TriggerScale == null) return;
            if (!InitCompleted) return;

            try
            {
                
                Collider[] array = Physics.OverlapBox(gameObject.transform.position, TriggerScale, gameObject.transform.rotation);
                List<Player> pls = new List<Player>();
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].gameObject.TryGetComponent(out Player pl))
                    {
                        if(!pls.Contains(pl))
                        pls.Add(pl);
                    }
                }
                foreach (var item in PlayersInTrigger)
                {
                    if (item != null)
                    {
                        if (!pls.Contains(item))
                        {
                            if (OnPlayerLeave != null)
                            {
                                OnPlayerLeave.Invoke(item);
                            }
                        }
                    }
                }
                foreach (var item in pls)
                {
                    if (!PlayersInTrigger.Contains(item))
                    {
                        if (OnPlayerEnter != null)
                        {
                            OnPlayerEnter.Invoke(item);
                        }
                    }
                }
                PlayersInTrigger = pls;
            }
            catch (Exception ex)
            {
                Logger.Error($"TRIGGER: [{Name}]", $"DETECTED ERROR: {ex}");
            }

           

        }

    }
}
