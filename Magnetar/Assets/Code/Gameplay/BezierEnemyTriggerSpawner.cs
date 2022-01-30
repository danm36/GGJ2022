using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    [RequireComponent(typeof(SplineEditor.BezierSpline))]
    public class BezierEnemyTriggerSpawner : MonoBehaviour
    {
        [Serializable]
        public class TriggerPoint
        {
            [field: SerializeField] public EnemySpawnTemplate TemplateToSpawn { get; set; }
            [field: SerializeField] public float TimeToSpawn { get; set; }
        }

        private class TriggerPointRuntimeEntry
        {
            public TriggerPoint triggerPoint;
            public EnemySpawnTemplate runtimeTemplate;
            public bool hasTriggered;
        }


        [field: SerializeField] public List<TriggerPoint> EnemyGroupSpawns { get; private set; } = new List<TriggerPoint>();
        private List<TriggerPointRuntimeEntry> runtimeSpawns = new List<TriggerPointRuntimeEntry>();

        private void Awake()
        {
            foreach(var triggerPoint in EnemyGroupSpawns)
            {
                var runtimeTemplate = Instantiate(triggerPoint.TemplateToSpawn);
                runtimeSpawns.Add(new TriggerPointRuntimeEntry
                {
                    triggerPoint = triggerPoint,
                    runtimeTemplate = runtimeTemplate,
                    hasTriggered = false,
                });
            }
        }

        public void UpdatePlayerProgression(float progression)
        {
            foreach(var spawn in runtimeSpawns)
            {
                if(!spawn.hasTriggered && spawn.triggerPoint.TimeToSpawn < progression)
                {
                    spawn.runtimeTemplate.Spawn();
                    spawn.hasTriggered = true;
                    continue;
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var bezierSpline = GetComponent<SplineEditor.BezierSpline>();
            float splineLength = bezierSpline.GetLinearLength();

            Gizmos.color = Color.yellow;
            foreach(var spawn in EnemyGroupSpawns)
            {
                var point = bezierSpline.GetPoint(spawn.TimeToSpawn / splineLength);
                Gizmos.DrawSphere(point, 8.0f);
            }
        }
#endif
    }
}
