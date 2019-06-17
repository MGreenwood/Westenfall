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
        SetupPlayer();

        _health = _maxHealth;
        _mana = _maxMana;

        StartCoroutine(ResourceRegen());
    }

    void SetupPlayer()
    {
        _maxHealth = (int)((float)_attributes.GetStat(Attributes.StatTypes.Stamina).value * Attributes.StamToHP);
        _maxMana = (int)((float)_attributes.GetStat(Attributes.StatTypes.Magic).value * Attributes.MagicToMP);
    }

    public void SetClass(PlayerClass.ClassType classType)
    {
        _classType = classType;
    }

    public bool EquipArmor(Armor armor, GameObject inventoryItemObject) // right click equip
    {


        return false;
    }

    public bool EquipWeapon(Weapon weapon, GameObject inventoryItemObject)
    {
        // ensure the stat requirements are met
        foreach(Attributes.Stat stat in weapon.GetStatRequirements())
        {
            if(_attributes.GetStat(stat.statType).value < stat.value)
            {
                Debug.Log("Stat requirements not met -- " + stat.statType);
                return false;
            }
        }

        // if slot is currently taken, return the inventory object that is currently equipped


        // actually equip the item
        

        return true;
    }

    public bool PickupItem(Item item)
    {
        return _inventory.AddItem(item);
    }

    public void Damage(int damage, Effect.AbilityEffect effect, bool crit, GameObject abilityOwner)
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

            switch (effect.effectType)
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

    IEnumerator ResourceRegen()
    {
        while (Application.isPlaying)
        {
            if (_health < _maxHealth)
            {
                AddHealth((int)((float)_attributes.GetStat(Attributes.StatTypes.Dexterity).value * Attributes.Bonus_Mod));
            }

            if (_mana < _maxMana)
            {
                AddMana((int)((float)_attributes.GetStat(Attributes.StatTypes.Spirit).value * Attributes.Bonus_Mod));
            }

            yield return new WaitForSeconds(1);
        }
    }

    public void AddHealth(int val)
    {
        _health += val;
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        D_HealthChanged?.Invoke();
    }

    public void AddMana(int val)
    {
        _mana += val;
        _mana = Mathf.Clamp(_mana, 0, _maxMana);
        D_ManaChanged?.Invoke();
    }

    public void ApplyEffect(Effect.AbilityEffect effect)
    {
        throw new NotImplementedException();
    }
}