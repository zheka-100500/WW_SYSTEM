using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WW_SYSTEM.API
{
    public class ROOM_DOOR
    {

        

        public ROOM_DOOR(DoorVariant doorComponent)
        {
            this.component = doorComponent;

            if(doorComponent.GetComponent<DoorNametagExtension>() != null)
            {
                this.DOORNAME = doorComponent.GetComponent<DoorNametagExtension>().GetName;
            }
            else
            {
                this.DOORNAME = "NONE";
            }

            this.OBJNAME = doorComponent.name;
            this.DoorObj = doorComponent.gameObject;
            ROOM = new ROOM(RoomIdUtils.RoomAtPosition(doorComponent.transform.position));


            switch (doorComponent.RequiredPermissions.RequiredPermissions)
            {
                case KeycardPermissions.ScpOverride:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.SCP_OVERRIDE;
                    break;
                case KeycardPermissions.Checkpoints:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.CHCKPOINT_ACC;
                    break;
                case KeycardPermissions.ExitGates:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.GATES;
                    break;
                case KeycardPermissions.Intercom:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.INCOM_ACC;
                    break;
                case KeycardPermissions.AlphaWarhead:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.WARHEAD;
                    break;
                case KeycardPermissions.ContainmentLevelOne:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.CONT_LVL_1;
                    break;
                case KeycardPermissions.ContainmentLevelTwo:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.CONT_LVL_2;
                    break;
                case KeycardPermissions.ContainmentLevelThree:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.CONT_LVL_3;
                    break;
                case KeycardPermissions.ArmoryLevelOne:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.ARMORY_LVL_1;
                    break;
                case KeycardPermissions.ArmoryLevelTwo:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.ARMORY_LVL_2;
                    break;
                case KeycardPermissions.ArmoryLevelThree:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.ARMORY_LVL_3;
                    break;
                default:
                    this.PERMTYPE = DOOR_PERMISSION_TYPE.NONE;
                    break;
            }

        }


        private DOOR_PERMISSION_TYPE PERMTYPE { get; set; }

        public bool IsLocked
        {
            get
            {
                if (component.ActiveLocks > 0)
                {
                    DoorLockMode mode = DoorLockUtils.GetMode((DoorLockReason)component.ActiveLocks);
                    if ((!mode.HasFlagFast(DoorLockMode.CanClose) || !mode.HasFlagFast(DoorLockMode.CanOpen)) && (mode == DoorLockMode.FullLock || (component.TargetState && !mode.HasFlagFast(DoorLockMode.CanClose)) || (!component.TargetState && !mode.HasFlagFast(DoorLockMode.CanOpen))))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public string DOORNAME { get; }

        public string OBJNAME { get; }

        public GameObject DoorObj;

        public DoorVariant component;

        public ROOM ROOM;

        public DOOR_PERMISSION_TYPE PERMISSION_TYPE{

            get{return this.PERMTYPE; }

            set{
                if(value != DOOR_PERMISSION_TYPE.NONE)
                {
                    this.PERMTYPE = value;
                    switch (value)
                    {
                        case DOOR_PERMISSION_TYPE.NONE:
                            break;
                        case DOOR_PERMISSION_TYPE.SCP_OVERRIDE:
                            component.RequiredPermissions.RequiredPermissions = KeycardPermissions.ScpOverride;
                            break;
                        case DOOR_PERMISSION_TYPE.CHCKPOINT_ACC:
                            component.RequiredPermissions.RequiredPermissions = KeycardPermissions.Checkpoints;
                            break;
                        case DOOR_PERMISSION_TYPE.INCOM_ACC:
                            component.RequiredPermissions.RequiredPermissions = KeycardPermissions.Intercom;
                            break;
                        case DOOR_PERMISSION_TYPE.CONT_LVL_1:
                            component.RequiredPermissions.RequiredPermissions = KeycardPermissions.ContainmentLevelOne;
                            break;
                        case DOOR_PERMISSION_TYPE.CONT_LVL_2:
                            component.RequiredPermissions.RequiredPermissions = KeycardPermissions.ContainmentLevelTwo;
                            break;
                        case DOOR_PERMISSION_TYPE.CONT_LVL_3:
                            component.RequiredPermissions.RequiredPermissions = KeycardPermissions.ContainmentLevelThree;
                            break;
                        case DOOR_PERMISSION_TYPE.ARMORY_LVL_1:
                            component.RequiredPermissions.RequiredPermissions = KeycardPermissions.ArmoryLevelOne;
                            break;
                        case DOOR_PERMISSION_TYPE.ARMORY_LVL_2:
                            component.RequiredPermissions.RequiredPermissions = KeycardPermissions.ArmoryLevelTwo;
                            break;
                        case DOOR_PERMISSION_TYPE.ARMORY_LVL_3:
                            component.RequiredPermissions.RequiredPermissions = KeycardPermissions.ArmoryLevelThree;
                            break;
                        case DOOR_PERMISSION_TYPE.WARHEAD:
                            component.RequiredPermissions.RequiredPermissions = KeycardPermissions.AlphaWarhead;
                            break;
                        case DOOR_PERMISSION_TYPE.GATES:
                            component.RequiredPermissions.RequiredPermissions = KeycardPermissions.ExitGates;
                            break;
                        default:
                            break;
                    }
                   
                }
                else
                {
                    this.PERMTYPE = value;
                    component.RequiredPermissions.RequiredPermissions = KeycardPermissions.None;
                }
              
            }

        }

        public bool Lock
        {

            set { component.ServerChangeLock(DoorLockReason.AdminCommand, value); }

        }

        public void ChangeState() {
            component.NetworkTargetState = !component.TargetState;
        }

        public bool IsClosed
        {

            get { return !component.TargetState; }

        }



        public void Destroy()
        {
            IDamageableDoor damageableDoor;
            if ((damageableDoor = (component as IDamageableDoor)) != null)
            {
                if (!damageableDoor.IsDestroyed && component.GetExactState() < 1f)
                {
                    damageableDoor.ServerDamage(float.MaxValue, DoorDamageType.ServerCommand);
                }
            }
        }

    }
}
