using System;
using System.Collections;
using System.Collections.Generic;
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
        private Interactable.Action IgniteAction;

        public delegate void ExplodeEvent(Grenade grenade, EventTime eventTime);
        public event ExplodeEvent OnExplodeEvent;

        public delegate void IgniteEvent(Grenade grenade, bool Ignited, EventTime eventTime);
        public event IgniteEvent OnIgniteEvent;

        public void Setup(float newExplosionForce, float newExplosionRange, float newTimeToExplode, string newEffectID, Interactable.Action newIgniteAction)
        {
            ExplosionForce = newExplosionForce;
            ExplosionRange = newExplosionRange;
            TimeToExplode = newTimeToExplode;
            EffectID = newEffectID;
            IgniteAction = newIgniteAction;
        }

        public void Start()
        {
            item = GetComponent<Item>();
            item.OnHeldActionEvent += Item_OnHeldActionEvent;
            Timer = TimeToExplode;
        }

        public void Update()
        {
            if (Ignited)
                Timer -= 1 * Time.deltaTime;
            if (Timer <= 0 && !Exploded)
                Explode();
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if(action == IgniteAction)
            {
                OnIgniteEvent.Invoke(this, Ignited, EventTime.OnStart);
                Ignited = !Ignited;
                OnIgniteEvent.Invoke(this, Ignited, EventTime.OnEnd);
            }
        }

        private void Explode()
        {
            OnExplodeEvent.Invoke(this, EventTime.OnStart);
            Exploded = true;
            Catalog.GetData<EffectData>(EffectID).Spawn(item.transform).Play();
            foreach (Collider collider in Physics.OverlapSphere(item.transform.position, ExplosionRange))
            {
                if (collider.GetComponentInParent<Creature>() is Creature creature)
                    creature.ragdoll.SetState(Ragdoll.State.Destabilized);
                collider.attachedRigidbody?.AddExplosionForce(ExplosionForce, item.transform.position, ExplosionRange);
            }
            OnExplodeEvent.Invoke(this, EventTime.OnEnd);
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

        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            if (IgniteAction == "AltUse" || IgniteAction == "AlternateUse" || IgniteAction == "SpellWheel")
                _IgniteAction = Interactable.Action.AlternateUseStart;
            else _IgniteAction = Interactable.Action.UseStart;
            item.gameObject.AddComponent<Grenade>().Setup(ExplosionForce, ExplosionRange, TimeToExplode, EffectID, _IgniteAction);
        }
    }
}
