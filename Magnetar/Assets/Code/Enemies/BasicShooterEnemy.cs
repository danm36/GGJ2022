using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Magnetar
{
    public class BasicShooterEnemy : AbstractEnemy
    {
        [field: SerializeField] public bool TrackPlayer { get; private set; }

        protected override void OnUpdate()
        {
            if(TrackPlayer)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(PlayableZoneController.Instance.Player.transform.position - transform.position, transform.up),
                    0.1f);
            }

            if (IsActiveInArena || CanAttackEvenWhenNotInArena)
            {
                foreach (RuntimeWeaponTrackingEntry w in weapons)
                {
                    w.Update();
                    w.TryShoot(ExistsInBackground ? null : PlayableZoneController.Instance.transform, transform.position, transform.rotation);
                }
            }
        }
    }
}
