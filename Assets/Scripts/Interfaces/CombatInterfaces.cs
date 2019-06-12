using UnityEngine;

public delegate void DamageTaken(float dmg, Effect.AbilityEffect effect, bool crit);

public interface IDamageable
{
    void Damage(int damage, Effect.AbilityEffect effectType, bool crit, GameObject abilityOwner);
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