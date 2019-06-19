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
    [SerializeField]
    Attributes _startingAttributes;
    Attributes _attributes; // these are the attributes chosen by the player
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
        _startingAttributes = Instantiate(_startingAttributes);
        _attributes = Instantiate(Resources.Load("ScriptableObjects/Attributes/BaselineAttributes")) as Attributes;
        _equipment = GetComponent<Equipment>();

        SetupPlayer();

        _health = _maxHealth;
        _mana = _maxMana;

        StartCoroutine(ResourceRegen());
    }

    void SetupPlayer()
    {
        // load assigned stats and used stats
        // used attribute points = 
        _appliedAttributes = new AppliedAttributes();
        _availableAttributePoints = _level * _statsPerLevel - usedAttributePoints;
        CalculateStats();
        // load equipment from file

        // load inventories

        // load spells

        // load preferences (options menu selections) & keybinds
        /*
        _maxHealth = (int)((float)_startingAttributes.GetStat(Attributes.StatTypes.Stamina).value * Attributes.StamToHP);
        _maxMana = (int)((float)_startingAttributes.GetStat(Attributes.StatTypes.Spirit).value * Attributes.SpiritToMP);
        */
        _maxHealth = (int)((float)_attributes.GetStat(Attributes.StatTypes.Stamina).value * Attributes.StamToHP);
        _maxMana = (int)((float)_attributes.GetStat(Attributes.StatTypes.Spirit).value * Attributes.SpiritToMP);
        

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
    }

    public void SetClass(PlayerClass.ClassType classType)
    {
        _classType = classType;
    }

    public bool EquipArmor(InventoryItemObject inventoryItemObject) // right click equip
    {
        // Player ensures the item can be equipped, sends to Equipment class that manages the actual equipping


        // ensure the stat requirements are met
        foreach (Attributes.Stat stat in (inventoryItemObject.GetItem() as Armor).GetStatRequirements())
        {
            if (_attributes.GetStat(stat.statType).value < stat.value)
            {
                Debug.Log("Stat requirements not met -- " + stat.statType);
                return false;
            }
        }

        // if slot is currently taken, return the inventory object that is currently equipped


        // actually equip the item
        _equipment.Equip(inventoryItemObject);

        // place item in applicable location

        CalculateStats();

        return true;
    }

    public bool EquipWeapon(InventoryItemObject inventoryItemObject)
    {
        // Player ensures the item can be equipped, sends to Equipment class that manages the actual equipping


        // ensure the stat requirements are met
        foreach(Attributes.Stat stat in (inventoryItemObject.GetItem() as Weapon).GetStatRequirements())
        {
            if(_attributes.GetStat(stat.statType).value < stat.value)
            {
                Debug.Log("Stat requirements not met -- " + stat.statType);
                return false;
            }
        }

        // if slot is currently taken, return the inventory object that is currently equipped


        // actually equip the item
        _equipment.Equip(inventoryItemObject);

        // place weapon on characters back/hip as appropriate


        // update stats
        CalculateStats();

        return true;
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

            // get weapon/armor basic stats
            foreach(Attributes.Stat wStat in _equipment.GetBasicStats())
            {
                if(wStat.statType == stat.statType)
                {
                    value += wStat.value;
                }
            }

            // get selected attribute points
            int statType = Convert.ToInt32(stat.statType);
            value += _appliedAttributes.GetValue(statType);

            _attributes.SetStat(stat.statType, value);

            value = 0;
        }

        float percentageValue = 0f;
        int   flatValue       = 0 ;
        // Weapon stats

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