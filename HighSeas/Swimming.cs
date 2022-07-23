using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;

namespace HighSeas
{
    class Swimming : LevelModule
    {
        public float SwimSpeed;
        public override IEnumerator OnPlayerSpawnCoroutine()
        {
            Player.local?.handLeft?.ragdollHand?.gameObject.AddComponent<SwimmingHand>().Setup(Player.local?.handLeft?.ragdollHand, SwimSpeed);
            Player.local?.handRight?.ragdollHand?.gameObject.AddComponent<SwimmingHand>().Setup(Player.local?.handRight?.ragdollHand, SwimSpeed);
            return base.OnPlayerSpawnCoroutine();
        }
    }

    class SwimmingHand : MonoBehaviour
    {
        public RagdollHand playerHand;
        public float SwimSpeed;
        public void Setup(RagdollHand setPlayerHand, float setSwimSpeed)
        {
            playerHand = setPlayerHand;
            SwimSpeed = setSwimSpeed;
        }

        public void Update()
        {
            if (!playerHand.waterHandler.inWater || !PlayerControl.GetHand(playerHand.side).usePressed)
                return;
            Player.local.locomotion.rb.AddForce(-playerHand.Velocity() * SwimSpeed, ForceMode.VelocityChange);
        }
    }
}
