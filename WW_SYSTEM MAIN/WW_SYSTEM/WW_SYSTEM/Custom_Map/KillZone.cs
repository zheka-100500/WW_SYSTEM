using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using PlayerStatsSystem;
namespace WW_SYSTEM.Custom_Map
{
    public class KillZone : MonoBehaviour
    {

        public StandardDamageHandler DamageHandler = new CustomReasonDamageHandler("KILL ZONE");

        private void Start()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (Round.TryGetPlayer(other.gameObject, out var pl))
            {
                pl.Kill(DamageHandler);

            }
        }
    }
}
