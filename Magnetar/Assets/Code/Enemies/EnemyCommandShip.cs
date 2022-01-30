using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class EnemyCommandShip : AbstractEnemy
    {
        [field: SerializeField] public GameObject Model { get; private set; }

        [field: SerializeField] public List<AbstractEnemy> EnemiesToSpawn { get; private set; } = new List<AbstractEnemy>();
        [field: SerializeField] public List<Transform> EnemySpawnPoints { get; private set; } = new List<Transform>();
        [field: SerializeField] public float TimeBetweenEnemySpawns { get; private set; } = 10.0f;

        [field: SerializeField] public Transform PositiveShootPoint { get; private set; }
        [field: SerializeField] public Bullet PositiveBullet { get; private set; }
        [field: SerializeField] public Transform NegativeShootPoint { get; private set; }
        [field: SerializeField] public Bullet NegativeBullet { get; private set; }
        [field: SerializeField] public float TimeBetweenBursts { get; private set; } = 10.0f;
        [field: SerializeField] public int ShotsPerBurst { get; private set; } = 10;
        [field: SerializeField] public float TimeBetweenShots { get; private set; } = 0.2f;
        [field: SerializeField] public float ShotSpeed { get; private set; } = 24.0f;

        public bool IsDead { get; private set; }

        private float enemySpawnTimer = 8.0f;
        private float shootTimer = 3.0f;
        private bool isShooting = false;

        protected override void OnUpdate()
        {
            if(IsDead)
            {
                return;
            }

            enemySpawnTimer -= Time.deltaTime;
            if(enemySpawnTimer <= 0)
            {
                Debug.Log("Spawning Enemies");
                enemySpawnTimer = TimeBetweenEnemySpawns;
                AbstractEnemy toSpawn = EnemiesToSpawn.Random();

                foreach(var spawnPoint in EnemySpawnPoints)
                {
                    AbstractEnemy e = Instantiate(toSpawn, spawnPoint.position, spawnPoint.rotation);
                    e.MotionMode = EMotionMode.MoveDownOverTime;
                    e.Spawn(true);
                }
            }

            shootTimer -= Time.deltaTime;
            if(!isShooting && shootTimer <= 0)
            {
                StartCoroutine(PerformShoot());
            }
        }

        private IEnumerator PerformShoot()
        {
            isShooting = true;

            // Positive
            for (int i = 0; i < ShotsPerBurst; ++i)
            {
                Vector3 toPlayer = (PositiveShootPoint.transform.position - PlayableZoneController.Instance.Players.Random().transform.position).normalized;

                Bullet b = BulletPool.Instance.GetBullet(PositiveBullet);
                b.Initialize(PlayableZoneController.Instance.transform, PositiveShootPoint.transform.position, PositiveShootPoint.transform.rotation, toPlayer * ShotSpeed, 0);
                AudioSourceComponent.PlayOneShot(ShootSoundEffect);

                yield return new WaitForSeconds(TimeBetweenShots);
            }

            yield return new WaitForSeconds(TimeBetweenShots * 4.0f);

            // Negative
            for (int i = 0; i < ShotsPerBurst; ++i)
            {
                Vector3 toPlayer = (NegativeShootPoint.transform.position - PlayableZoneController.Instance.Players.Random().transform.position).normalized;

                Bullet b = BulletPool.Instance.GetBullet(NegativeBullet);
                b.Initialize(PlayableZoneController.Instance.transform, NegativeShootPoint.transform.position, NegativeShootPoint.transform.rotation, toPlayer * ShotSpeed, 0);
                AudioSourceComponent.PlayOneShot(ShootSoundEffect);

                yield return new WaitForSeconds(TimeBetweenShots);
            }

            shootTimer = TimeBetweenBursts;
            isShooting = false;
        }

        protected override void OnDeath(Collision collision)
        {
            IsDead = true;
            StartCoroutine(DoDeathEffects());
        }

        private IEnumerator DoDeathEffects()
        {
            if (DeathEffect != null)
            {
                Instantiate(DeathEffect, transform.position, transform.rotation, transform.parent);
            }

            Model.SetActive(false);

            yield return new WaitForSeconds(2.0f);

            PlayableZoneController.Instance.MissionComplete();
            Destroy(gameObject);
        }
    }
}
