using MapGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WW_SYSTEM.API
{
    public class ROOM
    {
        public string Name { get; }

        public RoomType Type { get; }

        public ZoneType Zone { get; }

        public RoomShape Shape { get; } = RoomShape.Undefined;

        public RoomIdentifier Identifier { get; }

        public Transform Transform
        {
            get { return Object.transform; }
        }

        public GameObject Object { get; }



        public void SetFlickerableLightIntensity(float intensity)
        {
            foreach(FlickerableLightController flickerableLightController in Object.GetComponentsInChildren<FlickerableLightController>())
              {

                flickerableLightController.LightIntensityMultiplier = intensity;
            }
        
        }


        public void EnableFlickering(float duration = 10f)
        {
            foreach (FlickerableLightController flickerableLightController in Object.GetComponentsInChildren<FlickerableLightController>())
            {

                flickerableLightController.ServerFlickerLights(duration);
            }

        }

        private ZoneType GetZone(FacilityZone zone)
        {
            switch (zone)
            {
                case FacilityZone.LightContainment:
                    return ZoneType.Light;
                case FacilityZone.HeavyContainment:
                    return ZoneType.Heavy;
                case FacilityZone.Entrance:
                    return ZoneType.Entrance;
                case FacilityZone.Surface:
                    return ZoneType.Outside;
                case FacilityZone.Other:
                    return ZoneType.NONE;
                default:
                    return ZoneType.NONE;
            }
        }

        private RoomType GetType(RoomName type)
        {
            switch (type)
            {
                case RoomName.Lcz173:
                    return RoomType.Scp173;
                case RoomName.LczGreenhouse:
                    return RoomType.GR018;
                case RoomName.Lcz914:
                    return RoomType.Scp914;
                case RoomName.LczAirlock:
                    return RoomType.Airlock;
                case RoomName.LczArmory:
                    return RoomType.LCZArmory;
                case RoomName.LczCheckpointA:
                    return RoomType.LCZCheckpointA;
                case RoomName.LczCheckpointB:
                    return RoomType.LCZCheckpointB;
                case RoomName.LczClassDSpawn:
                    return RoomType.ClassDSpawn;
                case RoomName.LczGlassroom:
                    return RoomType.Plants;
                case RoomName.LczToilets:
                    return RoomType.Toilets;
                case RoomName.Hcz049:
                    return RoomType.Scp049;
                case RoomName.Hcz079:
                    return RoomType.Scp079;
                case RoomName.Hcz106:
                    return RoomType.Scp106;
                case RoomName.Hcz096:
                    return RoomType.Scp096;
                case RoomName.Hcz939:
                    return RoomType.Scp939;
                case RoomName.HczCheckpointA:
                    return RoomType.HCZCheckpointA;
                case RoomName.HczCheckpointB:
                    return RoomType.HCZCheckpointB;
                case RoomName.HczCheckpointToEntranceZone:
                    return RoomType.EZCheckpoint;
                case RoomName.HczMicroHID:
                    return RoomType.MicroHID;
                case RoomName.HczWarhead:
                    return RoomType.Nuke;
                case RoomName.HczServers:
                    return RoomType.Servers;
                case RoomName.HczArmory:
                    return RoomType.HCZArmory;
                case RoomName.HczTesla:
                    return RoomType.Tesla;
                case RoomName.EzGateA:
                    return RoomType.GateA;
                case RoomName.EzGateB:
                    return RoomType.GateB;
     
                case RoomName.EzCollapsedTunnel:
                    return RoomType.CollapsedTunnel;
                case RoomName.EzRedroom:
                    return RoomType.RedGate;
                case RoomName.EzIntercom:
                    return RoomType.Intercom;
                case RoomName.EzOfficeLarge:
                    return RoomType.PCs;
                case RoomName.EzOfficeSmall:
                    return RoomType.PCs_small;
                case RoomName.EzEvacShelter:
                    return RoomType.Shelter;
                case RoomName.EzOfficeStoried:
                    return RoomType.EZsmallstraight;
                case RoomName.Outside:
                    return RoomType.Outside;
                case RoomName.Pocket:
                    return RoomType.Pocket;
                case RoomName.Unnamed:
                    return RoomType.NONE;
                default:
                    return RoomType.NONE;
            }
        }

        public ROOM(RoomIdentifier room)
        {
            if (room == null) return;
            Identifier = room;
            Object = room.gameObject;
            
            Name = room.Name.ToString();

            Zone = GetZone(room.Zone);

            Type = GetType(room.Name);


        }

        public ROOM(GameObject room)
        {
            this.Object = room;
            this.Name = room.name;
            bool flag = this.Name.ToUpper().StartsWith("LCZ");
            if (flag)
            {
                this.Zone = ZoneType.Light;
            }
            else
            {
                bool flag2 = this.Name.ToUpper().StartsWith("HCZ");
                if (flag2)
                {
                    this.Zone = ZoneType.Heavy;
                }
                else
                {
                    bool flag3 = this.Name.ToUpper().StartsWith("EZ");
                    if (flag3)
                    {
                        this.Zone = ZoneType.Entrance;
                    }
                    else
                    {
                        bool flag4 = this.Name.ToUpper().Contains("OUTSIDE");
                        if (flag4)
                        {
                            this.Zone = ZoneType.Outside;
                        }
                        else
                        {
                            bool flag5 = this.Name.Contains("Map_LC_372CR");
                            if (flag5)
                            {
                                this.Zone = ZoneType.Light;
                            }
                            else
                            {
                                this.Zone = ZoneType.NONE;
                            }
                        }
                    }
                }
            }
            string text = this.Name.Replace("LCZ_", "").Replace("HCZ_", "").Replace("EZ_", "");
            this.Type = 0;
            foreach (string text2 in Enum.GetNames(typeof(RoomType)))
            {
                bool flag6 = text.ToUpper().Contains(text2.ToUpper());
                if (flag6)
                {
                    this.Type = (RoomType)Enum.Parse(typeof(RoomType), text2);
                    break;
                }
                bool flag7 = text.Contains("Testroom");
                if (flag7)
                {
                    this.Type = RoomType.Scp939;
                    this.Name = "HCZ_939";
                    this.Zone = ZoneType.Heavy;
                    break;
                }
                bool flag8 = text2.Contains("Scp") && text.Contains(text2.Replace("Scp", ""));
                if (flag8)
                {
                    this.Type = (RoomType)Enum.Parse(typeof(RoomType), text2);
                    break;
                }
                bool flag9 = text.ToUpper().Contains("OUTSIDE");
                if (flag9)
                {
                    this.Type = RoomType.Outside;
                    this.Name = "Outside";
                    break;
                }
                bool flag10 = text.Contains("Map_LC_372CR");
                if (flag10)
                {
                    this.Type = RoomType.GR018;
                    this.Name = "LCZ_372";
                    this.Zone = ZoneType.Light;
                    break;
                }
                bool flag11 = text.ToUpper().Contains("CHKP");
                if (flag11)
                {
                    this.Name = this.Name.Replace("Chkp", "Checkpoint");
                    this.Type = RoomType.Checkpoint;
                    break;
                }
                bool flag12 = text.Contains("Structure");
                if (flag12)
                {
                    this.Type = RoomType.MicroHID;
                    this.Name = "HCZ_MicroHID";
                    this.Zone = ZoneType.Heavy;
                    break;
                }
                bool flag13 = text.Contains("3room_HC");
                if (flag13)
                {
                    this.Type = RoomType.Elevator;
                    this.Name = this.Name.Replace("3room_HC", "Elevator");
                    this.Zone = ZoneType.Light;
                    break;
                }
                bool flag14 = text.Contains("Room3ar");
                if (flag14)
                {
                    this.Type = RoomType.HCZArmory;
                    this.Name = "HCZ_ARMORY";
                    break;
                }
            }
        }

    }
}
