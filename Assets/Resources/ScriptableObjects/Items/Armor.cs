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

    public enum ArmorClass { Light, Medium, Heavy }
    public enum Stats
    {
        HealthRegen, ManaRegen, Defense, MagicDefence, Health, Mana, // defensive / regen
        MoveSpeed, evasion // misc
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
        }

        public static Stat operator +(Stat s1, Stat s2)
        {
            return new Stat(s1._stat, s1._value + s2._value, s1._flatValue);
        }

        public static Stat operator -(Stat s1, Stat s2)
        {
            if(s1._value - s2._value <0)
                return new Stat(s1._stat, 0, s1._flatValue);

            return new Stat(s1._stat, s1._value - s2._value, s1._flatValue);
        }
        
        public static Stat operator *(Stat stat, float val)
        {
            return new Stat(stat._stat, stat._value * val, stat._flatValue);
        }
    }

    [SerializeField]
    private Item.EquipmentSlot _slot;


    [SerializeField]
    private ArmorClass _armorClass;

    [SerializeField]
    int _armorValue;

    [SerializeField]
    List<Stat> _stats;

    [SerializeField]
    bool isMagicItem;

    [SerializeField]
    int _maxStats;

    [SerializeField]
    Attributes.Stat[] _statRequirements;
    
    public override void init()
    {
        switch (_slot)
        {
            case Item.EquipmentSlot.Head:
                _itemSize = ArmorSizes[0];
                break;
            case Item.EquipmentSlot.Chest:
                _itemSize = ArmorSizes[1];
                break;
            case Item.EquipmentSlot.Hands:
                _itemSize = ArmorSizes[2];
                break;
            case Item.EquipmentSlot.Feet:
                _itemSize = ArmorSizes[3];
                break;
        }

        _itemType = ItemType.Armor;
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
    public int GetMaxStats() => _maxStats;

    public void AddStat(Stat statIn)
    {
        _stats.Add(statIn);
    }

    public void SetArmorClass(ArmorClass classIn) => _armorClass = classIn;
    public Item.EquipmentSlot GetEquipmentSlot() => _slot;
}
