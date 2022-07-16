using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThunderRoad;

namespace HighSeas
{
     class Bullet : MonoBehaviour
    {
        public Item item;
        public void Start()
        {
            item = GetComponent<Item>();
        }
    }
}
