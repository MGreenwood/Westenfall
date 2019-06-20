using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : Item
{
    public enum Stats { Strength, Dexterity, Magic, // Base Stats
                        PhysicalDamage, MagicDamage, CritRate, CritDamage, MinDamage, MaxDamage, // Damage Mods
                        CooldownReduction, HpOnHit // Misc
    } 

    [System.Serializable]
    public class Stat
    {
        public Stats _stat;
        public float _value;
        public bool _flatValue;
        public bool _isBasicStat;

        public Stat(Stats stat, float value, bool flatValue, bool isBasicStat)
        {
            _stat = stat;
            _value = value;
            _flatValue = flatValue;
            _isBasicStat = isBasicStat;
        }

        public static Stat operator+(Stat s1, Stat s2)
        {
            return new Stat(s1._stat, s1._value + s2._value,s1._flatValue, s1._isBasicStat);
        }

        public static Stat operator -(Stat s1, Stat s2)
        {
            if(s1._value - s2._value >= 0)
                return new Stat(s1._stat, s1._value - s2._value, s1._flatValue, s1._isBasicStat);
            else
                return new Stat(s1._stat, 0f, s1._flatValue, s1._isBasicStat);
        }
    }

    Item.ItemSize[] WeaponSizes = { new Item.ItemSize(2, 5), // Main Hand
                                    new Item.ItemSize(3,4),  // Shield
    };

    public enum WeaponType { Sword, Staff, Bow }
    public enum SlotType { MainHand, OffHand }
    
    [SerializeField]
    List<Stat> _stats;

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
    int maxStats;
    
    public override void init()
    {
        switch (_slotType)
        {
            case SlotType.MainHand:
                _itemSize = WeaponSizes[0];
                break;
            case SlotType.OffHand:
                _itemSize = WeaponSizes[1];
                break;
        }
    }

    public WeaponType GetWeaponType() => _weaponType;

    public List<Stat> GetStats()
    {
        return _stats;
    }

    public Attributes.Stat[] GetStatRequirements() => _statRequirements;
}
