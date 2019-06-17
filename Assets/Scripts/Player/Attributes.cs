using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Attributes")]
public class Attributes : ScriptableObject
{
    public enum StatTypes { Strength, Dexterity, Spirit, Stamina, Magic, NONE }
    public enum WeaponBonuses { }
    public enum ArmorBonuses { }

    public const float Bonus_Mod = 0.2f;
    public const float StamToHP = 12.4f;
    public const float MagicToMP = 14.8f;

    [Serializable]
    public struct Stat
    {
        public StatTypes statType;
        public int value;
    }

    public struct WeaponStat
    {
        public WeaponBonuses _weaponBonus;
        public int _value;

        public WeaponStat(WeaponBonuses weaponBonus, int value)
        {
            _weaponBonus = weaponBonus;
            _value = value;
        }
    }

    public struct ArmorStat
    {
        public ArmorBonuses _armorBonus;
        public int _value;

        public ArmorStat(ArmorBonuses armorBonus, int value)
        {
            _armorBonus = armorBonus;
            _value = value;
        }
    }

    [SerializeField]
    List<Stat> _stats;

    List<WeaponStat> _weaponStats;
    List<ArmorStat> _armorStats;


    bool _statsDirty = true;
    bool _weapStatsDirty = true;
    bool _armorStatsDirty = true;
     
    public Stat GetStat(StatTypes statType)
    {
        foreach(Stat s in _stats)
        {
            if(s.statType == statType)
            {
                if (_statsDirty)
                    CalculateStats();

                return s;
            }
        }

        return new Stat();
    }

    private void CalculateStats()
    {
        // use gear to determine stats


        _statsDirty = false;
    }

    private void CalculateWeaponStats()
    {
        // use gear to determine stats


        _weapStatsDirty = false;
    }

    private void CalculateArmorStats()
    {
        // use gear to determine stats


        _armorStatsDirty = false;
    }

    public void SetStatsDirty()
    {
        _statsDirty = true;
    }

    public void SetWeaponStatsDirty()
    {
        _weapStatsDirty = true;
    }

    public void SetArmorStatsDirty()
    {
        _armorStatsDirty = true;
    }
}


