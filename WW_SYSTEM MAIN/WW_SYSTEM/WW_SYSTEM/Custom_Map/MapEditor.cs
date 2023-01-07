using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using MEC;
using AdminToys;
using Mirror;
using WW_SYSTEM.API;
using PlayerStatsSystem;

namespace WW_SYSTEM.Custom_Map
{
    public static class MapEditor
    {
        private static AdminToyBase _LightSource;

        public static AdminToyBase LightSource
        {
            get
            {
                if (_LightSource == null)
                {
                    foreach (var item in Round.GetAllPrefabs)
                    {
                        if (item.TryGetComponent<AdminToyBase>(out var adminToyBase))
                        {
                            if (adminToyBase.CommandName.Contains("LightSource"))
                            {
                                _LightSource = adminToyBase;
                            }
                        }
                    }
                }
                return _LightSource;
            }

        }

        private static AdminToyBase _PrimitiveObject;

        public static AdminToyBase PrimitiveObject
        {
            get
            {
                if (_PrimitiveObject == null)
                {
                    foreach (var item in Round.GetAllPrefabs)
                    {
                        if (item.TryGetComponent<AdminToyBase>(out var adminToyBase))
                        {
                            if (adminToyBase.CommandName.Contains("PrimitiveObject"))
                            {
                                _PrimitiveObject = adminToyBase;
                            }
                        }
                    }
                }
                return _PrimitiveObject;
            }

        }

        private static Dictionary<string, AssetBundle> Server_Bundles = new Dictionary<string, AssetBundle>();
        public static List<GameObject> SpawnedObjects = new List<GameObject>();
        private static List<string> LoadInProgress = new List<string>();
        private static Dictionary<string, GameObject> ServerGameObjects = new Dictionary<string, GameObject>();


        public static IEnumerator<GameObject> GetAllBundlesObjects => ServerGameObjects.Values.GetEnumerator();

        private static CoroutineHandle LoadHandle = new CoroutineHandle();

        public static void UnloadAllBundles()
        {
            ServerGameObjects.Clear();
            DestroyAllSpawnedObjects();
            foreach (var item in Server_Bundles)
            {
                if (item.Value != null)
                {
                    try
                    {
                        item.Value.Unload(true);
                    }
                    catch (Exception)
                    {

                        continue;
                    }

                }
            }
            Server_Bundles.Clear();
        }

        public static bool TryGetBundleGameObject(string name, out GameObject gameObject)
        {
            name = name.ToUpper();
            if (ServerGameObjects.TryGetValue(name, out var obj))
            {
                gameObject = obj;
                return true;
            }
            gameObject = null;
            return false;
        }

        public static bool IsLoaded(string BundleName)
        {

            return Server_Bundles.ContainsKey(BundleName.ToUpper());
        }

        public static void LoadAssetsBundle(string url)
        {
            Info($"TRY LOAD: {url}");
            if (IsLoaded(GetFileName(url)))
            {
                throw new InvalidOperationException("Bundle is loaded!");
            }

            if (LoadInProgress.Contains(url))
            {
                throw new InvalidOperationException("Bundle load in process!");
            }
            LoadInProgress.Add(url);

            if (LoadHandle == new CoroutineHandle() || LoadHandle == null)
            {
                Info("STARTING LOAD HANDLE");
                LoadHandle = Timing.RunCoroutine(LoadBundles());
            }
            else
            {
                Warn("LOAD HANDLE IS NOT NULL! SKIPPING!");
            }
        }

        private static string GetFileName(string file)
        {
            int count = file.LastIndexOf("/") + 1;
            file = file.Remove(0, count);
            int startIndex = file.LastIndexOf(".");

            if (startIndex > 0) file = file.Remove(startIndex);
            return file.ToUpper();
        }


        static IEnumerator<float> LoadBundles()
        {
            if (LoadInProgress.Count <= 0)
            {
                Warn("URLS COUNT 0 BREAKING HANDLE");
                yield break;
            }
        LoadNext:
            var currenturl = LoadInProgress[0];
            LoadInProgress.RemoveAt(0);
            var name = GetFileName(currenturl);
            Info($"STARTING LOAD: {name}");
#pragma warning disable CS0618 // Type or member is obsolete
            using (WWW www = new WWW(currenturl))
#pragma warning restore CS0618 // Type or member is obsolete
            {
                Info($"[{name}] SENDING WEB REQUEST");
                yield return Timing.WaitUntilDone(www);

                if (!string.IsNullOrEmpty(www.error))
                {
                    Error($"[{name}] {www.error}");
                }

                if (www.assetBundle != null)
                {

                    RegisterBundle(www.assetBundle, name);
                    Info($"BUNDLE LOADED {name}");
                }
                else
                {
                    Error($"[{name}] WRONG BUNDLE FILE");
                }

            }
            if (LoadInProgress.Count > 0) goto LoadNext;
            Info("ALL BUNDLES LOADED!");
            LoadHandle = new CoroutineHandle();
            yield break;

        }


        private static void RegisterBundle(AssetBundle bundle, string name)
        {
            Server_Bundles.Add(name.ToUpper(), bundle);

            foreach (var item in bundle.LoadAllAssets<GameObject>())
            {
                var ObjName = item.name.ToUpper();

                if (!ServerGameObjects.ContainsKey(ObjName))
                {
                    ServerGameObjects.Add(ObjName, item);

                    Info($"{ObjName} LOADED!");
                }
            }
        }



        #region Logs
        private static void Info(string log)
        {
            Logger.Info("MapEditor", log);
        }
        private static void Warn(string log)
        {
            Logger.Warn("MapEditor", log);
        }
        private static void Error(string log)
        {
            Logger.Error("MapEditor", log);
        }

        #endregion

        public static Dictionary<string, int> GetAllTypes
        {
            get
            {
                var result = new Dictionary<string, int>();
                foreach (var item in Enum.GetValues(typeof(PrimitiveType)))
                {
                    var type = (PrimitiveType)item;
                    result.Add(type.ToString().ToUpper(), (int)type);

                }
                return result;
            }
        }

        public static bool HasAnimator(GameObject obj)
        {
            bool result = true;
            var count = obj.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                var o = obj.transform.GetChild(i);
                if (o.gameObject.GetComponent<Animator>() != null)
                {
                    result = true;
                }
                else
                {
                    if (o.parent != null)
                    {
                        result = HasAnimator(o.gameObject);
                    }
                }
                if (result) break;
            }


            return result;
        }

        public static bool TryLoadPrefab(string name, out GameObject GameObject, StandardDamageHandler damageHandler = null)
        {
            return TryLoadPrefab(name, out GameObject, Vector3.zero, damageHandler);

        }

        public static bool TryLoadPrefab(string name, out GameObject GameObject, Vector3 Scale, StandardDamageHandler damageHandler = null)
        {
            name = name.ToUpper();
            var types = GetAllTypes;
            if (TryGetBundleGameObject(name, out var obj))
            {


                var newObj = UnityEngine.Object.Instantiate(obj);

                if(Scale != Vector3.zero)
                {
                    newObj.transform.localScale = Scale;
                }

                var childs = GetAllChilds(newObj);
                foreach (var item in childs)
                {
                    var parent = item.transform.parent != null ? item.transform.parent : item.transform;

                    var hasnim = HasAnimator(item);
                    if (item.TryGetComponent<Light>(out var light))
                    {
                        var l = CreateLight(new Vector3(0, 0, 0), new Vector3(0, 0, 0), light.intensity, light.range, light.shadows != LightShadows.None, light.color);
                        var o = l.gameObject;
                        o.transform.SetParent(parent);
                        o.transform.position = item.transform.position;
                        o.transform.rotation = item.transform.rotation;
                        if (hasnim)
                        {
                            o.AddComponent<SyncObjectComponent>().Init(item, true);
                            l.NetworkMovementSmoothing = 60;
                        }

                        continue;
                    }
                    if (item.name.Contains(types.Keys.ToList(), out var found))
                    {
                        var type = (PrimitiveType)types[found.ToUpper()];

                        var color = Color.white;
                        if (item.TryGetComponent<MeshRenderer>(out var renderer) && renderer.material != null)
                        {
                            color = renderer.material.color;
                        }
                        if (item.TryGetComponent<Collider>(out var c))
                        {
                            GameObject.DestroyImmediate(c);
                        }

                        var o = CreatePrimitive(type, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1), color);

                        o.transform.SetParent(parent);
                        o.transform.position = item.transform.position;
                        o.transform.rotation = item.transform.rotation;
                        o.transform.localScale = item.transform.localScale;
                        if (hasnim)
                        {
                            o.gameObject.AddComponent<SyncObjectComponent>().Init(item, false);
                            o.NetworkMovementSmoothing = 60;
                        }
                    }
                    if (item.name.ToUpper().Contains("KILLZONE"))
                    {
                        if (item.TryGetComponent<Collider>(out var collider))
                        {
                            collider.isTrigger = true;
                            var zone = item.gameObject.AddComponent<KillZone>();
                            if (damageHandler == null)
                            {
                                damageHandler = new CustomReasonDamageHandler("KILL ZONE");
                            }

                            zone.DamageHandler = damageHandler;
                        }
                    }
                    if (item.name.ToUpper().Contains("PREFAB_"))
                    {
                        var prefName = item.name.ToUpper().Replace("PREFAB_", string.Empty).Replace(" ", string.Empty);
                        bool Found = false;
                        foreach (var pref in CustomNetworkManager.singleton.spawnPrefabs)
                        {
                            if(pref.name.ToUpper().Replace(" ", string.Empty).Equals(prefName, StringComparison.OrdinalIgnoreCase))
                            {
                                Logger.Info("MapEditor", $"Prefab {prefName} spawned!");
                                var spawn = GameObject.Instantiate(pref, item.transform.position, item.transform.rotation);
                                spawn.transform.SetParent(parent);
                                NetworkServer.Spawn(spawn);
                                Found = true;
                                break;
                            }
                        }
                        if(!Found)
                        Logger.Warn("MapEditor", $"Prefab {prefName} not found!");


                    }

                }
                GameObject = newObj;
                return true;
            }
            GameObject = null;
            return false;
        }

        public static List<GameObject> GetAllChilds(GameObject Obj)
        {
            List<GameObject> list = new List<GameObject>();
            List<GameObject> list2 = new List<GameObject>
            {
                Obj
            };
            for (; ; )
            {
                List<GameObject> list3 = new List<GameObject>();
                foreach (GameObject gameObject in list2)
                {
                    for (int i = 0; i < gameObject.transform.childCount; i++)
                    {
                        GameObject gameObject2 = gameObject.transform.GetChild(i).gameObject;
                        if (!list.Contains(gameObject2))
                        {
                            list.Add(gameObject2);
                            if (gameObject2.transform.childCount > 0 && !list3.Contains(gameObject2))
                            {
                                list3.Add(gameObject2);
                            }
                        }
                    }
                }
                if (list3.Count > 0)
                {
                    list2.Clear();
                    using (List<GameObject>.Enumerator enumerator2 = list3.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            GameObject item = enumerator2.Current;
                            list2.Add(item);
                        }
                        continue;
                    }

                }
                break;
            }
            return list;
        }

        public static void DestroyAllSpawnedObjects()
        {
            foreach (var item in SpawnedObjects)
            {
                if (item != null)
                {
                    NetworkServer.Destroy(item);
                }
            }
            SpawnedObjects.Clear();
        }

        public static AdminToyBase CreateLight(Vector3 pos, Vector3 rot, float intensity, float Range, bool LightShadows, Color color)
        {
            AdminToyBase toy = UnityEngine.Object.Instantiate(LightSource);
            LightSourceToy ptoy = toy.GetComponent<LightSourceToy>();
            ptoy.NetworkLightColor = color;
            ptoy.NetworkLightIntensity = intensity;
            ptoy.NetworkLightRange = Range;
            ptoy.NetworkLightShadows = LightShadows;
            ptoy.transform.position = pos;
            ptoy.transform.rotation = Quaternion.Euler(rot);
            ptoy.NetworkScale = ptoy.transform.localScale;
            NetworkServer.Spawn(toy.gameObject);
            SpawnedObjects.Add(toy.gameObject);
            return toy;
        }


        public static AdminToyBase CreatePrimitive(PrimitiveType type, Vector3 pos, Vector3 rot, Vector3 size, Color color)
        {
            AdminToyBase toy = UnityEngine.Object.Instantiate(PrimitiveObject);
            PrimitiveObjectToy ptoy = toy.GetComponent<PrimitiveObjectToy>();
            ptoy.NetworkPrimitiveType = type;
            ptoy.NetworkMaterialColor = color;
            ptoy.transform.position = pos;
            ptoy.transform.rotation = Quaternion.Euler(rot);
            ptoy.transform.localScale = size;
            ptoy.NetworkScale = ptoy.transform.localScale;
            NetworkServer.Spawn(toy.gameObject);
            SpawnedObjects.Add(toy.gameObject);
            return toy;
        }



    }

}
