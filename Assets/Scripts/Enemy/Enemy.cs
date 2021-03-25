using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IKillable, IHasAttributes
{
    private float health, speed;
    public float MAX_HEALTH;
    public GameObject combatText;

    public event DamageTaken damageTaken;

    public delegate void OnDeath();
    public OnDeath onDeath;
    public delegate void HealthChanged();
    public HealthChanged healthChanged;

    [SerializeField]
    Attributes _attributes;

    [SerializeField]
    public EnemySpawner.EnemyTypes _enemyType;

    EnemyBehaviorManager _behaviorManager;

    bool isDead = false;

    private void Start()
    {
        health = MAX_HEALTH;
        _attributes = Instantiate(_attributes);
        _behaviorManager = GetComponent<EnemyBehaviorManager>();
    }

    public float Health
    {
        get { return health; }
    }

    public Enemy(float health_, float speed_, float MAX_HEALTH_)
    {
        health = health_;
        speed = speed_;
        MAX_HEALTH = MAX_HEALTH_;
    }

    public void Damage(float dmg, Effect.AbilityEffect effect, bool crit, GameObject abilityOwner)
    {
        if (crit)
            dmg *= 1.25f;

        health -= dmg;                       // remove damage from health
        health = health < 0 ? 0f : health;   // do not allow below 0

        if (health <= 0 && !isDead)
        {
            // inform player that it was killed
            abilityOwner.GetComponent<Player>().AddExperience(1200000); // TODO
            int experience = 1200000;
            print("Killed " + name + ": Gained " + experience + " experience");

            Kill();
        }

        damageTaken?.Invoke(dmg, new Effect.AbilityEffect(), crit); // inform subscribers that HP has changed
        healthChanged?.Invoke();
        _behaviorManager.Damaged(abilityOwner, dmg);

        if(effect.effectType != Effect.EffectType.Basic)
            ApplyEffect(effect, abilityOwner);
        
    }

    void SubsequentDamage(Effect.AbilityEffect effect, GameObject abilityOwner)
    {
        if (isDead)
            return;

        int damage = effect.value;

        health -= damage;                       // remove damage from health
        health = health < 0 ? 0f : health;   // do not allow below 0

        if (health <= 0)
            Kill();

        damageTaken?.Invoke(damage, effect, false); // inform subscribers that HP has changed
        healthChanged?.Invoke();
        _behaviorManager.Damaged(abilityOwner, damage);
    }

    public void ApplyEffect(Effect.AbilityEffect effect, GameObject abilityOwner)
    {
        switch(effect.effectType)
        {
            case Effect.EffectType.Bleed:
            case Effect.EffectType.Burn:
            case Effect.EffectType.Poison:
            {
                StartCoroutine(ApplyDamageEffect(effect, abilityOwner));
            }
                break;
            case Effect.EffectType.Root:
                break;
        }
    }

    IEnumerator ApplyDamageEffect(Effect.AbilityEffect effect, GameObject abilityOwner)
    {
        int ticksRemaining = effect.numTicks;
        while(ticksRemaining > 0)
        {
            SubsequentDamage(effect, abilityOwner);
            ticksRemaining--;
            yield return new WaitForSeconds(1);
        }
    }

    public void Kill()
    {
        isDead = true;
        // drop any items

        // activate death animation in subroutine


        // remove this and place in death subroutine TODO
        onDeath?.Invoke(); // inform subscribed methods that enemy has died
        _behaviorManager.StopAllCoroutines();
        _behaviorManager.enabled = false;
        GetComponent<Collider>().enabled = false;

        GetComponent<LootTable>()?.DropItems();
        //Destroy(gameObject);
    }

    public void SubscribeToDeath(OnDeath method)
    {
        onDeath += method;
    }

    public Attributes GetAttributes() => _attributes;

    public void Knockback(Vector3 source, int power)
    {
        throw new NotImplementedException();
    }

    public void Stun(float time)
    {
        throw new NotImplementedException();
    }
}
