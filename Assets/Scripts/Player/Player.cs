using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    private void Awake()
    {
        instance = this;
    }

    PlayerAttributes _attributes;
    Inventory _inventory;
    Item[] _equipment;

    int _temp;

    private void Start()
    {
        _attributes = new PlayerAttributes();
    }

    public void EquipArmor(Armor armor) // right click equip
    {

    }

    public void EquipArmor(Armor armor, int slot) // equip for specific slot
    {
        
    }

    public bool PickupItem(Item item)
    {
        return _inventory.AddItem(item);
    }
}
