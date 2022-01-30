using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class BasicShooterEnemy : AbstractEnemy
    {
        [field: SerializeField] public bool TrackPlayer { get; private set; }
        private PlayerController trackingPlayer;

        protected override void OnEnable()
        {
            base.OnEnable();
            trackingPlayer = PlayableZoneController.Instance.Players.Random();
        }

        protected override void OnUpdate()
        {
            if(TrackPlayer)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(trackingPlayer.transform.position - transform.position, transform.up),
                    0.1f);
            }

            if (IsActiveInArena || CanAttackEvenWhenNotInArena)
            {
                foreach (RuntimeWeaponTrackingEntry w in weapons)
                {
                    w.Update();
                    if(w.TryShoot(ExistsInBackground ? null : PlayableZoneController.Instance.transform, transform.position, transform.rotation))
                    {
                        AudioSourceComponent.PlayOneShot(ShootSoundEffect);
                    }
                }
            }
        }
    }
}
