using SplineEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class SplineAttachedEnemySpawnTrigger : MonoBehaviour
    {
        public static List<SplineAttachedEnemySpawnTrigger> ArmedTriggers = new List<SplineAttachedEnemySpawnTrigger>();

        [field: SerializeField] public BezierSpline SplineToAttachTo { get; set; }
        [field: SerializeField] public List<AbstractEnemy> EnemiesToSpawn { get; set; } = new List<AbstractEnemy>();
        [field: SerializeField] public List<EnemySpawnTemplate> EnemyTemplatesToSpawn { get; set; } = new List<EnemySpawnTemplate>();

        public int SplineTriggerPoint { get; private set; }
        public bool IsArmed { get; private set; }
        public bool HasTriggered { get; private set; }

        void OnEnable()
        {
            HasTriggered = false;
            ArmedTriggers.Add(this);
            IsArmed = true;

            SplinePath path = new SplinePath();
            SplineToAttachTo.GetEvenlySpacedPoints(1.0f, path);
            float bestDist = float.MaxValue, dist;
            for (int i = 0; i < path.Points.Count; ++i)
            {
                dist = (transform.position - path.Points[i]).sqrMagnitude;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    SplineTriggerPoint = i;
                }
            }

            foreach (var enemy in EnemiesToSpawn)
            {
                if (!enemy.StartsAwake)
                {
                    enemy.gameObject.SetActive(false);
                }
            }
        }

        private void OnDisable()
        {
            ArmedTriggers.Remove(this);
            IsArmed = false;
        }

        private void LateUpdate()
        {
            if(HasTriggered && IsArmed)
            {
                ArmedTriggers.Remove(this);
                IsArmed = false;
            }
        }

        public void Trigger()
        {
            if(HasTriggered)
            {
                return;
            }

            HasTriggered = true;

            foreach (var enemy in EnemiesToSpawn)
            {
                enemy.Spawn();
            }

            foreach (var template in EnemyTemplatesToSpawn)
            {
                template.Spawn();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(transform.position, Vector3.one);

            foreach(var enemy in EnemiesToSpawn)
            {
                Gizmos.DrawLine(transform.position, enemy.transform.position);
            }

            foreach(var template in EnemyTemplatesToSpawn)
            {
                Gizmos.DrawLine(transform.position, template.transform.position);
            }
        }
    }
}
