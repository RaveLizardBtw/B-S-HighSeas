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
        public Button SaveButton;
        public static Text moneyText;
        public override void Init(MenuData menuData, Menu menu)
        {
            base.Init(menuData, menu);
            OverrideGunValuesToggle = menu.GetCustomReference("OverrideGunValuesToggle").GetComponent<Toggle>();
            NeedsBulletChangedToggle = menu.GetCustomReference("NeedsBulletToggle").GetComponent<Toggle>();
            NeedsPowderChangedToggle = menu.GetCustomReference("NeedsPowderToggle").GetComponent<Toggle>();
            SaveButton = menu.GetCustomReference("SaveButton").GetComponent<Button>();
            moneyText = menu.GetCustomReference("txt_Money").GetComponent<Text>();
            OverrideGunValuesToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(OverrideGunValues));
            NeedsBulletChangedToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(NeedsBulletChanged));
            NeedsPowderChangedToggle.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>(NeedsPowderChanged));
            SaveButton.onClick.AddListener(OnSavePressed);
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
        void OnSavePressed()
        {
            SaveLevel.Save();
        }
    }
}
