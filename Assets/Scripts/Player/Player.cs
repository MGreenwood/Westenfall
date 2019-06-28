using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour, IDamageable, ICanInvul, IHasAttributes
{
    public static Player instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField]
    PlayerClass.ClassType _classType;

    int _maxHealth = 100;
    int _health;
    int _maxMana = 100;
    int _mana;
    int _level = 20;
    
    [Serializable]
    class AppliedAttributes
    {
        public int[] values = new int[5]; //Strength, Dexterity, Spirit, Stamina, Magic

        public AppliedAttributes()
        {
            // all values will be set to 0 by default
            // they can then be loaded from DB later
        }

        public void ApplyAttribute(int index, int value)
        {
            values[index] += value; // trust
        }

        public int GetValue(int index)
        {
            return values[index];
        }
    }

    /*
     * 
     * This is where values can be changed to change gameplay
     * 
     * */

    const int _statsPerLevel = 5;

    /* end data variables
     * 
     * */


    //
    // Attributes
    // stored values
    [SerializeField]
    Attributes _startingAttributes;
    Attributes _attributes; // these are the attributes chosen by the player

    // calculated fields
    AppliedAttributes _appliedAttributes;
    int usedAttributePoints = 0;
    int _availableAttributePoints;
    // end Attributes
    //

    [SerializeField]
    Inventory _inventory;
    Equipment _equipment;

    [SerializeField]
    TextMeshProUGUI _statsText;

    // STATUS EFFECTS
    bool isInvulnerable = false;
    bool _stunned;
    Coroutine currentStun;

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

    // ----- Knockback
    public void Knockback(Vector3 source, int power) // power will be either 1, 2, or 3
    {
        GetComponent<PlayerController>().Knockback(source, power);
        Stun(PlayerController.delayTime);
    }
    // ----- end Knockback


    // ----- STUN
    public bool Stunned { get { return _stunned; } }
    public void Stun(float time)
    {
        if (currentStun != null)
            StopCoroutine(currentStun);

        currentStun = StartCoroutine(StunForTime(time));
    }
    IEnumerator StunForTime(float time)
    {
        _stunned = true;
        yield return new WaitForSeconds(time);
        _stunned = false;
    }
    // ----- end STUN

    // ----- Invulnerable
    IEnumerator MakeInvulnerable(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        isInvulnerable = false;
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
    // -----end Invulnerable


    public event HealthChanged healthChanged;

    private void Start()
    {      
        _equipment = GetComponent<Equipment>();

        SetupPlayer();

        StartCoroutine(ResourceRegen());
    }

    void SetupPlayer()
    {
        // load assigned stats and used stats
        // used attribute points = 
        _appliedAttributes = new AppliedAttributes();
        _availableAttributePoints = _level * _statsPerLevel - usedAttributePoints;
        _startingAttributes = Instantiate(_startingAttributes);
        _attributes = Instantiate(Resources.Load("ScriptableObjects/Attributes/BaselineAttributes")) as Attributes;
        CalculateStats();
        // load equipment from file

        // load inventories

        // load spells

        // load preferences (options menu selections) & keybinds
        /*
        _maxHealth = (int)((float)_startingAttributes.GetStat(Attributes.StatTypes.Stamina).value * Attributes.StamToHP);
        _maxMana = (int)((float)_startingAttributes.GetStat(Attributes.StatTypes.Spirit).value * Attributes.SpiritToMP);
        */
        
        _health = _maxHealth;
        _mana = _maxMana;
    }

    void UpdateStatDisplay()
    {
        string stats = "Basic Stats\n";
        foreach (Attributes.Stat a in _attributes.GetStats())
        {
            stats +=  $"{a.statType.ToString() + ":", -15} <color=#08ff02>{a.value}</color>\n";
        }

        stats += $"\nOffensive Attributes\n";
        int toIgnore = 3;// ignore the basic stats available to weapons
        foreach (Weapon.Stat a in _attributes.GetWeaponStats())
        {
            if(toIgnore > 0)
            {
                toIgnore--;
                continue;
            }

            string percent = a._flatValue ? "" : "%";
            stats += $"{a._stat.ToString() + ":",-25} {a._value, -1}{percent, 1}\n";
        }

        stats += $"\nDefensive Attributes\n";
        toIgnore = 2; // ignore the basic stats available to armor

        foreach (Armor.Stat a in _attributes.GetArmorStats())
        {
            if (toIgnore > 0)
            {
                toIgnore--;
                continue;
            }

            stats += $"{a._stat.ToString() + ":",-25} {a._value}\n";
        }

        // set the stats textbox text
        _statsText.text = stats;

        // update any attributes that are affected by stat changes
        _maxHealth = (int)((float)_attributes.GetStat(Attributes.StatTypes.Stamina).value * Attributes.StamToHP + _attributes.GetArmorStat(Armor.Stats.Health)._value);

        _maxMana = (int)((float)_attributes.GetStat(Attributes.StatTypes.Spirit).value * Attributes.SpiritToMP + _attributes.GetArmorStat(Armor.Stats.Mana)._value);

        if (_health > _maxHealth)
            _health = _maxHealth;
        if (_mana > _maxMana)
            _mana = _maxMana;

        D_HealthChanged?.Invoke();
        D_ManaChanged?.Invoke();
    }

    public void SetClass(PlayerClass.ClassType classType)
    {
        _classType = classType;
    }

    public bool EquipArmor(InventoryItemObject inventoryItemObject) // right click equip
    {
        // ensure the stat requirements are met
        foreach (Attributes.Stat stat in (inventoryItemObject.GetItem() as Armor).GetStatRequirements())
        {
            if (_attributes.GetStat(stat.statType).value < stat.value)
            {
                Debug.Log("Stat requirements not met -- " + stat.statType);
                return false;
            }
        }

        // level requirements
        if (inventoryItemObject.GetItem().GetLevelRequirement() > _level)
        {
            Debug.Log($"Requires level {inventoryItemObject.GetItem().GetLevelRequirement()}, you are level {_level}");
            return false;
        }


        // actually equip the item
        _equipment.Equip(inventoryItemObject, ref _attributes);


        // place item in applicable location on character

        UpdateStatDisplay();

        return true;
    }

    public bool EquipWeapon(InventoryItemObject inventoryItemObject)
    {
        // ensure the stat requirements are met
        foreach(Attributes.Stat stat in (inventoryItemObject.GetItem() as Weapon).GetStatRequirements())
        {
            if(_attributes.GetStat(stat.statType).value < stat.value)
            {
                Debug.Log("Stat requirements not met -- " + stat.statType);
                return false;
            }
        }

        // level requirements
        if (inventoryItemObject.GetItem().GetLevelRequirement() > _level)
        {
            Debug.Log($"Requires level {inventoryItemObject.GetItem().GetLevelRequirement()}, you are level {_level}");
            return false;
        }

        // actually equip the item
        _equipment.Equip(inventoryItemObject, ref _attributes);

        // place weapon on characters back/hip as appropriate


        // update stats
        UpdateStatDisplay();

        return true;
    }

    public void Unequip(Item.EquipmentSlot slot)
    {
        _equipment.Unequip(slot, ref _attributes);
        UpdateStatDisplay();
    }

    public Inventory GetInventory() => _inventory;

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

    public void CalculateStats()
    {
        int value = 0;

        // Basic stats
        foreach(Attributes.Stat stat in _attributes.GetStats())
        {
            // get starter values
            value += _startingAttributes.GetStat(stat.statType).value;
            
            _attributes.SetStat(stat.statType, value);

            value = 0;
        }

        _maxHealth = (int)((float)_attributes.GetStat(Attributes.StatTypes.Stamina).value * Attributes.StamToHP);
        _maxMana = (int)((float)_attributes.GetStat(Attributes.StatTypes.Spirit).value * Attributes.SpiritToMP);

        UpdateStatDisplay();
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
                AddHealth((int)((float)_attributes.GetStat(Attributes.StatTypes.Dexterity).value * Attributes.Bonus_Mod + _attributes.GetArmorStat(Armor.Stats.HealthRegen)._value));
            }

            if (_mana < _maxMana)
            {
                AddMana((int)((float)_attributes.GetStat(Attributes.StatTypes.Spirit).value * Attributes.Bonus_Mod + _attributes.GetArmorStat(Armor.Stats.ManaRegen)._value));
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

    public void ApplyEffect(Effect.AbilityEffect effect, GameObject abilityOwner)
    {
        throw new NotImplementedException();
    }

    public void LevelUp()
    {
        _level++;
        _availableAttributePoints += _statsPerLevel;
        UpdateStatDisplay();

        // play animation, make a sound, whatever
        // bing bang boom levelup WOOOO
        
    }
}