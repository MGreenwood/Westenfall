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

    private int lastIndex = -1; // the last ability that was cast

    // events
    public delegate void OnCast(string abilityName, float castTime, CastingComplete castingComplete);
    public static OnCast onCast; // used ONLY by cast bar, This is when button is pressed, not when skill is activated
    public delegate void OnCastCancel();
    public delegate void ControllingMovement(bool isControlling);
    public ControllingMovement D_ControllingMovement;
    public static OnCastCancel onCastCancel;
    public delegate void OnAbilityChanged();
    public OnAbilityChanged D_AbilityChanged;
    private bool casting = false;
    public delegate void CastingComplete(); // invoked by cast bar when timer is up
    private CastingComplete castingComplete;

    Player player;

    private bool gcd = false;

    public const float Global_Cooldown = 0.45f;

    private void Start()
    {
        //abilities = new Ability[numAbilities];
        //available = new bool[numAbilities];
        SetupPlayer();
        int numAbilities = abilities.Length;
        for(int i = 0; i < abilities.Length; ++i)
        {
            if (abilities[i] != null)
            {
                abilities[i] = Instantiate(abilities[i]);
                abilities[i].SetOwner(gameObject);
            }
        }

        castingComplete += CastingFinished; // subscribe to event for cast bar to invoke
    }

    void SetupPlayer() // load save
    {
        player = GetComponent<Player>();
    }

    public void SetAbility(int index, Ability ability)
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
        if (!casting && !player.Stunned)
        {
            if (Input.GetButtonDown("Weapon1") && abilities[0] != null) // primary
            {
                ActivateAbility(0);
            }
            else if (Input.GetButtonDown("Weapon2") && abilities[1] != null) // secondary
            {
                ActivateAbility(1);
            }
            else if (Input.GetButtonDown("Ability1") && abilities[2] != null) // movement
            {
                ActivateAbility(2);
            }
            else if (Input.GetButtonDown("Ability2") && abilities[3] != null) // movement
            {
                ActivateAbility(3);
            }
            else if (Input.GetButtonDown("Ability3") && abilities[4] != null) // movement
            {
                ActivateAbility(4);
            }
            else if (Input.GetButtonDown("Movement") && abilities[5] != null) // movement
            {
                ActivateAbility(5);
            }
        }

        // Cast Canceling
        if(casting && Input.GetKeyDown(KeyCode.Escape))
        {
            casting = false;
            onCastCancel?.Invoke();
            player.GetComponent<PlayerController>().MovementPaused(false);
        }
    }

    void CastingFinished()
    {
        if(abilities[lastIndex].Cast())
        {
            if (abilities[lastIndex] is Movement)
                D_ControllingMovement?.Invoke(true);

            StartCoroutine(CooldownManager(lastIndex));
            player.RemoveMana(abilities[lastIndex].GetCost());
        }

        casting = false;
        player.GetComponent<PlayerController>().MovementPaused(false);
    }

    void ActivateAbility(int index)
    {
        if (available[index] && !gcd && player.GetCurrentMana() > abilities[index].GetCost())
        {
            lastIndex = index;
            StartCoroutine(GlobalCooldown());

            if (abilities[index].CastTime > 0f) // do not begin cast bar for instant cast abilities
            {
                casting = true;
                onCast?.Invoke(abilities[index].abilityName, abilities[index].CastTime, castingComplete);
                player.GetComponent<PlayerController>().MovementPaused(true);
            }
            else
            {
                CastingFinished();
            }
        }
    }

    IEnumerator CooldownManager(int index)
    {
        available[index] = false;

        float cd = abilities[index].cooldown;

        if(cd > 0f)
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
