using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThunderRoad;
using UnityEngine;

namespace HighSeas
{
    class Grenade : MonoBehaviour
    {
        public Item item;

        public bool Ignited;
        public float Timer;
        public bool Exploded = false;

        private float ExplosionForce;
        private float ExplosionRange;
        private string EffectID;
        private float TimeToExplode;
        private bool InteractIgnite;
        private Interactable.Action IgniteAction;
        private GameObject SparksVFX;

        public delegate void ExplodeEvent(Grenade grenade, EventTime eventTime);
        public event ExplodeEvent OnExplodeEvent;

        public delegate void IgniteEvent(Grenade grenade, bool Ignited, EventTime eventTime);
        public event IgniteEvent OnIgniteEvent;

        public void Setup(bool newInteractIgnite, float newExplosionForce, float newExplosionRange, float newTimeToExplode, string newEffectID, Interactable.Action newIgniteAction)
        {
            ExplosionForce = newExplosionForce;
            ExplosionRange = newExplosionRange;
            TimeToExplode = newTimeToExplode;
            EffectID = newEffectID;
            IgniteAction = newIgniteAction;
            InteractIgnite = newInteractIgnite;
        }

        public void Start()
        {
            item = GetComponent<Item>();
            if (InteractIgnite)
                item.OnHeldActionEvent += Item_OnHeldActionEvent;
            item.mainCollisionHandler.OnCollisionStartEvent += MainCollisionHandler_OnCollisionStartEvent;
            SparksVFX = item.GetCustomReference("Sparks").gameObject;
            Timer = TimeToExplode;
        }

        private void MainCollisionHandler_OnCollisionStartEvent(CollisionInstance collisionInstance)
        {
            if (collisionInstance.targetCollider.material.name == "Blade" || collisionInstance.targetCollider.material.name == "Metal" && collisionInstance.impactVelocity.magnitude > 0.5f)
                Ignite();
        }

        public void Update()
        {
            if (Ignited)
                Timer -= 1 * Time.deltaTime;
            if (Timer <= 0 && !Exploded)
                Explode();
            if (!Ignited && item.imbues.Count > 0)
                foreach (Imbue imbue in item.imbues)
                    if (imbue.spellCastBase.id == "Fire")
                        Ignite();
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if(action == IgniteAction)
            {
                Ignite();
            }
        }

        public void Ignite()
        {
            OnIgniteEvent?.Invoke(this, Ignited, EventTime.OnStart);
            Ignited = !Ignited;
            SparksVFX.SetActive(Ignited);
            OnIgniteEvent?.Invoke(this, Ignited, EventTime.OnEnd);
        }
        public void Explode()
        {
            OnExplodeEvent?.Invoke(this, EventTime.OnStart);
            Exploded = true;
            GameObject obj = new GameObject();
            obj.transform.position = item.transform.position;
            GameObject.Destroy(obj, 2);

            Catalog.GetData<EffectData>(EffectID).Spawn(obj.transform).Play();
            foreach (Collider collider in Physics.OverlapSphere(item.transform.position, ExplosionRange))
            {
                float Random = UnityEngine.Random.value;
                if (collider.GetComponentInParent<Creature>() is Creature creature && !creature.isPlayer)
                {
                    creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                    if (Random < 0.9)
                    {
                        creature.Kill();
                        creature.ragdoll.GetPart(RagdollPart.Type.LeftArm).TrySlice();
                        creature.ragdoll.GetPart(RagdollPart.Type.RightLeg).TrySlice();
                    }
                }
                collider.attachedRigidbody?.AddExplosionForce(ExplosionForce * collider.attachedRigidbody.mass, item.transform.position, ExplosionRange);
            }
            item.Despawn(0.01f);
            OnExplodeEvent?.Invoke(this, EventTime.OnEnd);
        }
    }

    public class GrenadeModule : ItemModule
    {
        private Interactable.Action _IgniteAction;
        public float ExplosionForce;
        public float ExplosionRange;
        public string EffectID;
        public float TimeToExplode;
        public string IgniteAction;
        public bool InteractIgnite;

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            if (IgniteAction == "AltUse" || IgniteAction == "AlternateUse" || IgniteAction == "SpellWheel")
                _IgniteAction = Interactable.Action.AlternateUseStart;
            else _IgniteAction = Interactable.Action.UseStart;
            item.gameObject.AddComponent<Grenade>().Setup(InteractIgnite, ExplosionForce, ExplosionRange, TimeToExplode, EffectID, _IgniteAction);
        }
    }
}
