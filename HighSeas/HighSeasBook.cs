using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using UnityEngine.UI;

namespace HighSeas
{
    class HighSeasBook : MenuModule
    {
        public Toggle OverrideGunValuesToggle;
        public Toggle NeedsBulletChangedToggle;
        public Toggle NeedsPowderChangedToggle;
        public override void Init(MenuData menuData, Menu menu)
        {
            base.Init(menuData, menu);
            OverrideGunValuesToggle = menu.GetCustomReference("OverrideGunValuesToggle").GetComponent<Toggle>();
            NeedsBulletChangedToggle = menu.GetCustomReference("NeedsBulletToggle").GetComponent<Toggle>();
            NeedsPowderChangedToggle = menu.GetCustomReference("NeedsPowderToggle").GetComponent<Toggle>();
            OverrideGunValuesToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OverrideGunValues));
            NeedsBulletChangedToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(NeedsBulletChanged));
            NeedsPowderChangedToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(NeedsPowderChanged));
        }
        void OverrideGunValues(bool active)
        {
            HighSeasLevelManager.OverrideGunValues = active;
        }
        void NeedsBulletChanged(bool active)
        {
            HighSeasLevelManager.NeedsBullet = active;
        }
        void NeedsPowderChanged(bool active)
        {
            HighSeasLevelManager.NeedsPowder = active;
        }
    }
}
