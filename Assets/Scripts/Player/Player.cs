using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable, ICanInvul, IHasAttributes
{
    public static Player instance;

    private void Awake()
    {
        instance = this;
    }

    int _maxHealth = 100;
    int _health;
    int _maxMana = 100;
    int _mana;
    [SerializeField]
    PlayerClass.ClassType _classType;

    [SerializeField]
    Attributes _attributes;
    Inventory _inventory;
    Item[] _equipment;

    bool isInvulnerable = false;

    int _temp;

    public delegate void HealthChanged();
    public HealthChanged D_HealthChanged;
    public delegate void ManaChanged();
    public ManaChanged D_ManaChanged;

    public event DamageTaken damageTaken;
    
    public void ActivateInvulnerability(float seconds)
    {
        isInvulnerable = true;
        StartCoroutine(MakeInvulnerable(seconds));
    }

    IEnumerator MakeInvulnerable(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        isInvulnerable = false;
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }

    public event HealthChanged healthChanged;

    private void Start()
    {
        _attributes = Instantiate(_attributes);
        _health = _maxHealth;
        _mana = _maxMana;
    }

    public void SetClass(PlayerClass.ClassType classType)
    {
        _classType = classType;
    }

    public void EquipArmor(Armor armor) // right click equip
    {

    }

    public void EquipArmor(Armor armor, int slot) // equip for specific slot
    {
        
    }

    public bool PickupItem(Item item)
    {
        return _inventory.AddItem(item);
    }

    public void Damage(int damage, Effect.EffectType effectType, bool crit, GameObject abilityOwner)
    {
        if (damage >= _health)
        {
            _health -= _health;
            TriggerDeath();
        }
        else
        {
            _health -= damage;
            D_HealthChanged?.Invoke();

            switch (effectType)
            {
                case Effect.EffectType.Basic:
                    {

                    }
                    break;
            }
        }
    }

    private void TriggerDeath()
    {

    }

    public Attributes GetAttributes()
    {
        return _attributes;
    }

    public void RemoveMana(int val)
    {
        _mana -= val;
        D_ManaChanged?.Invoke();
    }

    public int GetMaxHealth() => _maxHealth;
    public int GetCurrentHealth() => _health;
    public int GetMaxMana() => _maxMana;
    public int GetCurrentMana() => _mana;

    public void SubscribeToHealthChanged(HealthChanged hc)
    {
        D_HealthChanged += hc;
    }

    public void SubscribeToManaChanged(ManaChanged mc)
    {
        D_ManaChanged += mc;
    }
}