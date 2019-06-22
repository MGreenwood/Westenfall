using UnityEngine;

public delegate void DamageTaken(float dmg, Effect.AbilityEffect effect, bool crit);

public interface IDamageable
{
    void Damage(int damage, Effect.AbilityEffect effectType, bool crit, GameObject abilityOwner);
    void ApplyEffect(Effect.AbilityEffect effect, GameObject abilityOwner);
    void Knockback(Vector3 source, int power);
    void Stun(float time);
    event DamageTaken damageTaken;
}

public interface IHealeable
{
    void Heal(float healAmount);
    float Health { get; }
}

public interface IKillable
{
    void Kill();
}

public interface ICanInvul
{
    void ActivateInvulnerability(float seconds);
    bool IsInvulnerable();
}

public interface IHasAttributes
{
    Attributes GetAttributes();
}

public interface ICanLimitMovement
{
    // must call player.GetComponent<PlayerController>().MovementPaused(false);
    void EndMovement();
}