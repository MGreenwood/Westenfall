using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Attributes")]
public class Attributes : ScriptableObject
{
    public enum StatTypes { Strength, Dexterity, Spirit, Stamina, Magic, NONE }

    public const float Bonus_Mod = 0.2f;
    public const float StamToHP = 12.4f;
    public const float SpiritToMP = 14.8f;

    [Serializable]
    public class Stat
    {
        public StatTypes statType;
        public int value;

        public Stat(StatTypes sType, int val)
        {
            statType = sType;
            value = val;
        }
    }
    

    [SerializeField]
    List<Stat> _stats;

    [SerializeField]
    List<Weapon.Stat> _weaponStats;
    [SerializeField]
    List<Armor.Stat> _armorStats;


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

        return null;
    }

    public Stat[] GetStats()
    {
        return _stats.ToArray();
    }
    public List<Weapon.Stat> GetWeaponStats() => _weaponStats;
    public List<Armor.Stat> GetArmorStats() => _armorStats;

    public void IncrementWeaponStat(Weapon.Stat stat)
    {
        int index = _weaponStats.IndexOf(_weaponStats.Find(x => x._stat == stat._stat));
        _weaponStats[index]._value += new Weapon.Stat(stat._stat, stat._value, stat._flatValue, stat._isBasicStat)._value;
    }

    public void SetStat(StatTypes statType, int value)
    {
        foreach(Stat s in _stats)
        {
            if(s.statType == statType)
            {
                s.value = value;
            }
        }
    }

    public Weapon.Stat GetWeaponStat(Weapon.Stat statType)
    {
        foreach (Weapon.Stat s in _weaponStats)
        {
            if (s._stat == statType._stat)
            {
                return s;
            }
        }

        return null;
    }

    public Armor.Stat GetArmorStat(Armor.Stat statType)
    {
        foreach (Armor.Stat s in _armorStats)
        {
            if (s._stat == statType._stat)
            {
                if (_statsDirty)
                    CalculateStats();

                return s;
            }
        }

        return null;
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


