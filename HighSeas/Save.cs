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
    public class Save
    {
        public List<String> FoundItems { get; set; }
    }

    public class SaveLevel : LevelModule
    {
        public delegate void SaveEvent(Save saveFile, EventTime eventTime);
        public static event SaveEvent OnSaveEvent;

        public delegate void LoadSaveEvent(Save saveFile, EventTime eventTime);
        public static event LoadSaveEvent OnLoadSave;

        public string SourceFolder;
        public static List<String> FoundItems;
        public static String SaveFolder { get; private set; }
        public override IEnumerator OnLoadCoroutine()
        {
            FoundItems = new List<string>();
            EventManager.onLevelLoad += EventManager_onLevelLoad;
            EventManager.onLevelUnload += EventManager_onLevelUnload;
            return base.OnLoadCoroutine();
        }

        private void EventManager_onLevelLoad(LevelData levelData, EventTime eventTime)
        {
            if (eventTime == EventTime.OnEnd)
            {
                StatRetrieve();
            }
        }
        public void EventManager_onLevelUnload(LevelData levelData, EventTime eventTime)
        {
            if (level.data.id != "Master" && level.data.id != "CharacterSelection" && eventTime == EventTime.OnEnd)
            {
                StatDump();
            }
        }
        public static void StatRetrieve()
        {
            SaveFolder = Path.Combine(Application.streamingAssetsPath, "Mods/HighSeas");
            string jsonFile = Path.Combine(SaveFolder, "Save.json");
            if (File.Exists(jsonFile))
            {
                OnLoadSave?.Invoke(null, EventTime.OnStart);
                Save worldSave = JsonConvert.DeserializeObject<Save>(File.ReadAllText(jsonFile));
                FoundItems = worldSave?.FoundItems;
                foreach(string s in FoundItems)
                    Catalog.GetData<ItemData>(s).purchasable = true;
                OnLoadSave?.Invoke(worldSave, EventTime.OnEnd);
            }
        }
        public static void StatDump()
        {
            OnSaveEvent?.Invoke(null, EventTime.OnStart);
            Save HighSeasSave = new Save()
            {
                FoundItems = SaveLevel.FoundItems
            };
            string contents = JsonConvert.SerializeObject((object)HighSeasSave, Formatting.Indented);
            File.WriteAllText(Path.Combine(SaveFolder, "Save.json"), JsonConvert.SerializeObject(HighSeasSave));
            OnSaveEvent?.Invoke(null, EventTime.OnEnd);
        }
    }
}
