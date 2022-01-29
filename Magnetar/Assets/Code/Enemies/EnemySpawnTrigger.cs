using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    [RequireComponent(typeof(BoxCollider))]
    public class EnemySpawnTrigger : MonoBehaviour
    {
        [field: SerializeField] public List<AbstractEnemy> EnemiesToSpawn = new List<AbstractEnemy>();
        [field: SerializeField] public List<EnemySpawnTemplate> EnemyTemplatesToSpawn = new List<EnemySpawnTemplate>();

        public bool HasTriggered { get; private set; }

        private BoxCollider trigger;


        // Start is called before the first frame update
        void OnEnable()
        {
            trigger = GetComponent<BoxCollider>();
            trigger.isTrigger = true;

            foreach (var enemy in EnemiesToSpawn)
            {
                if (!enemy.StartsAwake)
                {
                    enemy.gameObject.SetActive(false);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(HasTriggered)
            {
                return;
            }

            if (other.GetComponent<PlayerController>() != null)
            {
                HasTriggered = true;

                foreach (var enemy in EnemiesToSpawn)
                {
                    enemy.Spawn();
                }

                foreach(var template in EnemyTemplatesToSpawn)
                {
                    template.Spawn();
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var trigger = GetComponent<BoxCollider>();
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(trigger.center, trigger.size);

            foreach (var enemy in EnemiesToSpawn)
            {
                Gizmos.DrawLine(transform.position, enemy.transform.position);
            }

            foreach (var template in EnemyTemplatesToSpawn)
            {
                Gizmos.DrawLine(transform.position, template.transform.position);
            }
        }

        private void OnDrawGizmosSelected()
        {
            var trigger = GetComponent<BoxCollider>();
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(trigger.center, trigger.size);
            Gizmos.color = Color.red * 0.5f;
            Gizmos.DrawCube(trigger.center, trigger.size);

            foreach (var enemy in EnemiesToSpawn)
            {
                Gizmos.DrawLine(transform.position, enemy.transform.position);
            }

            foreach (var template in EnemyTemplatesToSpawn)
            {
                Gizmos.DrawLine(transform.position, template.transform.position);
            }
        }
#endif
    }
}
