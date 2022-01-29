using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class BombBullet : Bullet
    {
        [field: SerializeField] public float DetonationMagneticFieldMultiplier { get; private set; } = 32.0f;
        [field: SerializeField] public float ExplosionDecayTime { get; private set; } = 1.0f;

        Vector3 targetPosition;
        bool hasDetonated;

        public override void Initialize(Transform bulletSpawnSpace, Vector3 spawnPosition, Quaternion spawnRotation, Vector3 initialVelocity, int playerID)
        {
            base.Initialize(bulletSpawnSpace, spawnPosition, spawnRotation, initialVelocity, playerID);

            targetPosition = spawnPosition + spawnRotation * new Vector3(0.0f, 0.0f, 12.0f);
            /*
            Plane plane = new Plane(PlayableZoneController.Instance.transform.up, PlayableZoneController.Instance.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(plane.Raycast(ray, out float distance))
            {
                targetPosition = ray.GetPoint(distance);
            }
            else
            {
                Debug.LogError("Could not determine click point!");
            }
            */
        }

        protected override void FixedUpdate()
        {
            if (!hasDetonated)
            {
                Vector3 dir = targetPosition - transform.position;
                transform.Translate(dir * Time.deltaTime);

                if (dir.sqrMagnitude < 0.2f)
                {
                    OnBulletDestroy();
                    hasDetonated = true;
                }
            }
        }

        private IEnumerator Detonate()
        {
            if (MyMagnet != null)
            {
                float magnetStrengthMin = MyMagnet.magnetismStrength;
                float magnetStrengthMax = magnetStrengthMin * DetonationMagneticFieldMultiplier;

                for(float i = 0; i < 1.0f; i += Time.deltaTime / ExplosionDecayTime)
                {
                    MyMagnet.magnetismStrength = Mathf.Lerp(magnetStrengthMax, magnetStrengthMin, i);
                    yield return null;
                }

                MyMagnet.magnetismStrength = magnetStrengthMin;
                yield return null;
            }

            BulletPool.Instance.ReturnToPool(this);
        }

        protected override void OnBulletDestroy()
        {
            if (!hasDetonated)
            {
                StartCoroutine(Detonate());
                hasDetonated = true;
            }
        }
    }
}
