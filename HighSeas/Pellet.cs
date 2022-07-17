using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;

namespace HighSeas
{
     class Pellet : MonoBehaviour
    {
        public Gun gun;
        public Item item;
        public float _Damage;
        public void Setup(float Damage, Gun newGun)
        {
            item = GetComponent<Item>();
            gun = newGun;
            _Damage = Damage;
        }

        void OnCollisionEnter(Collision collision)
        {
            if(collision.collider.GetComponentInParent<Creature>() is Creature creature && !creature.isPlayer && collision.relativeVelocity.magnitude > 1)
            {
                creature.Damage(new CollisionInstance(new DamageStruct(DamageType.Pierce, _Damage)));
                creature.TryPush(Creature.PushType.Hit, collision.contacts[0].normal, 3);
            }
        }

        public void Update()
        {
            if (gun.GetItemSpellID(gun.item) == null)
                return;
            foreach (Imbue imbue in item.imbues)
            {
                imbue.Transfer(Catalog.GetData<SpellCastCharge>(gun.GetItemSpellID(gun.item)), imbue.maxEnergy);
            }
        }
    }
}
