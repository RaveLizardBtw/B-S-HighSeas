using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;

namespace HighSeas
{
     class Pellet : MonoBehaviour
    {
        public Item item;
        public float _Damage;
        public void Setup(float Damage)
        {
            item = GetComponent<Item>();
            _Damage = Damage;
        }

        void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.GetComponentInParent<Creature>() is Creature creature && !creature.isPlayer)
            {
                creature.Damage(new CollisionInstance(new DamageStruct(DamageType.Pierce, _Damage)));
                creature.TryPush(Creature.PushType.Hit, collision.contacts[0].normal, 3);
            }
        }
    }

    public class PelletModule : ItemModule
    {
        public float Damage;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<Pellet>().Setup(Damage);
        }
    }
}
