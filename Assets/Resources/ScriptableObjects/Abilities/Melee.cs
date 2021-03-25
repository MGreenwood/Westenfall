using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Melee/Melee")]
public class Melee : Ability
{
    int precision = 10; // how many raycasts to shoot, even numbers work best

    [SerializeField]
    float _angle; // the attack angle, best if set in multiples of 10
    [SerializeField]
    Effect.AbilityEffect _effect;
    [SerializeField]
    GameObject _particleSystem;
    [SerializeField]
    LayerMask hitLayer;
    
    public override bool Cast()
    {
        precision = (int)_angle / 5;

        if(_particleSystem != null)
            Instantiate(_particleSystem, owner.transform.position, owner.transform.rotation);

        int target = owner.layer == LayerMask.NameToLayer("Player") ? LayerMask.NameToLayer("Enemy") : LayerMask.NameToLayer("Player");

        List<GameObject> hits = new List<GameObject>();

        for (int i = -precision / 2; i < precision / 2; i++)
        {
            float angle = i * (_angle * Mathf.Deg2Rad) / precision;
            Vector3 direction = owner.transform.TransformDirection(new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)));

            if(GameManager.instance.Debug) // draw the raycasts
                Debug.DrawLine(owner.transform.position, owner.transform.position + direction.normalized * range, Color.red, 1f);

            RaycastHit[] rayHits = Physics.RaycastAll(owner.transform.position, direction.normalized, range, hitLayer);
            foreach (RaycastHit hit in rayHits)
            {
                if (!hits.Contains(hit.collider.gameObject))
                {
                    hits.Add(hit.collider.gameObject);
                }
            }
        }

        if (hits.Count > 0)
        {
            foreach (GameObject ob in hits)
            {
                if (ob.layer == target)
                {
                    bool isCrit = false; // TODO

                    Attributes.StatTypes bonusDamage;
                    if (_abilityType == Ability.AbilityType.Magic)
                        bonusDamage = Attributes.StatTypes.Magic;
                    else
                        bonusDamage = Attributes.StatTypes.Strength;
                    try
                    {
                        ob.GetComponent<IDamageable>().Damage(damage +
                            (int)((float)owner.GetComponent<IHasAttributes>().GetAttributes().GetStat(bonusDamage).value * Attributes.Bonus_Mod),
                            _effect, isCrit, owner);
                    }
                    catch
                    {
                        ob.GetComponent<IDamageable>().Damage(damage,
                            _effect, isCrit, owner);
                    }

                    if(_causesKnockback)
                        ob.GetComponent<IDamageable>().Knockback(owner.transform.position, _knockbackPower);
                }
            }
        }

        return true;
    }
}
