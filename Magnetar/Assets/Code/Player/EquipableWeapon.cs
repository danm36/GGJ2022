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
            public Vector2 initialVelocity = new Vector2(0, 1);
        }

        public string displayName = "Unknown Weapon";
        public EWeaponSlot slot = EWeaponSlot.Default;
        public int equipCost = 1;
        public List<WeaponBulletEntry> bullets = new List<WeaponBulletEntry>();
        public float shotCooldown = 0.5f;

        public void SpawnBullets(bool isPlayerOwner, Vector3 spawnPosition, Quaternion spawnRotation, Vector3 spawnVelocity)
        {
            Debug.Log($"Spawning bullets with velocity {spawnVelocity}");
            foreach (WeaponBulletEntry e in bullets)
            {
                Bullet b = BulletPool.Instance.GetBullet(e.bullet);
                // TODO: Velocity isn't working
                b.Initialize(isPlayerOwner, spawnPosition, spawnRotation, spawnVelocity + spawnRotation * new Vector3(e.initialVelocity.x, 0.0f, e.initialVelocity.y));
            }
        }
    }
}
