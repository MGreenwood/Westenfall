using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Armor")]
public class Armor : Item
{
    Item.ItemSize[] ArmorSizes = { new Item.ItemSize(2, 2), // Head
                                    new Item.ItemSize(3,4), // Chest
                                    new Item.ItemSize(2,2), // Hands
                                    new Item.ItemSize(2,3), // Feet
    };

    public enum Stats
    {
        Stamina, Spirit, // Base Stats           
        HealthRegen, ManaRegen, Defense, MagicDefence, Health, Mana, // defensive / regen
        MoveSpeed // misc
    } 

    [System.Serializable]
    public class Stat
    {
        public Stats _stat;
        public float _value;
        public bool _isBasicStat;

        public Stat(Stats stat, float value, bool isBasicStat)
        {
            _stat = stat;
            _value = value;
            _isBasicStat = isBasicStat;
        }

        public static Stat operator +(Stat s1, Stat s2)
        {
            return new Stat(s1._stat, s1._value + s2._value, s1._isBasicStat);
        }

        public static Stat operator -(Stat s1, Stat s2)
        {
            if(s1._value - s2._value <0)
                return new Stat(s1._stat, 0, s1._isBasicStat);

            return new Stat(s1._stat, s1._value - s2._value, s1._isBasicStat);
        }
    }

    public enum Slot { Head, Chest, Hands, Feet }

    [SerializeField]
    private Slot _slot;

    [SerializeField]
    int _armorValue;

    [SerializeField]
    List<Stat> _stats;

    [SerializeField]
    bool isMagicItem;

    [SerializeField]
    int maxStats;

    [SerializeField]
    Attributes.Stat[] _statRequirements;
    
    public override void init()
    {
        switch (_slot)
        {
            case Slot.Head:
                _itemSize = ArmorSizes[0];
                break;
            case Slot.Chest:
                _itemSize = ArmorSizes[1];
                break;
            case Slot.Hands:
                _itemSize = ArmorSizes[2];
                break;
            case Slot.Feet:
                _itemSize = ArmorSizes[3];
                break;
        }
    }

    public void Pickup()
    {
        // report to inventory that player is attempting to pickup this item
        if(Player.instance.PickupItem(this))
        {
            
        }
    }

    public Attributes.Stat[] GetStatRequirements() => _statRequirements;
    public List<Stat> GetStats() => _stats;
}
