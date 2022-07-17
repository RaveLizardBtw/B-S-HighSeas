using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace HighSeas
{
    public class GunpowderModule : ItemModule
    {
        public List<LiquidData.Content> contents;
        public float maxLevel = 50f;
        public LayerMask collisionLayer;
        public float flowSpeed = 1f;
        public float flowMinAngle = 70f;
        public float flowMaxAngle = 100f;
        public string effectFlowId = "GunpowderFlow";

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            GunpowderLiquidContainer imbueLiquidContainer = item.gameObject.GetComponent<GunpowderLiquidContainer>();
            if (!imbueLiquidContainer)
                imbueLiquidContainer = item.gameObject.AddComponent<GunpowderLiquidContainer>();
            imbueLiquidContainer.contents = new List<LiquidData.Content>();
            foreach (LiquidData.Content content in contents)
                imbueLiquidContainer.contents.Add(content.Clone());
            imbueLiquidContainer.flow = item.GetCustomReference("PotionFlow");
            imbueLiquidContainer.flow.transform.position += imbueLiquidContainer.flow.transform.up * 0.04f;
            imbueLiquidContainer.effectFlowId = effectFlowId;
            imbueLiquidContainer.maxLevel = maxLevel;
            imbueLiquidContainer.collisionLayer = collisionLayer;
            imbueLiquidContainer.flowSpeed = flowSpeed;
            imbueLiquidContainer.flowMinAngle = flowMinAngle;
            imbueLiquidContainer.flowMaxAngle = flowMaxAngle;
            imbueLiquidContainer.Init(item);
        }
    }
    public class GunpowderParticleCollisionSpawner : ParticleCollisionSpawner
    {

        private void OnParticleCollision(GameObject other)
        {
            if ((effectParticle.module as EffectModuleParticle).collisionLayerMask != ((effectParticle.module as EffectModuleParticle).collisionLayerMask | 1 << other.layer))
                return;
            Item item = other.GetComponent<Item>();
            if (!item || item.imbues.Count == 0)
                return;
            int collisionEvents = particle.GetCollisionEvents(other, this.collisionEvents);
            for (int index = 0; index < collisionEvents; ++index)
            {
                Gun GunInParent = this.collisionEvents[index].colliderComponent.GetComponentInParent<Gun>();
                if (GunInParent && !GunInParent.HasPowder)
                    GunInParent.AddPowder();
                    
            }
        }
    }
    public class GunpowderLiquidContainer : LiquidContainer
    {
        public new void SpawnEffectFlowLoop()
        {
            base.SpawnEffectFlowLoop();
            foreach (Effect effect in effectFlow.effects)
            {
                if (effect is EffectParticle)
                {
                    foreach (EffectParticleChild child in (effect as EffectParticle).childs)
                    {
                        GunpowderParticleCollisionSpawner collisionSpawner = child.GetComponent<GunpowderParticleCollisionSpawner>();
                        if (child.particleCollisionSpawner && !(child.particleCollisionSpawner is GunpowderParticleCollisionSpawner))
                        {
                            bool active = child.particleCollisionSpawner.active;
                            Destroy(child.particleCollisionSpawner);
                            if (!collisionSpawner)
                                collisionSpawner = child.gameObject.AddComponent<GunpowderParticleCollisionSpawner>();
                            collisionSpawner.active = active;
                            child.particleCollisionSpawner = collisionSpawner;
                        }
                    }
                }
            }
        }

        protected override void OnLiquidFlowStart()
        {
            SpawnEffectFlowLoop();
            effectFlow.Play();
            liquidFlow = true;
        }
    }
    class Gunpowder : LiquidData
    {
        public override void OnLiquidReception(LiquidReceiver liquidReceiver, float dilution, LiquidContainer liquidContainer)
        {
            base.OnLiquidReception(liquidReceiver, dilution, liquidContainer);
        }
    }
}
