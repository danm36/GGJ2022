using Cinemachine;
using SplineEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class PlayableZoneController : MonoBehaviour
    {
        public bool IsPlaying { get; private set; }
        public bool PlayerHasControl { get; private set; }
        public Vector3 Velocity { get; private set; }

        public BezierSpline targetSpline;
        private SplinePath splinePath;
        private float splineProgress = 0.0f;
        public float splineProgressionSpeed = 32.0f;

        public CinemachineVirtualCamera introCamera;
        public float introCameraHoldTime = 2.5f;
        public CinemachineVirtualCamera defaultCamera;
        private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

        private PlayerController player;


        // Start is called before the first frame update
        void Awake()
        {
            player = GetComponentInChildren<PlayerController>();

            foreach(var cam in GetComponentsInChildren<CinemachineVirtualCamera>())
            {
                cameras.Add(cam);
                cam.enabled = false;
            }
            introCamera.enabled = true;
        }

        void Start()
        {
            if (targetSpline != null)
            {
                Initialize();
            }
        }

        public void Initialize()
        {
            splinePath = new SplinePath();
            targetSpline.GetEvenlySpacedPoints(1.0f, splinePath);
            player.Initialize(this);

            StartCoroutine(Initialize_Intro());

            IsPlaying = true;
        }

        private IEnumerator Initialize_Intro()
        {
            yield return new WaitForSeconds(introCameraHoldTime);
            introCamera.enabled = false;
            defaultCamera.enabled = true;

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

            if (targetSpline.IsLoop)
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

            splineProgress += Time.deltaTime * splineProgressionSpeed;
            Velocity = (transform.position - oldPosition) / Time.deltaTime; 
        }
    }
}
