using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    float damage;
    float maxRange;
    Effect.AbilityEffect effect;
    Vector3 startPosition;
    GameObject _owner;
    Ability.AbilityType abilityType;


    float rotationSpeed = 20f;
    bool dead = false;

    public void SetVars(int damage_, float range, Effect.AbilityEffect effect_, GameObject owner, Ability.AbilityType aType)
    {
        damage = damage_;
        maxRange = range;
        effect = effect_;
        _owner = owner;
        abilityType = aType;

        startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (dead)
            return;

        if(Vector3.Distance(transform.position, startPosition) > maxRange)
        {
            DestroyOb();
        }
        //transform.Rotate(transform.forward, rotationSpeed);
        transform.position = new Vector3(transform.position.x, startPosition.y, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // determine if it was a crit , Use enemy resistance as well as player stats or vice versa TODO
        bool crit = false;

        if (gameObject.layer == LayerMask.NameToLayer("PlayerProjectile"))
        {
            float bonusDamage = 0;

            if (other.tag == "Enemy")
            {
                switch(abilityType)
                {
                    case Ability.AbilityType.Magic:
                        bonusDamage = (_owner.GetComponent<Player>().GetAttributes().GetStat(Attributes.StatTypes.Magic).value * Attributes.Bonus_Mod);
                        break;
                    case Ability.AbilityType.Melee:
                        bonusDamage = (_owner.GetComponent<Player>().GetAttributes().GetStat(Attributes.StatTypes.Strength).value * Attributes.Bonus_Mod);
                        break;
                    case Ability.AbilityType.Ranged:
                        bonusDamage = (_owner.GetComponent<Player>().GetAttributes().GetStat(Attributes.StatTypes.Dexterity).value * Attributes.Bonus_Mod);
                        break;
                }


                float critRate = 0f;
                float weaponDamage = 0f;

                Weapon playerWeapon = _owner.GetComponent<Equipment>().GetItem(Item.EquipmentSlot.Weapon) as Weapon;
                if(playerWeapon != null)
                {
                    if(playerWeapon.GetMagicDamage() > 0)
                    {
                        weaponDamage = Random.Range(playerWeapon.GetMagicDamage() * 0.8f, playerWeapon.GetMagicDamage() * 1.2f);
                    }
                    else
                        weaponDamage = Random.Range(playerWeapon.GetMinDamage(), playerWeapon.GetMaxDamage());

                    Weapon.Stat stat = playerWeapon.GetStats().Find(x => x._stat == Weapon.Stats.CritRate);

                    if (stat == null)
                        critRate = 5;
                    else
                        critRate = stat._value;
                    crit = Random.Range(0, 100f) < critRate;
                }
                other.gameObject.GetComponent<Enemy>().Damage(damage + weaponDamage + bonusDamage, effect, crit, _owner);
            }
        }
        else if (gameObject.layer == LayerMask.NameToLayer("EnemyProjectile"))
        {
            if (other.tag == "Player")
            {
                other.gameObject.GetComponent<Player>().Damage(damage, effect, crit, _owner);
            }
        }

        // destroy on collide with anything
        DestroyOb();
        
    }

    void DestroyOb()
    {
        Destroy(gameObject, 2); // let particles die
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
        try
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        catch { }

        foreach (Transform ob in transform)
        {
            try
            {
                ob.GetComponent<MeshRenderer>().enabled = false;
            }
            catch { }
        }

        dead = true;
    }
}
