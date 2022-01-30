using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public enum EWeaponSlot
    {
        Default,
        Magnetic_Positive,
        Magnetic_Negative,
    }

    [CreateAssetMenu(fileName = "Weapon.asset", menuName = "Magnetar/Equipable Weapon", order = 182)]
    public class EquipableWeapon : ScriptableObject
    {
        [Serializable]
        public class WeaponBulletEntry
        {
            public Bullet bullet;
            public Vector2 spawnOffset = Vector2.zero;
            public Vector2 initialDirection = new Vector2(0, 1);
            public float initialSpeed = 24.0f;
        }

        public string displayName = "Unknown Weapon";
        public EWeaponSlot slot = EWeaponSlot.Default;
        public int equipCost = 1;
        public List<WeaponBulletEntry> bullets = new List<WeaponBulletEntry>();
        public float shotCooldown = 0.5f;

        public void SpawnBullets(Transform bulletSpawnSpace, Vector3 spawnPosition, Quaternion spawnRotation, int playerID)
        {
            foreach (WeaponBulletEntry e in bullets)
            {
                Bullet b = BulletPool.Instance.GetBullet(e.bullet);
                // TODO: Velocity isn't working
                b.Initialize(bulletSpawnSpace, spawnPosition + spawnRotation * new Vector3(e.spawnOffset.x, 0.0f, e.spawnOffset.y), spawnRotation, new Vector3(e.initialDirection.x, 0.0f, e.initialDirection.y).normalized * e.initialSpeed, playerID);
            }
        }
    }

    public class RuntimeWeaponTrackingEntry
    {
        public EquipableWeapon Weapon { get; private set; }
        public float Cooldown { get; private set; }

        public RuntimeWeaponTrackingEntry(EquipableWeapon pWeapon)
        {
            Weapon = pWeapon;
        }

        public void Update()
        {
            Cooldown -= Time.deltaTime;
        }

        public bool TryShoot(Transform bulletSpawnSpace, Vector3 spawnPosition, Quaternion spawnRotation, int playerID = 0)
        {
            if (Weapon != null && Cooldown <= 0.0f)
            {
                Weapon.SpawnBullets(bulletSpawnSpace, spawnPosition, spawnRotation, playerID);
                Cooldown = Weapon.shotCooldown;
                return true;
            }

            return false;
        }
    }
}
