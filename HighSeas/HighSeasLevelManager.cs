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
        public static int Money;
        public static float DropChance;
        public static bool OverrideGunValues;
        public static bool NeedsPowder;
        public static bool NeedsBullet;

        public override IEnumerator OnLoadCoroutine()
        {
            AllGunsInLevel = new List<Gun>();
            ItemsToDrop = new List<string>();
            return base.OnLoadCoroutine();
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

        public static void AddMoney(int ammount)
        {
            Money += ammount;
            HighSeasBook.moneyText.text = Money.ToString();
        }
    }

}
