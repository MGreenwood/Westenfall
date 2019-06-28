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

    public const float Global_Cooldown = 0.3f;

    private void Start()
    {
        //abilities = new Ability[numAbilities];
        //available = new bool[numAbilities];
        SetupPlayer();
        abilities[0] = Instantiate(abilities[0]);
        abilities[1] = Instantiate(abilities[1]);
        abilities[2] = Instantiate(abilities[2]);
        abilities[3] = Instantiate(abilities[3]);
        abilities[0].SetOwner(gameObject);
        abilities[1].SetOwner(gameObject);
        abilities[2].SetOwner(gameObject);
        abilities[3].SetOwner(gameObject);

        castingComplete += CastingFinished; // subscribe to event for cast bar to invoke
    }

    void SetupPlayer() // load save
    {
        player = GetComponent<Player>();
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
        if (!casting && !player.Stunned)
        {
            if (Input.GetMouseButtonDown(0) && abilities[0] != null) // primary
            {
                ActivateAbility(0);
            }
            else if (Input.GetMouseButtonDown(1) && abilities[1] != null) // secondary
            {
                ActivateAbility(1);
            }
            else if (Input.GetKeyDown(KeyCode.Space) && abilities[2] != null) // movement
            {
                ActivateAbility(2);
            }
            else if (Input.GetKeyDown(KeyCode.E) && abilities[3] != null) // movement
            {
                ActivateAbility(3);
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
        if(abilities[lastIndex].Cast())
        {
            if (abilities[lastIndex] is Movement)
                D_ControllingMovement?.Invoke(true);

            StartCoroutine(CooldownManager(lastIndex));
            player.RemoveMana(abilities[lastIndex].GetCost());
        }

        casting = false;
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
