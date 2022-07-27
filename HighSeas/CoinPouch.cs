using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace HighSeas
{
    class CoinPouch : MonoBehaviour
    {
        public Item item;
        public Holder holder;
        public int MaxStartAmmount;
        public int MinStartAmmount;
        public int StartAmmount;

        public void Awake()
        {
            item = GetComponent<Item>();
            holder = GetComponentInChildren<Holder>();
            holder.Snapped += Holder_Snapped;
            holder.UnSnapped += Holder_UnSnapped;
            MinStartAmmount = 5;
            MaxStartAmmount = 10;
            StartAmmount = UnityEngine.Random.Range(MinStartAmmount, MaxStartAmmount);
        }

        private void Holder_UnSnapped(Item item)
        {
            item.GetComponentInChildren<MeshRenderer>().enabled = true;
        }

        private void Holder_Snapped(Item item)
        {
            item.GetComponentInChildren<MeshRenderer>().enabled = false;
        }
    }

    public class CoinPouchModule : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<CoinPouch>();
        }
    }
}
