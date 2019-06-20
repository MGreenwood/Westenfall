using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Projectiles/WeaponProjectile")]
public class WeaponProjectile : Projectile
{
    [SerializeField]
    Weapon.WeaponType _weaponType;
}
