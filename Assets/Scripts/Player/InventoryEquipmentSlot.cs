using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryEquipmentSlot : MonoBehaviour, IEquippableArea
{
    [SerializeField]
    Item.ItemType _itemType;
    [SerializeField]
    Weapon.SlotType _weaponSlot;
    [SerializeField]
    Armor.Slot _armorSlot;


    public InventoryEquipmentSlot GetEquipmentSlot()
    {
        return this;
    }
}
