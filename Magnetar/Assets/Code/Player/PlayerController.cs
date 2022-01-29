using SplineEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Magnetar
{
    public class PlayerController : MonoBehaviour
    {
        const float MAX_Z_TILT = 15.0f;
        const float BOUNDS_RECOVERY_RATE = 30.0f;

        public PlayableZoneController Parent { get; private set; }

        [field: SerializeField] public int PlayerID { get; private set; } = 1;
        [field: SerializeField] public Transform ModelTransform { get; private set; }
        [field: SerializeField] public OneShotEffect HitEffect { get; private set; }
        [field: SerializeField] public OneShotEffect DeathEffect { get; private set; }

        [field: SerializeField] public float MaxShipSpeed { get; private set; } = 60.0f;
        [field: SerializeField] public float BoostMultiplier { get; private set; } = 2.5f;

        Vector2 moveVec = Vector2.zero;
        Vector3 velocity = Vector3.zero;
        private bool isBoosting = false;

        private HealthComponent healthComponent;

        [field: SerializeField] public List<EquipableWeapon> EquippedWeapons { get; private set; } = new List<EquipableWeapon>();
        private List<RuntimeWeaponTrackingEntry> weapons = new List<RuntimeWeaponTrackingEntry>();

        private bool isShootingNormal = false;
        private bool isShootingPositive = false;
        private bool isShootingNegative = false;

        void OnEnable()
        {
            Parent = GetComponentInParent<PlayableZoneController>();

            healthComponent = GetComponent<HealthComponent>();
            if(healthComponent == null)
            {
                throw new NullReferenceException("No health component has been added to the player!");
            }

            healthComponent.OnHurt += OnHurt;
            healthComponent.OnDeath += OnDeath;

            weapons.Clear();
            foreach (EquipableWeapon weapon in EquippedWeapons)
            {
                if (weapon != null)
                {
                    weapons.Add(new RuntimeWeaponTrackingEntry(weapon));
                }
            }
        }

        private void OnDisable()
        {
            if(healthComponent != null)
            {
                healthComponent.OnHurt -= OnHurt;
                healthComponent.OnDeath -= OnDeath;
            }
        }

        void Update()
        {
            // Handle shooting
            foreach(RuntimeWeaponTrackingEntry w in weapons)
            {
                w.Update();

                bool doShoot;
                if(w.Weapon.slot == EWeaponSlot.Magnetic_Positive)
                {
                    doShoot = isShootingPositive;
                }
                else if (w.Weapon.slot == EWeaponSlot.Magnetic_Negative)
                {
                    doShoot = isShootingNegative;
                }
                else
                {
                    doShoot = isShootingNormal;
                }

                if(doShoot)
                {
                    w.TryShoot(Parent.transform, transform.position, transform.rotation, PlayerID);
                }
            }
        }

        private void FixedUpdate()
        {
            float boostedSpeed = MaxShipSpeed * BoostMultiplier;
            Vector3 targetVelocity;
            if(Parent.PlayerHasControl)
            {
                targetVelocity = new Vector3(moveVec.x, 0, moveVec.y) * (isBoosting ? boostedSpeed : MaxShipSpeed);
            }
            else
            {
                targetVelocity = Vector3.zero;
            }

            // Constrain to bounds by overriding specific player controls
            float recoveryAmount = Time.fixedDeltaTime * BOUNDS_RECOVERY_RATE * boostedSpeed;
            if (transform.localPosition.x < -Parent.PlayerBounds.x)
            {
                targetVelocity.x = Mathf.Min(velocity.x + recoveryAmount, boostedSpeed);
            }
            else if (transform.localPosition.x > Parent.PlayerBounds.x)
            {
                targetVelocity.x = Mathf.Max(velocity.x - recoveryAmount, -boostedSpeed);
            }
            else if (transform.localPosition.z < -Parent.PlayerBounds.y)
            {
                targetVelocity.z = Mathf.Min(velocity.z + recoveryAmount, boostedSpeed);
            }
            else if (transform.localPosition.z > Parent.PlayerBounds.y)
            {
                targetVelocity.z = Mathf.Max(velocity.z - recoveryAmount, -boostedSpeed);
            }

            velocity = Vector3.Lerp(velocity, targetVelocity, 0.1f);

            Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Clamp(-velocity.x / MaxShipSpeed * MAX_Z_TILT, -MAX_Z_TILT, MAX_Z_TILT));

            transform.Translate(velocity * Time.fixedDeltaTime);
            ModelTransform.localRotation = targetRotation;
        }


        #region User Inputs

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

        #endregion

        #region Health Events

        private void OnHurt(Collision collision, float hurtAmount, float currentHealth)
        {
            if (HitEffect != null && collision.contactCount > 0)
            {
                var contact = collision.GetContact(0);
                Instantiate(HitEffect, contact.point, Quaternion.LookRotation(contact.normal), transform);
            }

            Debug.Log($"Player was hurt by {hurtAmount} - Current health: {currentHealth}");
        }

        private void OnDeath(Collision collision)
        {
            if (DeathEffect != null)
            {
                Instantiate(DeathEffect, transform.position, transform.rotation, transform.parent);
            }

            Debug.Log($"Player is dead!");
        }

        #endregion
    }
}
