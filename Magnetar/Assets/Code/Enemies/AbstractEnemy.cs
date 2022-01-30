using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public enum EEnemyFlyTransitionDirection
    {
        None,
        Front,
        Behind,
        Above,
        Below,
        Left,
        Right,
    }

    public enum EMotionMode
    {
        Static,
        MoveDownOverTime
    }

    [RequireComponent(typeof(Collider), typeof(HealthComponent))]
    public abstract class AbstractEnemy : MonoBehaviour
    {
        public bool IsActiveInArena { get; private set; }

        public HealthComponent Health { get; private set; }
        [field: SerializeField] public bool ExistsInBackground { get; private set; }
        [field: SerializeField] public bool StartsAwake { get; private set; }
        [field: SerializeField] public bool CanAttackEvenWhenNotInArena { get; private set; }
        [field: SerializeField] public EEnemyFlyTransitionDirection FlyInDirection { get; private set; } = EEnemyFlyTransitionDirection.None;
        [field: SerializeField] public float FlyInDelay { get; private set; } = 0.0f;
        [field: SerializeField] public EEnemyFlyTransitionDirection FlyOutDirection { get; private set; } = EEnemyFlyTransitionDirection.None;
        [field: SerializeField] public float FlyOutDelay { get; private set; } = 0.0f;
        [field: SerializeField] public EMotionMode MotionMode { get; set; } = EMotionMode.Static;
        [field: SerializeField] public float MotionModeSpeed { get; set; } = 16.0f;
        [field: SerializeField] public OneShotEffect HitEffect { get; private set; }
        [field: SerializeField] public OneShotEffect DeathEffect { get; private set; }

        [field: SerializeField] public List<EquipableWeapon> EquippedWeapons { get; private set; } = new List<EquipableWeapon>();
        protected readonly List<RuntimeWeaponTrackingEntry> weapons = new List<RuntimeWeaponTrackingEntry>();
        [field: SerializeField] public AudioClip ShootSoundEffect { get; private set; }

        public AudioSource AudioSourceComponent { get; private set; }

        private float elapsed;
        private Collider myCollider;
        private float motionElapsedDistance = 0.0f;

        protected virtual void OnEnable()
        {
            Health = GetComponent<HealthComponent>();
            if (Health == null)
            {
                Debug.LogError("Enemy lacks a health component!", gameObject);
                return;
            }

            Health.OnHurt += OnHurt;
            Health.OnDeath += OnDeath;

            myCollider = GetComponent<Collider>();

            weapons.Clear();
            foreach (EquipableWeapon weapon in EquippedWeapons)
            {
                if (weapon != null)
                {
                    weapons.Add(new RuntimeWeaponTrackingEntry(weapon));
                }
            }

            AudioSourceComponent = GetComponent<AudioSource>();
        }

        protected virtual void OnDisable()
        {
            if (Health != null)
            {
                Health.OnHurt -= OnHurt;
                Health.OnDeath -= OnDeath;
            }
        }

        public void Spawn(bool isAlreadyInWorldSpace= false)
        {
            gameObject.SetActive(true);

            if (!ExistsInBackground)
            {
                transform.SetParent(PlayableZoneController.Instance.transform, isAlreadyInWorldSpace);
                transform.localEulerAngles = new Vector3(0.0f, transform.localEulerAngles.y, 0.0f);
            }

            if (FlyInDirection != EEnemyFlyTransitionDirection.None)
            {
                StartCoroutine(PerformFlyInTransition());
            }
            else
            {
                IsActiveInArena = true;
            }
        }

        private void Update()
        {
            if (IsActiveInArena)
            {
                elapsed += Time.deltaTime;

                if (FlyOutDirection != EEnemyFlyTransitionDirection.None && elapsed > FlyOutDelay)
                {
                    IsActiveInArena = false;
                    StartCoroutine(PerformFlyOutTransition());
                }
            }

            if(MotionMode == EMotionMode.MoveDownOverTime)
            {
                motionElapsedDistance += Time.deltaTime * MotionModeSpeed;
                transform.Translate(new Vector3(0.0f, 0.0f, Time.deltaTime * MotionModeSpeed), Space.Self);
                if (motionElapsedDistance > PlayableZoneController.DEFAULT_PLAYER_BOUNDS.y * 2.0f)
                {
                    Destroy(gameObject);
                }
            }

            OnUpdate();
        }

        protected virtual void OnUpdate() { }

        private Vector3 GetFlyInOutTarget(EEnemyFlyTransitionDirection dir, Vector3 inBoundsPosition)
        {
            Vector3 target = inBoundsPosition;

            switch (dir)
            {
                default:
                case EEnemyFlyTransitionDirection.Front:
                    target.z += PlayableZoneController.DEFAULT_PLAYER_BOUNDS.y * 2.0f;
                    break;
                case EEnemyFlyTransitionDirection.Behind:
                    target.z -= PlayableZoneController.DEFAULT_PLAYER_BOUNDS.y * 2.0f;
                    break;
                case EEnemyFlyTransitionDirection.Left:
                    target.x -= PlayableZoneController.DEFAULT_PLAYER_BOUNDS.x * 2.0f;
                    break;
                case EEnemyFlyTransitionDirection.Right:
                    target.x += PlayableZoneController.DEFAULT_PLAYER_BOUNDS.x * 2.0f;
                    break;
                case EEnemyFlyTransitionDirection.Above:
                    target.z += PlayableZoneController.DEFAULT_PLAYER_BOUNDS.y * 2.0f;
                    target.y += PlayableZoneController.DEFAULT_PLAYER_BOUNDS.y * 2.0f;
                    break;
                case EEnemyFlyTransitionDirection.Below:
                    target.z -= PlayableZoneController.DEFAULT_PLAYER_BOUNDS.y * 3.0f;
                    target.y -= PlayableZoneController.DEFAULT_PLAYER_BOUNDS.y * 2.0f;
                    break;
            }

            return target;
        }

        private IEnumerator PerformFlyInTransition()
        {
            Vector3 startPosition = GetFlyInOutTarget(FlyInDirection, transform.localPosition);
            Vector3 targetPosition = transform.localPosition;

            transform.localPosition = startPosition;

            if (FlyInDelay > 0.0f)
            {
                myCollider.enabled = false;
                yield return new WaitForSeconds(FlyInDelay);
                myCollider.enabled = true;
            }

            for (float progress = 0.0f; progress < 1.0f; progress += Time.deltaTime)
            {
                transform.localPosition = Vector3.Lerp(startPosition, targetPosition, Mathf.Pow(progress, 0.2f));
                yield return null;
            }

            transform.localPosition = targetPosition;
            IsActiveInArena = true;
        }

        private IEnumerator PerformFlyOutTransition()
        {
            Vector3 startPosition = transform.localPosition;
            Vector3 targetPosition = GetFlyInOutTarget(FlyOutDirection, transform.localPosition);

            transform.localPosition = startPosition;


            for (float progress = 0.0f; progress < 1.0f; progress += Time.deltaTime)
            {
                transform.localPosition = Vector3.Lerp(startPosition, targetPosition, progress * progress);
                yield return null;
            }

            Destroy(gameObject);
        }

        protected virtual void OnHurt(Collision collision, float hurtAmount, float currentHealth)
        {
            if (HitEffect != null && collision.contactCount > 0)
            {
                var contact = collision.GetContact(0);
                Instantiate(HitEffect, contact.point, Quaternion.LookRotation(contact.normal), transform);
            }
        }

        protected virtual void OnDeath(Collision collision)
        {
            if (DeathEffect != null)
            {
                Instantiate(DeathEffect, transform.position, transform.rotation, transform.parent);
            }

            Destroy(gameObject);
        }
    }
}
