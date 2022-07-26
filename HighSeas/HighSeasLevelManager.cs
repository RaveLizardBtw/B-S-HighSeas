using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace HighSeas
{
    class HighSeasLevelManager : LevelModule
    {
        public static List<Gun> AllGunsInLevel;
        public static List<String> ItemsToDrop;
        public static float DropChance;
        public static bool OverrideGunValues;
        public static bool NeedsPowder;
        public static bool NeedsBullet;

        public override IEnumerator OnLoadCoroutine()
        {
            EventManager.onCreatureKill += EventManager_onCreatureKill;
            AllGunsInLevel = new List<Gun>();
            ItemsToDrop = new List<string>();
            return base.OnLoadCoroutine();
        }

        private void EventManager_onCreatureKill(Creature creature, Player player, CollisionInstance collisionInstance, EventTime eventTime)
        {
            float Random = UnityEngine.Random.value;
            if(eventTime == EventTime.OnEnd && Random < DropChance)
            {
                var ran = new System.Random();
                int index = ran.Next(ItemsToDrop.Count);
                Catalog.GetData<ItemData>(ItemsToDrop[index], false).SpawnAsync(item =>
                {
                    item.transform.position = creature.handLeft.transform.position;
                    item.transform.rotation = Quaternion.identity;
                });
            }
        }

        public override void Update()
        {
            base.Update();
            if (!OverrideGunValues || AllGunsInLevel.Count <= 0)
                return;

            foreach (Gun gun in AllGunsInLevel)
            {
                gun.NeedsPowder = NeedsPowder;
                gun.NeedsBullet = NeedsBullet;
            }
        }
    }

}
