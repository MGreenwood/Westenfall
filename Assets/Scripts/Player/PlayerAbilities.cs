using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    /*
     * Purpose: Attached to Player Object and manages casting all abilities on skill bar.
     *          Also manages cooldowns and global cooldown timer
     * */

    [SerializeField] private Ability[] abilities;
    [SerializeField] private bool[] available; // parallel array stating if off cooldown // true == available to cast
    private int numAbilities = 5;
    private int lastIndex = -1; // the last ability that was cast

    // events
    public delegate void OnCast(string abilityName, float castTime, CastingComplete castingComplete);
    public static OnCast onCast; // used ONLY by cast bar, This is when button is pressed, not when skill is activated
    public delegate void OnCastCancel();
    public static OnCastCancel onCastCancel;
    private bool casting = false;
    public delegate void CastingComplete(); // invoked by cast bar when timer is up
    private CastingComplete castingComplete;

    private bool gcd = false;

    public const float Global_Cooldown = 0.3f;

    private void Start()
    {
        //abilities = new Ability[numAbilities];
        //available = new bool[numAbilities];


        abilities[0].SetOwner(gameObject);
        abilities[1].SetOwner(gameObject);

        castingComplete += CastingFinished; // subscribe to event for cast bar to invoke
    }

    void SetAbility(int index, Ability ability)
    {
        abilities[index] = ability;
        abilities[index].SetOwner(gameObject);

        available[index] = true;
    }

    private void Update()
    {
        if (UIManager.instance._isOverUI)
            return;

        // INPUT
        if (!casting)
        {
            

            if (Input.GetMouseButtonDown(0))
            {
                ActivateAbility(0);
            }
            if (Input.GetMouseButtonDown(1))
            {
                ActivateAbility(1);
            }
        }

        // Cast Canceling
        if(casting && Input.GetKeyDown(KeyCode.Escape))
        {
            casting = false;
            onCastCancel?.Invoke();
        }
    }

    void CastingFinished()
    {
        abilities[lastIndex].Cast();
        casting = false;
        StartCoroutine(CooldownManager(lastIndex));
    }

    void ActivateAbility(int index)
    {
        if (available[index] && !gcd)
        {
            casting = true;
            onCast?.Invoke(abilities[index].abilityName, abilities[index].CastTime, castingComplete);
            lastIndex = index;
            StartCoroutine(GlobalCooldown());
        }
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
