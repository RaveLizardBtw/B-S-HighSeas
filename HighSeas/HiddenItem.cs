using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;

namespace HighSeas
{
    public class HiddenItem : MonoBehaviour
    {
        public Item item;

        public void Start()
        {
            item = GetComponent<Item>();
            item.OnGrabEvent += Item_OnGrabEvent;
        }

        private void Item_OnGrabEvent(Handle handle, RagdollHand ragdollHand)
        {
            Discover();
        }

        public void Discover()
        {
            if (!SaveLevel.FoundItems.Contains(item.data.id))
            {
                SaveLevel.FoundItems.Add(item.data.id);
                Catalog.GetData<ItemData>(item.itemId).purchasable = true;
                SaveLevel.StatDump();
            }
        }
    }
    class HiddenItemModule : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<HiddenItem>();
        }
    }

}
