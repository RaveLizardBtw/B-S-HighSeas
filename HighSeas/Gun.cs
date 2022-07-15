using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;

namespace HighSeas
{
    public class Gun : MonoBehaviour
    {
        public Item item;

        private bool CanShoot = true;
        private bool IsShooting = false;
        private int ShotsRemaining = 1;

        private Transform ShootPoint;
        private Interactable.Action ShootAction;
        private Interactable.Action ReloadAction;
        private float ShootForce;
        private float ShootCooldown;
        private float ReloadTime;
        private string SFXID;
        private string ProjectileID;
        private bool ProjectileUseGravity;
        private float ProjectileLife;
        private int MaxAmo;

        public delegate void ShootEvent(Gun gun, EventTime eventTime);
        public event ShootEvent OnShootEvent;

        public delegate void ReloadEvent(Gun gun, EventTime eventTime);
        public event ReloadEvent OnReloadEvent;

        public void Setup(float newShootForce, float newShootCooldown, float newReloadTime, string newSFXID, string newProjectileID, bool newProjectileUseGravity, float newProjectileLife, int newMaxAmo, Interactable.Action newShootAction, Interactable.Action newReloadAction)
        {
            ShootForce = newShootForce;
            ShootCooldown = newShootCooldown;
            SFXID = newSFXID;
            ProjectileID = newProjectileID;
            ProjectileUseGravity = newProjectileUseGravity;
            ShootAction = newShootAction;
            ReloadAction = newReloadAction;
            ReloadTime = newReloadTime;
            MaxAmo = newMaxAmo;
            ProjectileLife = newProjectileLife;
        }

        public void Awake()
        {
            item = GetComponent<Item>();
            item.OnHeldActionEvent += Item_OnHeldActionEvent;
        }

        private void Item_OnHeldActionEvent(RagdollHand ragdollHand, Handle handle, Interactable.Action action)
        {
            if (action == ShootAction && CanShoot)
            {
                Shoot();
            }
            else if(action == ReloadAction)
            {
                item.StartCoroutine(Reload());
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

        public IEnumerator Reload()
        {
            OnReloadEvent.Invoke(this, EventTime.OnStart);
            CanShoot = false;
            yield return new WaitForSeconds(ReloadTime);
            ShotsRemaining = MaxAmo;
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
        private Interactable.Action SetReloadAction;

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
            if (ReloadAction == "AltUse" || ReloadAction == "AlternateUse" || ReloadAction == "SpellWheel")
                SetReloadAction = Interactable.Action.AlternateUseStart;
            item.gameObject.AddComponent<Gun>().Setup(ShootForce, ShootCooldown, ReloadTime, SFXID, ProjectileID, ProjectileUseGravity, ProjectileLife, MaxAmo, SetShootAction, SetReloadAction);
        }
    }
}
