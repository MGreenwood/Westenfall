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
        Dexterity, Stamina, // Base Stats           
        HealthRegen, ManaRegen, Defense, MagicDefence, Health, Mana // defensive / regen
    } 

    public struct Stat
    {
        public Stats _stat;
        public float _value;

        public Stat(Stats stat, float value)
        {
            _stat = stat;
            _value = value;
        }
    }

    public enum Slot { Head, Chest, Hands, Feet }

    [SerializeField]
    private Slot _slot;

    [SerializeField]
    List<Stat> _stats;

    /*public void CreateNew(Slot slot, Item.Rarity rarity, params Stat[] stats)
    {
        _itemType = ItemType.Armor;
        _rarity = rarity;
        _slot = slot;

        

        foreach(Stat s in stats)
        {
            _stats.Add(s);
        }
    }*/

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
}
