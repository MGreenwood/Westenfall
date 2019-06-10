using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Projectile")]
public class Projectile : Ability
{
    [SerializeField] GameObject prefab;
    [SerializeField] Effect.EffectType effectType;
    [SerializeField] float speed;

    public override bool Cast()
    {
        // create and set required variables
        GameObject projectile = Instantiate(prefab, owner.transform.position, Quaternion.identity);
        Attributes.StatTypes bonusDamage;
        if (_abilityType == Ability.AbilityType.Magic)
            bonusDamage = Attributes.StatTypes.Magic;
        else
            bonusDamage = Attributes.StatTypes.Strength;

        projectile.GetComponent<ProjectileBehavior>().SetVars(
            damage + owner.GetComponent<IHasAttributes>().GetAttributes().GetStat(bonusDamage).value, // bonus damage from stats
            range, Effect.EffectType.Basic, owner);

        // add projectile velocity towards mouse
        /*
        Vector3 mousePos = MouseManager.instance.GetMousePosition() + new Vector3(0, owner.transform.position.y, 0);
        projectile.GetComponent<Rigidbody>().velocity = (mousePos - owner.transform.position).normalized * speed;*/

        projectile.GetComponent<Rigidbody>().velocity = (owner.transform.forward).normalized * speed;

        return true;
    }
}
