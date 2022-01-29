using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class PlayerSwitchBulletFactory : Bullet
    {
        public Bullet defaultPrefab;
        public Bullet playerPositivePrefab;
        public Bullet playerNegativePrefab;

        public override void Initialize(Transform bulletSpawnSpace, Vector3 spawnPosition, Quaternion spawnRotation, Vector3 initialVelocity, int playerID)
        {
            Bullet prefab;

            if(playerID == 1 && playerPositivePrefab != null)
            {
                prefab = playerPositivePrefab;
            }
            else if (playerID == 2 && playerNegativePrefab != null)
            {
                prefab = playerNegativePrefab;
            }
            else
            {
                prefab = defaultPrefab;
            }

            if(prefab != null)
            {
                Bullet b = BulletPool.Instance.GetBullet(prefab);
                b.Initialize(bulletSpawnSpace, spawnPosition, spawnRotation, initialVelocity, playerID);
            }

            OnBulletDestroy();
        }
    }
}
