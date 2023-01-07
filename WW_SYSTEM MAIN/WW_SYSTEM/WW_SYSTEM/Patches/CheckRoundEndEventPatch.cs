using GameCore;
using HarmonyLib;
using MEC;
using PlayerRoles;
using RoundRestarting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WW_SYSTEM.API;
using WW_SYSTEM.EventHandlers;
using WW_SYSTEM.Events;
using Time = UnityEngine.Time;

namespace WW_SYSTEM.Patches
{
	[HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.Start))]
	public class CheckRoundEndEventPatch
	{
        private static IEnumerator<float> Process(RoundSummary roundSummary)
        {
            float time = Time.unscaledTime;
            while (roundSummary != null)
            {
   
                yield return Timing.WaitForSeconds(2.5f);
                if (!RoundSummary.RoundLock)
                {


                    if (RoundSummary.RoundInProgress() && Time.unscaledTime - time >= 15f)
                    {
                        RoundSummary.SumInfo_ClassList newList = default(RoundSummary.SumInfo_ClassList);
                        foreach (ReferenceHub hub in ReferenceHub.AllHubs)
                        {
                            switch (hub.GetTeam())
                            {
                                case Team.SCPs:
                                    if (hub.GetRoleId() == RoleTypeId.Scp0492)
                                    {
                                        newList.zombies++;
                                    }
                                    else
                                    {
                                        newList.scps_except_zombies++;
                                    }
                                    break;
                                case Team.FoundationForces:
                                    newList.mtf_and_guards++;
                                    break;
                                case Team.ChaosInsurgency:
                                    newList.chaos_insurgents++;
                                    break;
                                case Team.Scientists:
                                    newList.scientists++;
                                    break;
                                case Team.ClassD:
                                    newList.class_ds++;
                                    break;
                            }
                        }
                        yield return float.NegativeInfinity;
                        newList.warhead_kills = (AlphaWarheadController.Detonated ? AlphaWarheadController.Singleton.WarheadKills : -1);
                        yield return float.NegativeInfinity;
                        int MtfAndScience = newList.mtf_and_guards + newList.scientists;
                        int classDs = newList.chaos_insurgents + newList.class_ds;
                        int AllScps = newList.scps_except_zombies + newList.zombies;
                        int num4 = newList.class_ds + RoundSummary.EscapedClassD;
                        int num5 = newList.scientists + RoundSummary.EscapedScientists;
                        RoundSummary.SurvivingSCPs = newList.scps_except_zombies;
                        float num6 = (float)((roundSummary.classlistStart.class_ds == 0) ? 0 : (num4 / roundSummary.classlistStart.class_ds));
                        float num7 = (float)((roundSummary.classlistStart.scientists == 0) ? 1 : (num5 / roundSummary.classlistStart.scientists));
                        if (newList.class_ds <= 0 && MtfAndScience <= 0)
                        {
                            roundSummary._roundEnded = true;
                        }
                        else
                        {
                            int num8 = 0;
                            if (MtfAndScience > 0)
                            {
                                num8++;
                            }
                            if (classDs > 0)
                            {
                                num8++;
                            }
                            if (AllScps > 0)
                            {
                                num8++;
                            }
                            roundSummary._roundEnded = (num8 <= 1);
                        }

                        bool flag = MtfAndScience > 0;
                        bool flag2 = classDs > 0;
                        bool flag3 = AllScps > 0;
                        RoundSummary.LeadingTeam leadingTeam = RoundSummary.LeadingTeam.Draw;
                        if (flag)
                        {
                            leadingTeam = ((RoundSummary.EscapedScientists >= RoundSummary.EscapedClassD) ? RoundSummary.LeadingTeam.FacilityForces : RoundSummary.LeadingTeam.Draw);
                        }
                        else if (flag3 || (flag3 && flag2))
                        {
                            leadingTeam = ((RoundSummary.EscapedClassD > RoundSummary.SurvivingSCPs) ? RoundSummary.LeadingTeam.ChaosInsurgency : ((RoundSummary.SurvivingSCPs > RoundSummary.EscapedScientists) ? RoundSummary.LeadingTeam.Anomalies : RoundSummary.LeadingTeam.Draw));
                        }
                        else if (flag2)
                        {
                            leadingTeam = ((RoundSummary.EscapedClassD >= RoundSummary.EscapedScientists) ? RoundSummary.LeadingTeam.ChaosInsurgency : RoundSummary.LeadingTeam.Draw);
                        }
                        ROUND_END_STATUS round_END_STATUS = ROUND_END_STATUS.ON_GOING;
                        switch (leadingTeam)
                        {
                            case RoundSummary.LeadingTeam.FacilityForces:
                                round_END_STATUS = ROUND_END_STATUS.MTF_VICTORY;
                                break;
                            case RoundSummary.LeadingTeam.ChaosInsurgency:
                                round_END_STATUS = ROUND_END_STATUS.CI_VICTORY;
                                break;
                            case RoundSummary.LeadingTeam.Anomalies:
                                round_END_STATUS = ROUND_END_STATUS.SCP_VICTORY;
                                break;
                            case RoundSummary.LeadingTeam.Draw:
                                round_END_STATUS = ROUND_END_STATUS.NO_VICTORY;
                                break;
                        }
                 
                        if((MtfAndScience + classDs + AllScps) <= 1)
                        {
                            round_END_STATUS = ROUND_END_STATUS.FORCE_END;
                        }

                        CheckRoundEndEvent checkRoundEndEvent = new CheckRoundEndEvent(round_END_STATUS, MtfAndScience, classDs, AllScps, roundSummary._roundEnded);
                        EventManager.Manager.HandleEvent<IEventHandlerCheckRoundEnd>(checkRoundEndEvent);
                        roundSummary._roundEnded = checkRoundEndEvent.EndRound;
                        if (checkRoundEndEvent.Status == ROUND_END_STATUS.FORCE_END)
                        {
                            Round.ForceEndRound = true;
                            roundSummary._roundEnded = true;
                        }


                        if (checkRoundEndEvent.Status == ROUND_END_STATUS.ON_GOING)
                        {
                            roundSummary._roundEnded = false;

                        }
                        if (Round.ForceEndRound)
                        {
                            roundSummary._roundEnded = true;
                        }
                        Round.ForceEndRound = false;

                        if (roundSummary._roundEnded)
                        {
                            FriendlyFireConfig.PauseDetector = true;

                            leadingTeam = RoundSummary.LeadingTeam.Draw;
                            switch (checkRoundEndEvent.Status)
                            {
                                case ROUND_END_STATUS.MTF_VICTORY:
                                    leadingTeam = RoundSummary.LeadingTeam.FacilityForces;
                                    break;
                                case ROUND_END_STATUS.SCP_VICTORY:
                                    leadingTeam = RoundSummary.LeadingTeam.Anomalies;
                                    break;
                                case ROUND_END_STATUS.CI_VICTORY:
                                    leadingTeam = RoundSummary.LeadingTeam.ChaosInsurgency;
                                    break;
                            }
                            FriendlyFireConfig.PauseDetector = true;
                            string text = string.Concat(new object[]
                            {
                            "Round finished! Anomalies: ",
                            AllScps,
                            " | Chaos: ",
                            classDs,
                            " | Facility Forces: ",
                            MtfAndScience,
                            " | D escaped percentage: ",
                            num6,
                            " | S escaped percentage: : ",
                            num7
                            });
                            GameCore.Console.AddLog(text, Color.gray, false, GameCore.Console.ConsoleLogType.Log);
                            ServerLogs.AddLog(ServerLogs.Modules.Logger, text, ServerLogs.ServerLogType.GameEvent, false);
                            yield return Timing.WaitForSeconds(1.5f);
                            int num9 = Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);
                            if (roundSummary != null)
                            {
                                if (roundSummary._roundEnded)
                                {
                                    Round.WarheadLocked = false;
                                    RoundEndEvent ev = new RoundEndEvent(Round.RoundCount);
                                    EventManager.Manager.HandleEvent<IEventHandlerRoundEnd>(ev);
                                    Server.IgnoreTokenUserIds.Clear();
                                }
                                roundSummary.RpcShowRoundSummary(roundSummary.classlistStart, newList, leadingTeam, RoundSummary.EscapedClassD, RoundSummary.EscapedScientists, RoundSummary.KilledBySCPs, num9, (int)RoundStart.RoundLength.TotalSeconds);
                            }
                            yield return Timing.WaitForSeconds((float)(num9 - 1));
                            roundSummary.RpcDimScreen();
                            yield return Timing.WaitForSeconds(1f);
                            RoundRestart.InitiateRoundRestart();
                        }
                    }
                }
            
            }
        }


		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			foreach (CodeInstruction instruction in instructions)
			{
				if (instruction.opcode == OpCodes.Call)
				{
					if (instruction.operand != null
						&& instruction.operand is MethodBase methodBase
						&& methodBase.Name != nameof(RoundSummary._ProcessServerSideCode))
					{
						yield return instruction;
					}
					else
					{
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CheckRoundEndEventPatch), nameof(Process)));
						yield return new CodeInstruction(OpCodes.Ldarg_0);
						yield return new CodeInstruction(OpCodes.Call, AccessTools.FirstMethod(typeof(MECExtensionMethods2), (m) =>
						{
							var generics = m.GetGenericArguments();
							var paramseters = m.GetParameters();
							return m.Name == "CancelWith"
							&& generics.Length == 1
							&& paramseters.Length == 2
							&& paramseters[0].ParameterType == typeof(IEnumerator<float>)
							&& paramseters[1].ParameterType == generics[0];
						}).MakeGenericMethod(typeof(RoundSummary)));
					}
				}
				else
				{
					yield return instruction;
				}
			}
		}
	}
}
