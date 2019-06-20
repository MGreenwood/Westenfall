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

        public static Stat operator +(Stat s1, Stat s2)
        {
            return new Stat(s1.statType, s1.value + s2.value);
        }
        public static Stat operator -(Stat s1, Stat s2)
        {
            if(s1.value - s2.value < 0)
                return new Stat(s1.statType, 0);

            return new Stat(s1.statType, s1.value - s2.value);
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
        _weaponStats[index]._value += stat._value;
    }

    public void IncrementArmorStat(Armor.Stat stat)
    {
        int index = _armorStats.IndexOf(_armorStats.Find(x => x._stat == stat._stat));
        _armorStats[index]._value += stat._value;
    }

    public void IncrementStat(Stat stat)
    {
        int index = _stats.IndexOf(_stats.Find(x => x.statType == stat.statType));
        _stats[index].value += stat.value;
    }

    public void DecrementStat(Stat stat)
    {
        int index = _stats.IndexOf(_stats.Find(x => x.statType == stat.statType));
        _stats[index].value -= stat.value;
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

    public void SetWeaponStat(Weapon.Stat stat)
    {
        foreach (Weapon.Stat s in _weaponStats)
        {
            if (s._stat == stat._stat)
            {
                s._value = stat._value;
            }
        }
    }

    public void SetArmorStat(Armor.Stat stat)
    {
        foreach (Armor.Stat s in _armorStats)
        {
            if (s._stat == stat._stat)
            {
                s._value = stat._value;
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

    public Armor.Stat GetArmorStat(Armor.Stats statType)
    {
        foreach (Armor.Stat s in _armorStats)
        {
            if (s._stat == statType)
            {
                return s;
            }
        }

        return null;
    }
}


