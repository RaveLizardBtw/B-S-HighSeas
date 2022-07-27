using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace HighSeas
{
    public class Coin : MonoBehaviour
    {
        public delegate void CollectCoinEvent(Coin coin, EventTime eventTime);
        public static event CollectCoinEvent OnCollect;

        public Item item;
        public int Worth;

        public void Setup(int newWorth)
        {
            item = GetComponent<Item>();
            Worth = newWorth;
        }

        public void Collect()
        {
            OnCollect?.Invoke(this, EventTime.OnStart);
            HighSeasLevelManager.AddMoney(Worth);
            item.Despawn();
            OnCollect?.Invoke(this, EventTime.OnEnd);
        }
    }

    public class CoinModule : ItemModule
    {
        public int Worth;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<Coin>().Setup(Worth);
        }
    }

}
