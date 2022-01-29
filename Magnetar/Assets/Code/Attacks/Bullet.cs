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

        public VisualEffect effect;
        public float damage = 1.0f;
        public float lifeTime = 5.0f;

        private Vector3 localVelocity;
        private Vector3 globalVelocity = Vector3.zero;
        private float elapsedLife = 0.0f;
        private Magnet myMagnet;

        /// <summary>
        /// Sets up the bullet as if it's just spawned.
        /// </summary>
        /// <param name="isPlayerBullet">Handles appropiate collision flags.</param>
        /// <param name="bulletSpawnSpace">The parent of the spawned bullet.</param>
        /// <param name="spawnPosition">The world position to spawn the bullet.</param>
        /// <param name="spawnRotation">The world rotation to spawn the bullet.</param>
        /// <param name="initialVelocity">The world velocity of this bullet.</param>
        public void Initialize(bool isPlayerBullet, Transform bulletSpawnSpace, Vector3 spawnPosition, Quaternion spawnRotation, Vector3 initialVelocity)
        {
            if(isPlayerBullet)
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
            elapsedLife = 0.0f;
        }

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
            myMagnet = GetComponent<Magnet>();
        }

        private void Update()
        {
            if(myMagnet != null)
            {
                globalVelocity += myMagnet.DesiredWorldVelocity * Time.deltaTime;
            }

            Vector3 vel = localVelocity + transform.InverseTransformVector(globalVelocity);
            vel.y = 0;
            transform.Translate(vel * Time.deltaTime, Space.Self);

            elapsedLife += Time.deltaTime;
            if(elapsedLife > lifeTime)
            {
                BulletPool.Instance.ReturnToPool(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other);
        }
    }
}
