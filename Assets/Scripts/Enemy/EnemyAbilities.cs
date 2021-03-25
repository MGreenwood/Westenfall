using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyAbilities : MonoBehaviour
{
    /*
     * Purpose: Attached to Enemy Object and manages casting all abilities on skill bar.
     *          Also manages cooldowns and global cooldown timer
     * */
     
    [SerializeField] private Ability[] abilities;
    [SerializeField] private bool[] available; // parallel array stating if off cooldown // true == available to cast
    private int lastIndex = -1; // the last ability that at least started casting

    private bool casting = false;
    private bool gcd = false;

    public const float Global_Cooldown = 0.3f;

    private void Awake()
    {
        for(int i=0;i<abilities.Length;i++)
        {
            abilities[i] = Instantiate(abilities[i]);
            abilities[i].SetOwner(gameObject);
        }
    }

    public bool IsAvailable(int index) => available[index];
    

    void SetAbility(int index, Ability ability)
    {
        abilities[index] = ability;
        abilities[index].SetOwner(gameObject);

        available[index] = true;
    }

    public Ability[] GetAbilities() => abilities;   

    void CastingFinished()
    {
        if(abilities[lastIndex].Cast())
        {
            StartCoroutine(CooldownManager(lastIndex));
            StartCoroutine(GlobalCooldown());
        }

        casting = false;
    }

    public bool ActivateAbility(int index)
    {
        bool successful = false;

        if (available[index] && !gcd && !casting)
        {
            lastIndex = index;

            if (abilities[index].CastTime > 0f) // do not begin cast bar for instant cast abilities
            {
                casting = true;
                CastCall(abilities[index].abilityName, abilities[index].CastTime, index);
            }
            else
            {
                CastingFinished();
            }

            successful = true;
        }

        return successful;
    }

    void CastCall(string abilityName, float castTime, int index)
    {
        StartCoroutine(Cast(abilityName, castTime, index));
    }

    IEnumerator Cast(string abilityName, float castTime, int index)
    {
        yield return new WaitForSeconds(castTime);
        CastingFinished();
    }

    IEnumerator CooldownManager(int index)
    {
        available[index] = false;

        float cd = abilities[index].cooldown;
        yield return new WaitForSeconds(cd);

        available[index] = true;
    }

    IEnumerator GlobalCooldown()
    {
        gcd = true;
        yield return new WaitForSeconds(Global_Cooldown);
        gcd = false;
    }
}
