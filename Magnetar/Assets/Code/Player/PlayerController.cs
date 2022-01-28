using SplineEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Magnetar
{
    public class PlayerController : MonoBehaviour
    {
        private class WeaponTrackingEntry
        {
            public EquipableWeapon weapon;
            public float cooldown;
        }

        const float MAX_Z_TILT = 15.0f;
        readonly Vector2 PLAYER_BOUNDS = new Vector2(100.0f, 60.0f);
        const float BOUNDS_RECOVERY_RATE = 30.0f;

        public PlayableZoneController Parent { get; private set; }

        public Transform modelTransform;
        public float maxShipSpeed = 60.0f;
        public float boostMultiplier = 2.5f;

        Vector2 moveVec = Vector2.zero;
        Vector3 velocity = Vector3.zero;
        private bool isBoosting = false;

        public List<EquipableWeapon> equipableWeapons = new List<EquipableWeapon>();
        private List<WeaponTrackingEntry> weapons = new List<WeaponTrackingEntry>();


        private bool isShootingNormal = false;
        private bool isShootingPositive = false;
        private bool isShootingNegative = false;

        void Start()
        {
        }

        public void Initialize(PlayableZoneController parent)
        {
            Parent = parent;

            weapons.Clear();
            foreach (EquipableWeapon weapon in equipableWeapons)
            {
                weapons.Add(new WeaponTrackingEntry()
                {
                    weapon = weapon
                });
            }
        }

        void Update()
        {
            float boostedSpeed = maxShipSpeed * boostMultiplier;
            Vector3 targetVelocity = new Vector3(moveVec.x, 0, moveVec.y) * (isBoosting ? boostedSpeed : maxShipSpeed);

            // Constrain to bounds by overriding specific player controls
            float recoveryAmount = Time.deltaTime * BOUNDS_RECOVERY_RATE * boostedSpeed;
            if (transform.localPosition.x < -PLAYER_BOUNDS.x)
            {
                targetVelocity.x = Mathf.Min(velocity.x + recoveryAmount, boostedSpeed);
            }
            else if (transform.localPosition.x > PLAYER_BOUNDS.x)
            {
                targetVelocity.x = Mathf.Max(velocity.x - recoveryAmount, -boostedSpeed);
            }
            else if (transform.localPosition.z < -PLAYER_BOUNDS.y)
            {
                targetVelocity.z = Mathf.Min(velocity.z + recoveryAmount, boostedSpeed);
            }
            else if (transform.localPosition.z > PLAYER_BOUNDS.y)
            {
                targetVelocity.z = Mathf.Max(velocity.z - recoveryAmount, -boostedSpeed);
            }

            velocity = Vector3.Lerp(velocity, targetVelocity, 0.1f);

            Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Clamp(-velocity.x / maxShipSpeed * MAX_Z_TILT, -MAX_Z_TILT, MAX_Z_TILT));

            transform.Translate(velocity * Time.deltaTime);
            modelTransform.localRotation = targetRotation;

            // Handle shooting
            foreach(WeaponTrackingEntry w in weapons)
            {
                w.cooldown -= Time.deltaTime;

                bool doShoot;
                if(w.weapon.slot == EWeaponSlot.Magnetic_Positive)
                {
                    doShoot = isShootingPositive;
                }
                else if (w.weapon.slot == EWeaponSlot.Magnetic_Negative)
                {
                    doShoot = isShootingNegative;
                }
                else
                {
                    doShoot = isShootingNormal;
                }

                if(doShoot && w.cooldown <= 0.0f)
                {
                    w.weapon.SpawnBullets(true, transform.position, transform.rotation, velocity + Parent.Velocity);
                    w.cooldown = w.weapon.shotCooldown;
                }
            }
        }

        public void OnMove(InputValue value)
        {
            moveVec = value.Get<Vector2>();
        }

        public void OnBoost(InputValue value)
        {
            isBoosting = value.isPressed;
        }

        public void OnShootNormal(InputValue value)
        {
            isShootingNormal = value.isPressed;
        }

        public void OnShootPositive(InputValue value)
        {
            isShootingPositive = value.isPressed;
        }

        public void OnShootNegative(InputValue value)
        {
            isShootingNegative = value.isPressed;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(PLAYER_BOUNDS.x * 2, 1.0f, PLAYER_BOUNDS.y * 2.0f));
        }
#endif
    }
}
