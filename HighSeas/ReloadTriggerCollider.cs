using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;

namespace HighSeas
{
    class ReloadTriggerCollider : MonoBehaviour
    {
        public Gun gun;

        public void Start()
        {
            gun = GetComponentInParent<Gun>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if(other.GetComponentInParent<Pellet>() is Pellet bullet)
            {
                if (gun.ShotsRemaining >= gun.MaxAmo)
                    return;
                gun.Reload();
                bullet.item.Despawn();
            }
        }
    }
}
