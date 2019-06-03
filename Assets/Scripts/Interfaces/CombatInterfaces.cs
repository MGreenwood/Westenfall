public delegate void DamageTaken(float dmg, Effect.EffectType effectType, bool crit);
public delegate void HealthChanged();

public interface IDamageable
{
    void Damage(float damage, Effect.EffectType effectType);
    event DamageTaken damageTaken;
    event HealthChanged healthChanged;
}

public interface IEnemyDamageable
{
    void Damage(float damage, Effect.EffectType effectType, bool crit);
    event DamageTaken damageTaken;
    event HealthChanged healthChanged;
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