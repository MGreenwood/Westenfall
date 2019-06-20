using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Projectiles/Projectile")]
public class Projectile : Ability
{
    [SerializeField] GameObject prefab;
    [SerializeField] Effect.AbilityEffect effect;
    [SerializeField] float speed;

    public override bool Cast()
    {
        // create and set required variables
        GameObject projectile = Instantiate(prefab, owner.transform.position, Quaternion.identity);
        projectile.transform.forward = owner.transform.forward;
        
        Attributes.StatTypes bonusDamage;
        if (_abilityType == Ability.AbilityType.Magic)
            bonusDamage = Attributes.StatTypes.Magic;
        else
            bonusDamage = Attributes.StatTypes.Strength;

        projectile.GetComponent<ProjectileBehavior>().SetVars(
            damage, // bonus damage from stats
            range, effect, owner, _abilityType);

        // add projectile velocity towards mouse
        /*
        Vector3 mousePos = MouseManager.instance.GetMousePosition() + new Vector3(0, owner.transform.position.y, 0);
        projectile.GetComponent<Rigidbody>().velocity = (mousePos - owner.transform.position).normalized * speed;*/

        projectile.GetComponent<Rigidbody>().velocity = projectile.transform.forward.normalized * speed;

        return true;
    }
}
