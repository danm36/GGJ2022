using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class EnemySpawnTemplate : MonoBehaviour
    {
        [field: SerializeField] public List<AbstractEnemy> EnemiesToSpawn = new List<AbstractEnemy>();

        private void OnEnable()
        {
            foreach(var enemy in GetComponentsInChildren<AbstractEnemy>())
            {
                EnemiesToSpawn.Add(enemy);
            }

            foreach (var enemy in EnemiesToSpawn)
            {
                if (!enemy.StartsAwake)
                {
                    enemy.gameObject.SetActive(false);
                }
            }
        }

        public void Spawn()
        {
            Debug.Log("SPAWNING");

            foreach (var enemy in EnemiesToSpawn)
            {
                enemy.Spawn();
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(PlayableZoneController.DEFAULT_PLAYER_BOUNDS.x * 2.0f, 8.0f, PlayableZoneController.DEFAULT_PLAYER_BOUNDS.y * 2.0f));
            Gizmos.matrix *= Matrix4x4.Rotate(Quaternion.Euler(0.0f, 180.0f, 0.0f));
            Gizmos.DrawFrustum(Vector3.zero, 15.0f, PlayableZoneController.DEFAULT_PLAYER_BOUNDS.y * 0.3f, 0.0f, 1.0f);
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 size = new Vector3(PlayableZoneController.DEFAULT_PLAYER_BOUNDS.x * 2.0f, 8.0f, PlayableZoneController.DEFAULT_PLAYER_BOUNDS.y * 2.0f);
            Gizmos.color = Color.yellow;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, size);
            Gizmos.color = Color.yellow * 0.5f;
            Gizmos.DrawCube(Vector3.zero, size);
        }
#endif

    }
}
