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

        private Collider BulletTriggerCollider;
        private Interactable.Action Action;
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

        public void Setup(float newShootForce, float newShootCooldown, float newRecoil, string newSFXID, string newReloadSFXID, string newProjectileID, bool newProjectileUseGravity, float newProjectileLife, int newMaxAmo, Interactable.Action newAction)
        {
            ShootForce = newShootForce;
            ShootCooldown = newShootCooldown;
            SFXID = newSFXID;
            ReloadSFXID = newReloadSFXID;
            ProjectileID = newProjectileID;
            ProjectileUseGravity = newProjectileUseGravity;
            Action = newAction;
            MaxAmo = newMaxAmo;
            ProjectileLife = newProjectileLife;
            Recoil = newRecoil;
        }

        public void Awake()
        {
            item = GetComponent<Item>();
            BulletTriggerCollider = item.GetCustomReference("BulletTriggerCollider").GetComponent<Collider>();
            BulletTriggerCollider.gameObject.AddComponent<ReloadTriggerCollider>();
            ReloadHandle = item.GetCustomReference("ReloadHandle").GetComponent<Handle>();
            animator = GetComponent<Animator>();
            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if (action == Action && CanShoot && ShotsRemaining > 0 && HasPowder && !isShot && handle != ReloadHandle)
            {
                Shoot();
            }
            if(action == Action && isShot && handle == ReloadHandle)
            {
                isShot = false;
                Catalog.GetData<EffectData>(ReloadSFXID).Spawn(item.flyDirRef).Play();
                animator.SetBool("Fired", false);
            }
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
    }

    public class GunModule : ItemModule
    {
        private Interactable.Action SetShootAction;

        public float ShootForce;
        public float ShootCooldown;
        public string SFXID;
        public string ReloadSFXID;
        public string Action;
        public string ProjectileID;
        public bool ProjectileUseGravity;
        public float ProjectileLife;
        public float Recoil;
        public int MaxAmo;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            if (Action == "Use" || Action == "Trigger")
                SetShootAction = Interactable.Action.UseStart;
            else SetShootAction = Interactable.Action.AlternateUseStart;
            item.gameObject.AddComponent<Gun>().Setup(ShootForce, ShootCooldown, Recoil, SFXID, ReloadSFXID, ProjectileID, ProjectileUseGravity, ProjectileLife, MaxAmo, SetShootAction);
        }
    }
}
