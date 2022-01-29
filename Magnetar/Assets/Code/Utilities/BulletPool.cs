using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Magnetar
{
    public class BulletPool : MonoBehaviour
    {
        public static BulletPool Instance { get; private set; }

        Dictionary<string, List<Bullet>> bulletPools = new Dictionary<string, List<Bullet>>();

        private List<Bullet> activeBullets = new List<Bullet>(2048);

        private void Awake()
        {
            if(Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        public T GetBullet<T>(T prefab) where T : Bullet
        {
            if(!bulletPools.ContainsKey(prefab.name))
            {
                bulletPools.Add(prefab.name, new List<Bullet>());
            }

            T bullet = bulletPools[prefab.name].FirstOrDefault(b => !b.IsActive && b is T) as T;
            if(bullet == null)
            {
                bullet = Instantiate(prefab);
                bulletPools[prefab.name].Add(bullet);
            }

            bullet.transform.SetParent(null);
            bullet.gameObject.SetActive(true);
            activeBullets.Add(bullet);
            return bullet;
        }

        public void ReturnToPool(Bullet bullet)
        {
            bullet.transform.SetParent(transform);
            bullet.gameObject.SetActive(false);
            activeBullets.Remove(bullet);
        }
    }
}
