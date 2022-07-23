using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace HighSeas
{
    class Molotov : MonoBehaviour
    {
        public Item item;

        public void Awake()
        {
            item = GetComponent<Item>();
            item.mainCollisionHandler.OnCollisionStartEvent += MainCollisionHandler_OnCollisionStartEvent;
            LiveValue.Set<float>("Molotov.SmashForceRequired", 1, true);
        }

        private void MainCollisionHandler_OnCollisionStartEvent(CollisionInstance collisionInstance)
        {
            var Effect = Catalog.GetData<EffectData>("Molotov.Flames", false);
            GameObject Position = new GameObject("FlamePosition");
            Position.transform.position = item.transform.position;
            var SpawnedEffect = Effect.Spawn(Position.transform);
            SpawnedEffect.Play();
            SpawnedEffect.effects.ForEach((Action<Effect>)(i => i.gameObject.AddComponent<ParticleBurn>()));
        }
    }

    class ParticleBurn : MonoBehaviour
    {
        void OnParticleCollision(GameObject other)
        {
            if (other.GetComponentInParent<Creature>() is Creature creature)
                creature?.TryElectrocute(50f, 5, true, true, Catalog.GetData<EffectData>("ImbueFireRagdoll", true));
        }
    }

    public class MolotovModule : ItemModule
    {
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<Molotov>();
        }
    }
}
