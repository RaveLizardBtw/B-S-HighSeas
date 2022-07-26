using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using RainyReignGames.RevealMask;
using System.Linq;
using UnityEngine.UI;

namespace HighSeas
{
    public class GunpowderModule : ItemModule
    {
        public static bool AltPressed;

        public LiquidContainer container;
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
            item.OnHeldActionEvent += Item_OnHeldActionEvent;
            GunpowderLiquidContainer liquidContainer = item.gameObject.GetComponent<GunpowderLiquidContainer>();
            if (!liquidContainer)
                liquidContainer = item.gameObject.AddComponent<GunpowderLiquidContainer>();
            liquidContainer.contents = new List<LiquidData.Content>();
            foreach (LiquidData.Content content in contents)
                liquidContainer.contents.Add(content.Clone());
            liquidContainer.liquidLevelText = item.GetCustomReference("PotionIndicator").GetComponent<Text>();
            liquidContainer.flow = item.GetCustomReference("PotionFlow");
            //liquidContainer.flow.transform.position += liquidContainer.flow.transform.up * 0.04f;
            liquidContainer.effectFlowId = effectFlowId;
            liquidContainer.maxLevel = maxLevel;
            liquidContainer.collisionLayer = collisionLayer;
            liquidContainer.flowSpeed = flowSpeed;
            liquidContainer.flowMinAngle = flowMinAngle;
            liquidContainer.flowMaxAngle = flowMaxAngle;
            container = liquidContainer;
            liquidContainer.Init(item);
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if (action == Interactable.Action.AlternateUseStart)
                AltPressed = true;
            if (action == Interactable.Action.AlternateUseStop)
            {
                AltPressed = false;
                container.effectFlow.Stop();
                container.liquidFlow = false;
            }
        }
    }
    public class GunpowderParticleCollisionSpawner : ParticleCollisionSpawner
    {
        public ParticleSystem part;
        public void Start()
        {
            part = GetComponent<ParticleSystem>();
            collisionEvents = new List<ParticleCollisionEvent>();
        }
        private void OnParticleCollision(GameObject other)
        {
            if ((effectParticle.module as EffectModuleParticle).collisionLayerMask != ((effectParticle.module as EffectModuleParticle).collisionLayerMask | 1 << other.layer))
                return;
            Item item = other.GetComponent<Item>();
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
                    if (!effect.gameObject.GetComponent<GunpowderParticleCollisionSpawner>())
                        effect.gameObject.AddComponent<GunpowderParticleCollisionSpawner>();
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
            if (!GunpowderModule.AltPressed)
                return;
            SpawnEffectFlowLoop();
            effectFlow.Play();
            liquidFlow = true;
        }
    }
}
