using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Magnetar
{
    public enum EMagnetType
    {
        NotMagnetic = 0,
        Positive = 1,
        Negative = 2,
        Monopole = 3,
    }

    [RequireComponent(typeof(Collider))]
    public class Bullet : MonoBehaviour
    {
        public bool IsActive => elapsedLife <= lifeTime;

        public VisualEffect effect;
        public EMagnetType magnetismType = EMagnetType.NotMagnetic;
        public float magnetismStrength = 0.0f;
        public float damage = 1.0f;
        public float lifeTime = 5.0f;

        private Vector3 velocity;
        private float elapsedLife = 0.0f;

        /// <summary>
        /// Sets up the bullet as if it's just spawned.
        /// </summary>
        /// <param name="isPlayerBullet">Handles appropiate collision flags.</param>
        /// <param name="spawnPosition">The world position to spawn the bullet.</param>
        /// <param name="spawnRotation">The world rotation to spawn the bullet.</param>
        /// <param name="initialVelocity">The world velocity of this bullet.</param>
        public void Initialize(bool isPlayerBullet, Vector3 spawnPosition, Quaternion spawnRotation, Vector3 initialVelocity)
        {
            if(isPlayerBullet)
            {
                gameObject.layer = LayerMask.NameToLayer("PlayerBullet");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("EnemyBullet");
            }

            transform.position = spawnPosition;
            transform.rotation = spawnRotation;
            velocity = initialVelocity;
            elapsedLife = 0.0f;
        }

        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void Update()
        {
            transform.Translate(velocity * Time.deltaTime);
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
