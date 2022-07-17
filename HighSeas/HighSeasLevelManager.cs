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
        public bool OverrideGunValues;
        public bool NeedsPowder;
        public bool NeedsBullet;

        public override IEnumerator OnLoadCoroutine()
        {
            AllGunsInLevel = new List<Gun>() { };
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
    }
}
