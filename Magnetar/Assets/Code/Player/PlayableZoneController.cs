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
        public bool IsGameOver { get; private set; }
        [field: SerializeField] public bool IsTwoShipMode { get; set; }

        public Vector3 Velocity { get; private set; }

        public Vector2 PlayerBounds { get; private set; } = new Vector2(DEFAULT_PLAYER_BOUNDS.x, DEFAULT_PLAYER_BOUNDS.y);

        [field: SerializeField] public BezierSpline TargetSpline { get; private set; }
        private SplinePath splinePath;
        private float splineProgress = 0.0f;
        [field: SerializeField] public float SplineProgressionSpeed { get; private set; } = 32.0f;

        [field: SerializeField] public PlayerHUD PlayerHUDPrefab { get; private set; }
        public PlayerHUD PlayerHUD { get; private set; }

        [field: SerializeField] public PlayerController SinglePlayerShip { get; private set; }
        [field: SerializeField] public PlayerController Player1Ship { get; private set; }
        [field: SerializeField] public PlayerController Player2Ship { get; private set; }

        [field: SerializeField] public CinemachineVirtualCamera IntroCamera { get; private set; }
        [field: SerializeField] public CinemachineVirtualCamera IntroCameraTwoPlayer { get; private set; }
        [field: SerializeField] public float IntroCameraHoldTime { get; private set; } = 2.5f;
        [field: SerializeField] public CinemachineVirtualCamera DefaultCamera { get; private set; }
        private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

        public List<PlayerController> Players { get; private set; } = new List<PlayerController>();
        public BoxCollider EnemySpawnTrigger { get; private set; }

        private BezierEnemyTriggerSpawner bezierEnemySpawnTrigger;

        // Start is called before the first frame update
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }
            Instance = this;

            EnemySpawnTrigger = GetComponent<BoxCollider>();
            EnemySpawnTrigger.isTrigger = true;
            EnemySpawnTrigger.size = new Vector3(PlayerBounds.x * 2, 8.0f, PlayerBounds.y * 2.0f);

            SinglePlayerShip.gameObject.SetActive(false);
            Player1Ship.gameObject.SetActive(false);
            Player2Ship.gameObject.SetActive(false);

            foreach (var cam in GetComponentsInChildren<CinemachineVirtualCamera>())
            {
                cameras.Add(cam);
                cam.enabled = false;
            }
            IntroCamera.enabled = true;
        }

        public void Initialize()
        {
            if(TargetSpline == null)
            {
                Debug.LogError("Could not start game. Target Spline was null!");
                return;
            }

            PlayerHUD = Instantiate(PlayerHUDPrefab);

            if (IsTwoShipMode)
            {
                SinglePlayerShip.gameObject.SetActive(false);
                Player1Ship.gameObject.SetActive(true);
                Player2Ship.gameObject.SetActive(true);

                Players.Add(Player1Ship);
                Players.Add(Player2Ship);

                PlayerHUD.Initialize(Player1Ship, Player2Ship);

                IntroCameraTwoPlayer.enabled = true;
                IntroCamera.enabled = false;
            }
            else
            {
                SinglePlayerShip.gameObject.SetActive(true);
                Player1Ship.gameObject.SetActive(false);
                Player2Ship.gameObject.SetActive(false);

                Players.Add(SinglePlayerShip);

                PlayerHUD.Initialize(SinglePlayerShip);
            }

            splinePath = new SplinePath();
            TargetSpline.GetEvenlySpacedPoints(1.0f, splinePath);
            bezierEnemySpawnTrigger = TargetSpline.GetComponent<BezierEnemyTriggerSpawner>();

            StartCoroutine(Initialize_Intro());

            IsPlaying = true;
        }

        private IEnumerator Initialize_Intro()
        {
            yield return new WaitForSeconds(IntroCameraHoldTime);
            IntroCamera.enabled = false;
            IntroCameraTwoPlayer.enabled = false;
            DefaultCamera.enabled = true;

            yield return new WaitForSeconds(0.5f);
            PlayerHasControl = true;
            PlayerHUD.BeginGameplay();
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
                if(point2Idx == point1Idx)
                {
                    point2Idx = (point2Idx + 1) % splinePath.Points.Count;
                }
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
            if(forward.sqrMagnitude < 0.1f)
            {
                forward = transform.forward;
            }
            float lerpProgress = splineProgress - point1Idx;

            transform.position = Vector3.Lerp(splinePath.Points[point1Idx], splinePath.Points[point2Idx], lerpProgress);
            transform.rotation = Quaternion.Slerp(
                Quaternion.LookRotation(forward, splinePath.Normals[point1Idx]),
                Quaternion.LookRotation(forward, splinePath.Normals[point2Idx]),
                lerpProgress);

            if(IsGameOver)
            {
                SplineProgressionSpeed = Mathf.Max(0.0f, SplineProgressionSpeed - Time.deltaTime * 8.0f);
            }

            splineProgress += Time.deltaTime * SplineProgressionSpeed;
            Velocity = (transform.position - oldPosition) / Time.deltaTime; 

            if(bezierEnemySpawnTrigger != null)
            {
                bezierEnemySpawnTrigger.UpdatePlayerProgression(splineProgress);
            }

            foreach(var trigger in SplineAttachedEnemySpawnTrigger.ArmedTriggers)
            {
                if(trigger.SplineToAttachTo == TargetSpline && trigger.SplineTriggerPoint <= splineProgress)
                {
                    trigger.Trigger();
                }
            }
        }

        public void GameOver()
        {
            IsGameOver = true;
            PlayerHasControl = false;
            PlayerHUD.ShowGameOver();
        }

        public void MissionComplete()
        {
            StartCoroutine(DoMissionComplete());
        }

        private IEnumerator DoMissionComplete()
        {
            PlayerHasControl = false;
            foreach (var cam in cameras)
            {
                cam.enabled = false;
            }

            if (IsTwoShipMode)
            {
                IntroCameraTwoPlayer.enabled = true;
            }
            else
            {
                IntroCamera.enabled = true;
            }

            yield return new WaitForSeconds(2.0f);

            PlayerHUD.ShowMissionComplete();
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
