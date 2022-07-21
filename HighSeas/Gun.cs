using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace HighSeas
{
     class Gun : MonoBehaviour
    {
        public Item item;

        public bool CanShoot = true;
        public bool IsShooting = false;
        public int ShotsRemaining = 1;
        public int MaxAmo;
        public bool HasPowder = true;
        public bool isShot = false;
        public bool NeedsPowder;
        public bool NeedsBullet;
        public float Damage;

        private Collider BulletTriggerCollider;
        private Interactable.Action Action;
        private Interactable.Action ReloadAction;
        private float ShootForce;
        private float ShootCooldown;
        private string SFXID;
        private string ReloadSFXID;
        private string ProjectileID;
        private bool ProjectileUseGravity;
        private float ProjectileLife;
        private float Recoil;
        private Handle ReloadHandle;
        private Animator animator;

        public delegate void ShootEvent(Gun gun, EventTime eventTime);
        public event ShootEvent OnShootEvent;

        public delegate void ReloadEvent(Gun gun, EventTime eventTime);
        public event ReloadEvent OnReloadEvent;

        public void Setup(float newDamage, float newShootForce, float newShootCooldown, float newRecoil, string newSFXID, string newReloadSFXID, string newProjectileID, bool newProjectileUseGravity, bool newNeedsPowder, bool newNeedsBullet, float newProjectileLife, int newMaxAmo, Interactable.Action newAction, Interactable.Action newReloadAction)
        {
            ShootForce = newShootForce;
            ShootCooldown = newShootCooldown;
            SFXID = newSFXID;
            ReloadSFXID = newReloadSFXID;
            ProjectileID = newProjectileID;
            ProjectileUseGravity = newProjectileUseGravity;
            Action = newAction;
            ReloadAction = newReloadAction;
            MaxAmo = newMaxAmo;
            ProjectileLife = newProjectileLife;
            Recoil = newRecoil;
            NeedsPowder = newNeedsPowder;
            NeedsBullet = newNeedsBullet;
            Damage = newDamage;
        }

        public void Awake()
        {
            item = GetComponent<Item>();
            BulletTriggerCollider = item.GetCustomReference("BulletTriggerCollider").GetComponent<Collider>();
            BulletTriggerCollider.gameObject.AddComponent<ReloadTriggerCollider>();
            ReloadHandle = item.GetCustomReference("ReloadHandle").GetComponent<Handle>();
            animator = GetComponent<Animator>();
            item.OnHeldActionEvent += Item_OnHeldActionEvent;
            HighSeasLevelManager.AllGunsInLevel.Add(this);
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if ((action == Action && CanShoot && ShotsRemaining > 0 && HasPowder && !isShot && handle != ReloadHandle) || (action == Action && CanShoot && NeedsPowder && HasPowder && !NeedsBullet && !isShot && handle != ReloadHandle) || (action == Action && CanShoot && NeedsBullet && ShotsRemaining > 0 && !NeedsPowder && !isShot && handle != ReloadHandle) || (action == Action && CanShoot && !NeedsBullet && !NeedsPowder && !isShot && handle != ReloadHandle))
                Shoot();
            if (action == ReloadAction && isShot && handle != ReloadHandle)
                CockBack();
            else if (action == Action && isShot && handle == ReloadHandle)
                CockBack();
        }

        public void Shoot()
        {
            OnShootEvent?.Invoke(this, EventTime.OnStart);
            isShot = true;
            HasPowder = false;
            ShotsRemaining -= 1;
            IsShooting = true;
            animator.SetBool("Fired", true);
            item.rb.AddForce(-item.flyDirRef.forward * Recoil, ForceMode.Impulse);
            Catalog.GetData<EffectData>(SFXID).Spawn(item.flyDirRef).Play();
            Catalog.GetData<ItemData>(ProjectileID).SpawnAsync(Projectile =>
            {
                Projectile.gameObject.AddComponent<Pellet>().Setup(Damage, this);
                Projectile.transform.position = item.flyDirRef.position;
                Projectile.transform.rotation = Quaternion.identity;
                Projectile.rb.useGravity = ProjectileUseGravity;
                Projectile.Throw();
                Projectile.rb.AddForce(item.flyDirRef.forward * ShootForce, ForceMode.Impulse);
                Projectile.Despawn(ProjectileLife);
            });
            item.StartCoroutine(Cooldown(ShootCooldown));
            IsShooting = false;
            OnShootEvent?.Invoke(this, EventTime.OnStart);
        }

        public void CockBack()
        {
            isShot = false;
            Catalog.GetData<EffectData>(ReloadSFXID).Spawn(item.flyDirRef).Play();
            animator.SetBool("Fired", false);
        }
        public void AddPowder()
        {
            HasPowder = true;
        }
        public void Reload()
        {
            if (ShotsRemaining >= MaxAmo)
                return;
            OnReloadEvent?.Invoke(this, EventTime.OnStart);
            CanShoot = false;
            ShotsRemaining += 1;
            CanShoot = true;
            OnReloadEvent?.Invoke(this, EventTime.OnEnd);
        }

        public IEnumerator Cooldown(float CooldownTime)
        {
            CanShoot = false;
            yield return new WaitForSeconds(CooldownTime);
            CanShoot = true;
        }
        public string GetItemSpellID(Item item)
        {
            foreach (Imbue imbue in item.imbues)
            {
                if (imbue.spellCastBase != null)
                    return imbue.spellCastBase.id;
            }
            return null;
        }
    }

    public class GunModule : ItemModule
    {
        private Interactable.Action SetShootAction;
        private Interactable.Action SetReloadAction;

        public float ShootForce;
        public float ShootCooldown;
        public string SFXID;
        public string ReloadSFXID;
        public string Action;
        public string ReloadAction;
        public string ProjectileID;
        public bool ProjectileUseGravity;
        public float ProjectileLife;
        public float Recoil;
        public int MaxAmo;
        public bool NeedsPowder;
        public bool NeedsBullet;
        public float Damage;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            if (Action == "Use" || Action == "Trigger")
                SetShootAction = Interactable.Action.UseStart;
            else SetShootAction = Interactable.Action.AlternateUseStart;
            if (ReloadAction == "AltUse" || ReloadAction == "AlternateUse" || ReloadAction == "SpellWheel")
                SetReloadAction = Interactable.Action.AlternateUseStart;
            else SetReloadAction = Interactable.Action.UseStart;
            item.gameObject.AddComponent<Gun>().Setup(Damage, ShootForce, ShootCooldown, Recoil, SFXID, ReloadSFXID, ProjectileID, ProjectileUseGravity, NeedsPowder, NeedsBullet, ProjectileLife, MaxAmo, SetShootAction, SetReloadAction);
        }
    }
}
