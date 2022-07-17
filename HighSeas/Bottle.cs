using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;

namespace HighSeas
{
    class Bottle : MonoBehaviour
    {
        public Item item;
        public GameObject BottleLowerHalf;
        public Damager pierceDamager;
        public float BreakForce;

        public void Setup(float newBreakForce)
        {
            BreakForce = newBreakForce;
        }
        public void Awake()
        {
            item = GetComponent<Item>();
            BottleLowerHalf = item.GetCustomReference("BottleLowerHalf").gameObject;
            pierceDamager = item.GetCustomReference("pierceDamager").GetComponent<Damager>();
            item.mainCollisionHandler.OnCollisionStartEvent += MainCollisionHandler_OnCollisionStartEvent;
        }

        private void MainCollisionHandler_OnCollisionStartEvent(CollisionInstance collisionInstance)
        {
            if(collisionInstance.impactVelocity.magnitude > BreakForce)
            {
                BottleLowerHalf.SetActive(false);
                pierceDamager.gameObject.SetActive(true);
            }
        }
    }

    public class BottleModule : ItemModule 
    {
        public float BreakForce;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<Bottle>().Setup(BreakForce);
        }
    }
}
