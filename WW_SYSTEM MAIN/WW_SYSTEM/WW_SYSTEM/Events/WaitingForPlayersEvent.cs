using InventorySystem.Items.Pickups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW_SYSTEM.API;
using WW_SYSTEM.Custom_Items;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Level_System;
namespace WW_SYSTEM.Events
{
    public class WaitingForPlayersEvent : WFP
    {

        public WaitingForPlayersEvent()
        {

            if(Round.Loot_Spawn_Multipler > 1)
            {
                foreach (var item in UnityEngine.GameObject.FindObjectsOfType<ItemPickupBase>())
                {
                    for (int i = 1; i < Round.Loot_Spawn_Multipler; i++)
                    {
                        Round.SpawnItem(item.NetworkInfo.ItemId, item.transform.position);
                    }
             
                }


            }

            if (Round.UseLevelSYSTEM)
            {
                Logger.Info("LEVELSYSTEM", "CLEARING BUFFER");
                LevelManager.Levels.Clear();
            }
            CustomItemManager.ItemsInRound.Clear();
        }
        public override void ExecuteHandler(IEventHandler handler)
        {
            ((IEventHandlerWaitingForPlayers)handler).OnWaitingForPlayers(this);
        }
    }
}
