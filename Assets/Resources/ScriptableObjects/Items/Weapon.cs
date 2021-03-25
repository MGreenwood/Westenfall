using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : Item
{
    public enum Stats { PhysicalDamage, MagicDamage, CritRate, CritDamage, MinDamage, MaxDamage, // Damage Mods
        CooldownReduction, HpOnHit, CastingSpeed // Misc
    }

    [System.Serializable]
    public class Stat
    {
        public Stats _stat;
        public float _value;
        public bool _flatValue;
        public bool _isBasicStat;

        public Stat(Stats stat, float value, bool flatValue)
        {
            _stat = stat;
            _value = value;
            _flatValue = flatValue;
        }

        public static Stat operator +(Stat s1, Stat s2)
        {
            return new Stat(s1._stat, s1._value + s2._value, s1._flatValue);
        }

        public static Stat operator -(Stat s1, Stat s2)
        {
            if (s1._value - s2._value >= 0)
                return new Stat(s1._stat, s1._value - s2._value, s1._flatValue);
            else
                return new Stat(s1._stat, 0f, s1._flatValue);
        }

        public static Stat operator *(Stat stat, float val)
        {
            return new Stat(stat._stat, stat._value * val, stat._flatValue);
        }
    }

    public enum WeaponType { ShortSword, LongSword, Staff, Bow, Hammer, Daggers, Hatchet, Greataxe, Mace, Wand }
    public enum SlotType { MainHand, OffHand }
    
    [SerializeField]
    List<Stat> _stats;

    [SerializeField]
    float _minDamage = 0f;
    [SerializeField]
    float _maxDamage = 0f;

    [SerializeField]
    float _magicDamage = 0f;

    [Space(15)]
    [SerializeField]
    private WeaponType _weaponType;

    [SerializeField]
    Attributes.Stat[] _statRequirements;

    [SerializeField]
    SlotType _slotType;

    [SerializeField]
    bool isMagicItem;

    [SerializeField]
    int _maxStats;
    
    public override void init()
    {
        switch (_weaponType)
        {
            case WeaponType.ShortSword:
            case WeaponType.Daggers:
            case WeaponType.Mace:
                _itemSize = new ItemSize(2, 4);
                break;
            case WeaponType.LongSword:
            case WeaponType.Staff:
            case WeaponType.Bow:
            case WeaponType.Hammer:
                _itemSize = new ItemSize(2, 5);
                break;
            case WeaponType.Wand:
                _itemSize = new ItemSize(1, 4);
                break;
        }

        _itemType = ItemType.Weapon;
    }

    public WeaponType GetWeaponType() => _weaponType;
    public void SetWeaponType(WeaponType wTypeIn) => _weaponType = wTypeIn;

    public void SetAttackDamage(float min, float max)
    {
        _minDamage = min;
        _maxDamage = max;
    }
    public void SetMagicDamage(float dmg) => _magicDamage = dmg;

    public List<Stat> GetStats()
    {
        return _stats;
    }

    public void AddStat(Stat statIn)
    {
        _stats.Add(statIn);
    }

    public Attributes.Stat[] GetStatRequirements() => _statRequirements;
    public int GetMaxStats() => _maxStats;

    public float GetMinDamage() => _minDamage;
    public float GetMaxDamage() => _maxDamage;
    public float GetMagicDamage() => _magicDamage;
}
