using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace WW_SYSTEM.Triggers
{
    public static class Trigger_Manager
    {
        public static List<Trigger> Triggers = new List<Trigger>();


        #region TryGetTrigger

        public static bool TryGetTrigger(string name, out Trigger trigger)
        {
            try
            {

                for (int i = 0; i < Triggers.Count; i++)
                {
                    if (Triggers[i].Name == name)
                    {
                        trigger = Triggers[i];
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }

            trigger = null;
            return false;
        }

        public static bool TryGetTrigger(int Id, out Trigger trigger)
        {
            try
            {

                for (int i = 0; i < Triggers.Count; i++)
                {
                    if (Triggers[i].ID == Id)
                    {
                        trigger = Triggers[i];
                        return true;
                    }
                }
            }
            catch (Exception)
            {
            }

            trigger = null;
            return false;
        }

        #endregion

        public static Trigger CreateTrigger(int id, string Name, Vector3 Pos, Quaternion Rot, Vector3 Scale, bool IsRestartProtected = false)
        {
            try
            {

                if (TryGetTrigger(id, out var t)) return t;
                if (TryGetTrigger(Name, out var t1)) return t1;

                Trigger trigger = GameObject.CreatePrimitive(PrimitiveType.Cube).AddComponent<Trigger>();
                trigger.gameObject.transform.position = Pos;
                trigger.gameObject.transform.rotation = Rot;
                GameObject.DestroyImmediate(trigger.gameObject.GetComponent<BoxCollider>());
                trigger.Init(id, Name, Scale);
                if (IsRestartProtected)
                {
                    GameObject.DontDestroyOnLoad(trigger.gameObject);
                }
                return trigger;
            }
            catch (Exception ex)
            {
                throw new Exception($"FAILED TO CREATE TRIGGER: {ex}");
            }
        }




    }
}
