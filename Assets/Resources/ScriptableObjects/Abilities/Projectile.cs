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
        projectile.GetComponent<ProjectileBehavior>().SetVars(damage, range, Effect.EffectType.Basic);

        // add projectile velocity towards mouse
        /*
        Vector3 mousePos = MouseManager.instance.GetMousePosition() + new Vector3(0, owner.transform.position.y, 0);
        projectile.GetComponent<Rigidbody>().velocity = (mousePos - owner.transform.position).normalized * speed;*/

        projectile.GetComponent<Rigidbody>().velocity = (owner.transform.forward).normalized * speed;

        return true;
    }
}
