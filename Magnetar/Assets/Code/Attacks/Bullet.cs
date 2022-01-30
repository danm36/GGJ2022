using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Magnetar
{
    [RequireComponent(typeof(Collider))]
    public class Bullet : MonoBehaviour
    {
        public bool IsActive => elapsedLife <= lifeTime;

        public float damage = 1.0f;
        public float lifeTime = 5.0f;

        private Vector3 localVelocity;
        private Vector3 globalVelocity = Vector3.zero;
        private float elapsedLife = 0.0f;
        public Magnet MyMagnet { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        /// <summary>
        /// Sets up the bullet as if it's just spawned.
        /// </summary>
        /// <param name="bulletSpawnSpace">The parent of the spawned bullet.</param>
        /// <param name="spawnPosition">The world position to spawn the bullet.</param>
        /// <param name="spawnRotation">The world rotation to spawn the bullet.</param>
        /// <param name="initialVelocity">The world velocity of this bullet.</param>
        /// <param name="playerID">Handles appropiate collision flags.</param>
        public virtual void Initialize(Transform bulletSpawnSpace, Vector3 spawnPosition, Quaternion spawnRotation, Vector3 initialVelocity, int playerID)
        {
            if(playerID > 0)
            {
                gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
            }

            transform.SetParent(bulletSpawnSpace);
            transform.position = spawnPosition;
            transform.rotation = spawnRotation;
            localVelocity = initialVelocity;
            globalVelocity = Vector3.zero;
            Rigidbody.velocity = Vector3.zero;
            elapsedLife = 0.0f;
        }

        private void Awake()
        {
            MyMagnet = GetComponent<Magnet>();
            Rigidbody = GetComponent<Rigidbody>();
        }

        protected virtual void Update()
        {
            if (MyMagnet != null)
            {
                globalVelocity += MyMagnet.DesiredWorldVelocity * Time.deltaTime;
            }

            elapsedLife += Time.deltaTime;
            if(elapsedLife > lifeTime)
            {
                OnBulletDestroy();
            }
        }

        protected virtual void FixedUpdate()
        {
            Vector3 vel = localVelocity + transform.InverseTransformVector(globalVelocity);
            vel.y = 0;
            transform.Translate(vel * Time.deltaTime, Space.Self);
        }

        protected virtual void OnBulletDestroy()
        {
            BulletPool.Instance.ReturnToPool(this);
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            HealthComponent hp = collision.collider.GetComponent<HealthComponent>();
            if (hp != null)
            {
                hp.Hurt(collision, damage);
                OnBulletDestroy();
            }
        }
    }
}
