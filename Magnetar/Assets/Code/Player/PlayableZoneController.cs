using Cinemachine;
using SplineEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    [RequireComponent(typeof(BoxCollider))]
    public class PlayableZoneController : MonoBehaviour
    {
        public static readonly Vector2 DEFAULT_PLAYER_BOUNDS = new Vector2(100.0f, 60.0f);

        public static PlayableZoneController Instance { get; private set; }
        public bool IsPlaying { get; private set; }
        public bool PlayerHasControl { get; private set; }
        public Vector3 Velocity { get; private set; }

        public Vector2 PlayerBounds { get; private set; } = new Vector2(DEFAULT_PLAYER_BOUNDS.x, DEFAULT_PLAYER_BOUNDS.y);

        [field: SerializeField] public BezierSpline TargetSpline { get; private set; }
        private SplinePath splinePath;
        private float splineProgress = 0.0f;
        [field: SerializeField] public float SplineProgressionSpeed { get; private set; } = 32.0f;

        [field: SerializeField] public CinemachineVirtualCamera IntroCamera { get; private set; }
        [field: SerializeField] public float IntroCameraHoldTime { get; private set; } = 2.5f;
        [field: SerializeField] public CinemachineVirtualCamera DefaultCamera { get; private set; }
        private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

        public PlayerController Player { get; private set; }
        public BoxCollider EnemySpawnTrigger { get; private set; }


        // Start is called before the first frame update
        void Awake()
        {
            Player = GetComponentInChildren<PlayerController>();
            EnemySpawnTrigger = GetComponent<BoxCollider>();
            EnemySpawnTrigger.isTrigger = true;
            EnemySpawnTrigger.size = new Vector3(PlayerBounds.x * 2, 8.0f, PlayerBounds.y * 2.0f);

            foreach (var cam in GetComponentsInChildren<CinemachineVirtualCamera>())
            {
                cameras.Add(cam);
                cam.enabled = false;
            }
            IntroCamera.enabled = true;
        }

        void Start()
        {
            if (TargetSpline != null)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            if(Instance != null)
            {
                Destroy(Instance);
            }
            Instance = this;

            splinePath = new SplinePath();
            TargetSpline.GetEvenlySpacedPoints(1.0f, splinePath);

            StartCoroutine(Initialize_Intro());

            IsPlaying = true;
        }

        private IEnumerator Initialize_Intro()
        {
            yield return new WaitForSeconds(IntroCameraHoldTime);
            IntroCamera.enabled = false;
            DefaultCamera.enabled = true;

            yield return new WaitForSeconds(0.5f);
            PlayerHasControl = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsPlaying)
            {
                return;
            }

            Vector3 oldPosition = transform.position;

            int point1Idx;
            int point2Idx;

            if (TargetSpline.IsLoop)
            {
                point1Idx = Mathf.FloorToInt(splineProgress) % splinePath.Points.Count;
                point2Idx = Mathf.CeilToInt(splineProgress) % splinePath.Points.Count;
            }
            else
            {
                point1Idx = Mathf.FloorToInt(splineProgress);
                point2Idx = Mathf.CeilToInt(splineProgress);

                if (point2Idx >= splinePath.Points.Count)
                {
                    Velocity = Vector3.zero;
                    Debug.LogWarning("Reached end of path!");
                    return;
                }
            }

            Vector3 point1 = splinePath.Points[point1Idx];
            Vector3 point2 = splinePath.Points[point2Idx];
            Vector3 forward = (point2 - point1).normalized;
            float lerpProgress = splineProgress - point1Idx;

            transform.position = Vector3.Lerp(splinePath.Points[point1Idx], splinePath.Points[point2Idx], lerpProgress);
            transform.rotation = Quaternion.Slerp(
                Quaternion.LookRotation(forward, splinePath.Normals[point1Idx]),
                Quaternion.LookRotation(forward, splinePath.Normals[point2Idx]),
                lerpProgress);

            splineProgress += Time.deltaTime * SplineProgressionSpeed;
            Velocity = (transform.position - oldPosition) / Time.deltaTime; 

            foreach(var trigger in SplineAttachedEnemySpawnTrigger.ArmedTriggers)
            {
                if(trigger.SplineToAttachTo == TargetSpline && trigger.SplineTriggerPoint <= splineProgress)
                {
                    trigger.Trigger();
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(PlayerBounds.x * 2, 1.0f, PlayerBounds.y * 2.0f));
        }
#endif
    }
}
