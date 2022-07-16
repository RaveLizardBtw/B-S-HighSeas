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

        private Transform ShootPoint;
        private Collider BulletTriggerCollider;
        private Interactable.Action ShootAction;
        private float ShootForce;
        private float ShootCooldown;
        private float ReloadTime;
        private string SFXID;
        private string ProjectileID;
        private bool ProjectileUseGravity;
        private float ProjectileLife;

        public delegate void ShootEvent(Gun gun, EventTime eventTime);
        public event ShootEvent OnShootEvent;

        public delegate void ReloadEvent(Gun gun, EventTime eventTime);
        public event ReloadEvent OnReloadEvent;

        public void Setup(float newShootForce, float newShootCooldown, float newReloadTime, string newSFXID, string newProjectileID, bool newProjectileUseGravity, float newProjectileLife, int newMaxAmo, Interactable.Action newShootAction)
        {
            ShootForce = newShootForce;
            ShootCooldown = newShootCooldown;
            SFXID = newSFXID;
            ProjectileID = newProjectileID;
            ProjectileUseGravity = newProjectileUseGravity;
            ShootAction = newShootAction;
            ReloadTime = newReloadTime;
            MaxAmo = newMaxAmo;
            ProjectileLife = newProjectileLife;
        }

        public void Awake()
        {
            item = GetComponent<Item>();
            ShootPoint = item.GetCustomReference("ShootPoint");
            BulletTriggerCollider = item.GetCustomReference("BulletTriggerCollider").GetComponent<Collider>();
            BulletTriggerCollider.gameObject.AddComponent<ReloadTriggerCollider>();
            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if (action == ShootAction && CanShoot)
            {
                Shoot();
            }
        }

        public void Shoot()
        {
            OnShootEvent.Invoke(this, EventTime.OnStart);
            IsShooting = true;
            Catalog.GetData<EffectData>(SFXID).Spawn(ShootPoint).Play();
            Catalog.GetData<ItemData>(ProjectileID).SpawnAsync(Projectile =>
            {
                Projectile.transform.position = ShootPoint.position;
                Projectile.transform.rotation = Quaternion.identity;
                Projectile.rb.useGravity = ProjectileUseGravity;
                Projectile.Throw();
                Projectile.rb.AddForce(ShootPoint.forward * ShootForce, ForceMode.Impulse);
                Projectile.Despawn(ProjectileLife);
            });
            item.StartCoroutine(Cooldown(ShootCooldown));
            IsShooting = false;
            OnShootEvent.Invoke(this, EventTime.OnStart);
        }

        public void Reload()
        {
            if (ShotsRemaining >= MaxAmo)
                return;
            OnReloadEvent.Invoke(this, EventTime.OnStart);
            CanShoot = false;
            ShotsRemaining += 1;
            CanShoot = true;
            OnReloadEvent.Invoke(this, EventTime.OnEnd);
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
        public float ReloadTime;
        public string SFXID;
        public string ShootAction;
        public string ReloadAction;
        public string ProjectileID;
        public bool ProjectileUseGravity;
        public float ProjectileLife;
        public int MaxAmo;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            if (ShootAction == "Use" || ShootAction == "Trigger")
                SetShootAction = Interactable.Action.UseStart;
            else SetShootAction = Interactable.Action.AlternateUseStart;
            item.gameObject.AddComponent<Gun>().Setup(ShootForce, ShootCooldown, ReloadTime, SFXID, ProjectileID, ProjectileUseGravity, ProjectileLife, MaxAmo, SetShootAction);
        }
    }
}
