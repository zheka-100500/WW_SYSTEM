using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using InventorySystem.Items.Firearms;
using Mirror;
using PlayerRoles;
using Respawning;
using UnityEngine;

namespace WW_SYSTEM.API
{
    public static class CustomNetwork
    {
        private static readonly Dictionary<Type, MethodInfo> WriterExtensionsValue = new Dictionary<Type, MethodInfo>();
        private static readonly Dictionary<string, ulong> SyncVarDirtyBitsValue = new Dictionary<string, ulong>();
        private static readonly ReadOnlyDictionary<Type, MethodInfo> ReadOnlyWriterExtensionsValue = new ReadOnlyDictionary<Type, MethodInfo>(WriterExtensionsValue);
        private static readonly ReadOnlyDictionary<string, ulong> ReadOnlySyncVarDirtyBitsValue = new ReadOnlyDictionary<string, ulong>(SyncVarDirtyBitsValue);
        private static MethodInfo setDirtyBitsMethodInfoValue = null;
        private static MethodInfo sendSpawnMessageMethodInfoValue = null;

        public static ReadOnlyDictionary<Type, MethodInfo> WriterExtensions
        {
            get
            {
                if (WriterExtensionsValue.Count == 0)
                {
                    foreach (MethodInfo method in typeof(NetworkWriterExtensions).GetMethods().Where(x => !x.IsGenericMethod && x.GetParameters()?.Length == 2))
                        WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);

                    foreach (MethodInfo method in typeof(GeneratedNetworkCode).GetMethods().Where(x => !x.IsGenericMethod && x.GetParameters()?.Length == 2 && x.ReturnType == typeof(void)))
                        WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);

                    foreach (Type serializer in typeof(ServerConsole).Assembly.GetTypes().Where(x => x.Name.EndsWith("Serializer")))
                    {
                        foreach (MethodInfo method in serializer.GetMethods().Where(x => x.ReturnType == typeof(void) && x.Name.StartsWith("Write")))
                            WriterExtensionsValue.Add(method.GetParameters().First(x => x.ParameterType != typeof(NetworkWriter)).ParameterType, method);
                    }
                }

                return ReadOnlyWriterExtensionsValue;
            }
        }


        public static ReadOnlyDictionary<string, ulong> SyncVarDirtyBits
        {
            get
            {
                if (SyncVarDirtyBitsValue.Count == 0)
                {
                    foreach (PropertyInfo property in typeof(ServerConsole).Assembly.GetTypes()
                        .SelectMany(x => x.GetProperties())
                        .Where(m => m.Name.StartsWith("Network")))
                    {
                        MethodInfo setMethod = property.GetSetMethod();
                        if (setMethod is null)
                            continue;
                        MethodBody methodBody = setMethod.GetMethodBody();
                        if (methodBody is null)
                            continue;
                        byte[] bytecodes = methodBody.GetILAsByteArray();
                        if (!SyncVarDirtyBitsValue.ContainsKey($"{property.Name}"))
                            SyncVarDirtyBitsValue.Add($"{property.Name}", bytecodes[bytecodes.LastIndexOf((byte)OpCodes.Ldc_I8.Value) + 1]);
                    }
                }

                return ReadOnlySyncVarDirtyBitsValue;
            }
        }

 
        public static MethodInfo SetDirtyBitsMethodInfo
        {
            get
            {
                if (setDirtyBitsMethodInfoValue is null)
                {
                    setDirtyBitsMethodInfoValue = typeof(NetworkBehaviour).GetMethod(nameof(NetworkBehaviour.SetDirtyBit));
                }

                return setDirtyBitsMethodInfoValue;
            }
        }

 
        public static MethodInfo SendSpawnMessageMethodInfo
        {
            get
            {
                if (sendSpawnMessageMethodInfoValue is null)
                {
                    sendSpawnMessageMethodInfoValue = typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.NonPublic | BindingFlags.Static);
                }

                return sendSpawnMessageMethodInfoValue;
            }
        }


        public static void Shake(this Player player) => SendFakeTargetRpc(player, AlphaWarheadController.Singleton.netIdentity, typeof(AlphaWarheadController), nameof(AlphaWarheadController.RpcShake), true);


        public static void PlayBeepSound(this Player player) => SendFakeTargetRpc(player, ReferenceHub.HostHub.networkIdentity, typeof(AmbientSoundPlayer), nameof(AmbientSoundPlayer.RpcPlaySound), 7);

  
        public static void SetPlayerInfoForTargetOnly(this Player player, Player target, string info) => player.SendFakeSyncVar(target.Hub.networkIdentity, typeof(NicknameSync), nameof(NicknameSync.Network_customPlayerInfoString), info);

   
        public static void PlayCassieAnnouncement(this Player player, string words, bool makeHold = false, bool makeNoise = true, bool isSubtitles = false) => SendFakeTargetRpc(player, RespawnEffectsController.AllControllers.Last().netIdentity, typeof(RespawnEffectsController), nameof(RespawnEffectsController.RpcCassieAnnouncement), words, makeHold, makeNoise, isSubtitles);


        public static void MessageTranslated(this Player player, string words, string translation, bool makeHold = false, bool makeNoise = true, bool isSubtitles = true)
        {
            StringBuilder annoucement = new StringBuilder();
            string[] cassies = words.Split('\n');
            string[] translations = translation.Split('\n');
            for (int i = 0; i < cassies.Count(); i++)
                annoucement.Append($"{translations[i]}<alpha=#00> {cassies[i].Replace(' ', ' ')} </alpha><split>");

            SendFakeTargetRpc(player, RespawnEffectsController.AllControllers.Last().netIdentity, typeof(RespawnEffectsController), nameof(RespawnEffectsController.RpcCassieAnnouncement), annoucement, makeHold, makeNoise, isSubtitles);
        }


        public static void SendFakeSyncVar(this Player target, NetworkIdentity behaviorOwner, Type targetType, string propertyName, object value)
        {
            void CustomSyncVarGenerator(NetworkWriter targetWriter)
            {
                targetWriter.WriteUInt64(SyncVarDirtyBits[$"{propertyName}"]);
                WriterExtensions[value.GetType()]?.Invoke(null, new[] { targetWriter, value });
            }

            PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
            PooledNetworkWriter writer2 = NetworkWriterPool.GetWriter();
            MakeCustomSyncWriter(behaviorOwner, targetType, null, CustomSyncVarGenerator, writer, writer2);
            target.Hub.networkIdentity.connectionToClient.Send(new UpdateVarsMessage() { netId = behaviorOwner.netId, payload = writer.ToArraySegment() });
            NetworkWriterPool.Recycle(writer);
            NetworkWriterPool.Recycle(writer2);
        }

        public static void ResyncSyncVar(NetworkIdentity behaviorOwner, Type targetType, string propertyName) => SetDirtyBitsMethodInfo.Invoke(behaviorOwner.gameObject.GetComponent(targetType), new object[] { SyncVarDirtyBits[$"{propertyName}"] });


        public static void SendFakeTargetRpc(Player target, NetworkIdentity behaviorOwner, Type targetType, string rpcName, params object[] values)
        {
            PooledNetworkWriter writer = NetworkWriterPool.GetWriter();

            foreach (object value in values)
                WriterExtensions[value.GetType()].Invoke(null, new[] { writer, value });

            RpcMessage msg = new RpcMessage
            {
                netId = behaviorOwner.netId,
                componentIndex = GetComponentIndex(behaviorOwner, targetType),
                functionHash = (targetType.FullName.GetStableHashCode() * 503) + rpcName.GetStableHashCode(),
                payload = writer.ToArraySegment(),
            };
            target.Conn.Send(msg, 0);
            NetworkWriterPool.Recycle(writer);
        }

        public static void SendFakeSyncObject(Player target, NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customAction)
        {
            PooledNetworkWriter writer = NetworkWriterPool.GetWriter();
            PooledNetworkWriter writer2 = NetworkWriterPool.GetWriter();
            MakeCustomSyncWriter(behaviorOwner, targetType, customAction, null, writer, writer2);
            target.Hub.networkIdentity.connectionToClient.Send(new UpdateVarsMessage() { netId = behaviorOwner.netId, payload = writer.ToArraySegment() });
            NetworkWriterPool.Recycle(writer);
            NetworkWriterPool.Recycle(writer2);
        }

     
        public static void EditNetworkObject(NetworkIdentity identity, Action<NetworkIdentity> customAction)
        {
            customAction.Invoke(identity);

            ObjectDestroyMessage objectDestroyMessage = new ObjectDestroyMessage
            {
                netId = identity.netId,
            };
            foreach (Player ply in Round.GetPlayers())
            {
                ply.Conn.Send(objectDestroyMessage, 0);
                SendSpawnMessageMethodInfo.Invoke(null, new object[] { identity, ply.Conn });
            }
        }

  
        private static int GetComponentIndex(NetworkIdentity identity, Type type)
        {
            return Array.FindIndex(identity.NetworkBehaviours, (x) => x.GetType() == type);
        }


        private static void MakeCustomSyncWriter(NetworkIdentity behaviorOwner, Type targetType, Action<NetworkWriter> customSyncObject, Action<NetworkWriter> customSyncVar, NetworkWriter owner, NetworkWriter observer)
        {
            byte behaviorDirty = 0;
            NetworkBehaviour behaviour = null;


            for (int i = 0; i < behaviorOwner.NetworkBehaviours.Length; i++)
            {
                if (behaviorOwner.NetworkBehaviours[i].GetType() == targetType)
                {
                    behaviour = behaviorOwner.NetworkBehaviours[i];
                    behaviorDirty = (byte)i;
                    break;
                }
            }


            owner.WriteByte(behaviorDirty);


            int position = owner.Position;
            owner.WriteInt32(0);
            int position2 = owner.Position;


            if (customSyncObject != null)
                customSyncObject.Invoke(owner);
            else
                behaviour.SerializeObjectsDelta(owner);

           
            customSyncVar?.Invoke(owner);

         
            int position3 = owner.Position;
            owner.Position = position;
            owner.WriteInt32(position3 - position2);
            owner.Position = position3;

       
            if (behaviour.syncMode != SyncMode.Observers)
            {
                ArraySegment<byte> arraySegment = owner.ToArraySegment();
                observer.WriteBytes(arraySegment.Array, position, owner.Position - position);
            }
        }
    }
}

