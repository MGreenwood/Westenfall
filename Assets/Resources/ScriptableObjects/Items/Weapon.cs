using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class Weapon : Item
{
    public enum Stats { Strength, Intellect, // Base Stats
                        PhysicalDamage, MagicDamage, CritRate, CritDamage, MinDamage, MaxDamage, // Damage Mods
                        CooldownReduction, HpOnHit // Misc
    } 
    public struct Stat
    {
        public readonly Stats _stat;
        public readonly float _value;
        public readonly bool _flatValue;

        public Stat(Stats stat, float value, bool flatValue)
        {
            _stat = stat;
            _value = value;
            _flatValue = flatValue;
        }
    }

    Item.ItemSize[] WeaponSizes = { new Item.ItemSize(2, 5), // Main Hand
                                    new Item.ItemSize(3,4),  // Shield
    };

    public enum WeaponType { Sword, Staff, Dagger }
    public enum SlotType { MainHand, OffHand }
    
    [SerializeField]
    List<Stat> _stats;
    [Space(15)]
    [SerializeField]
    private WeaponType _weaponType;

    [SerializeField]
    SlotType _slotType;

    /*public void CreateNew(WeaponType weaponType, params Stat[] stats)
    {
        _stats = new List<Stat>();

        _itemType = ItemType.Weapon;

        foreach(Stat s in stats)
        {
            _stats.Add(s);
        }

        init();
    }*/

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

    

    public List<Stat> GetStats()
    {
        return _stats;
    }
}
