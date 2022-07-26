using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Globalization;
using Newtonsoft.Json;
using ThunderRoad;
using UnityEngine;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.AddressableAssets;

namespace HighSeas
{
    public class DropChances
    {
        public List<String> ItemsToDrop { get; set; }
        public float DropChance { get; set; }
    }
    public class DropChancesLevel : LevelModule
    {
        public delegate void DropEvent(Creature creature, EventTime eventTime);
        public static event DropEvent OnDropEvent;

        public string SourceFolder;
        public static List<String> ItemsToDrop;
        public static float DropChance;
        public static String SaveFolder { get; private set; }
        public override IEnumerator OnLoadCoroutine()
        {
            ItemsToDrop = new List<string>();
            EventManager.onCreatureKill += EventManager_onCreatureKill;
            EventManager.onLevelLoad += EventManager_onLevelLoad;
            return base.OnLoadCoroutine();
        }
        private void EventManager_onLevelLoad(LevelData levelData, EventTime eventTime)
        {
            if (eventTime == EventTime.OnEnd)
            {
                StatRetrieve();
            }
        }
        private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            float Random = UnityEngine.Random.value;
            if (eventTime == EventTime.OnEnd && Random < DropChance)
            {
                OnDropEvent?.Invoke(creature, EventTime.OnStart);
                var ran = new System.Random();
                int index = ran.Next(ItemsToDrop.Count);
                Catalog.GetData<ItemData>(ItemsToDrop[index], false).SpawnAsync(item =>
                {
                    item.transform.position = creature.handLeft.transform.position;
                    item.transform.rotation = Quaternion.identity;
                });
                OnDropEvent?.Invoke(creature, EventTime.OnEnd);
            }
        }
        public static void StatRetrieve()
        {
            SaveFolder = Path.Combine(Application.streamingAssetsPath, "Mods/HighSeas");
            string jsonFile = Path.Combine(SaveFolder, "DropChances.json");
            Debug.Log(jsonFile);
            if (File.Exists(jsonFile))
            {
                DropChances drops = JsonConvert.DeserializeObject<DropChances>(File.ReadAllText(jsonFile));
                ItemsToDrop = drops?.ItemsToDrop;
                DropChance = (float)(drops?.DropChance);
                foreach (string s in ItemsToDrop)
                    HighSeasLevelManager.ItemsToDrop.Add(s);
                HighSeasLevelManager.DropChance = DropChance;
                Debug.Log("Stats Retrieved");
            }
        }
    }
}
